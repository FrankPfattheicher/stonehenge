namespace IctBaden.Stonehenge2.SimpleHttp
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    internal class HttpProcessor
    {
        public TcpClient Socket;
        public SimpleHttpServer Server;

        private StreamReader inputStream;
        public StreamWriter OutputStream;

        public string HttpMethod;
        public string HttpUrl;
        public string HttpProtocolVersionstring;
        public Hashtable HttpHeaders = new Hashtable();

        private const int MaxPostSize = 10 * 1024 * 1024; // 10MB

        public HttpProcessor(TcpClient socket, SimpleHttpServer server)
        {
            Socket = socket;
            Server = server;
        }

        public void Process()
        {
            // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            // "processed" view of the world, and we want the data raw after the headers
            inputStream = new StreamReader(Socket.GetStream());

            // we probably shouldn't be using a streamwriter for all output from handlers either
            OutputStream = new StreamWriter(new BufferedStream(Socket.GetStream()), new UTF8Encoding(false)) { NewLine = "\r\n" };
            try
            {
                ParseRequest();
                ReadHeaders();
                if (HttpMethod.Equals("GET"))
                {
                    HandleGetRequest();
                }
                else if (HttpMethod.Equals("POST"))
                {
                    HandlePostRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
                WriteNotFound();
            }
            try
            {
                OutputStream.Flush();
                Socket.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            inputStream = null; 
            OutputStream = null;
        }

        public void ParseRequest()
        {
            var request = inputStream.ReadLine();
            if (request == null)
            {
                throw new Exception("invalid http request line");
            }
            var tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            HttpMethod = tokens[0].ToUpper();
            HttpUrl = tokens[1];
            HttpProtocolVersionstring = tokens[2];

            Console.WriteLine("starting: " + request);
        }

        public void ReadHeaders()
        {
            Console.WriteLine("ReadHeaders()");
            string line;
            while ((line = inputStream.ReadLine()) != null)
            {
                if (line.Equals(""))
                {
                    Console.WriteLine("got headers");
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                Console.WriteLine("header: {0}:{1}", name, value);
                HttpHeaders[name] = value;
            }
        }

        public void HandleGetRequest()
        {
            Server.HandleGetRequest(this);
        }

        private const int BufSize = 4096;
        public void HandlePostRequest()
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            Console.WriteLine("get post data start");
            var ms = new MemoryStream();
            if (HttpHeaders.ContainsKey("Content-Length"))
            {
                var contentLen = Convert.ToInt32(HttpHeaders["Content-Length"]);
                if (contentLen > MaxPostSize)
                {
                    throw new Exception(
                        string.Format("POST Content-Length({0}) too big for this simple server",
                          contentLen));
                }
                var buf = new byte[BufSize];
                var toRead = contentLen;
                while (toRead > 0)
                {
                    Console.WriteLine("starting Read, toRead={0}", toRead);
                    var numread = inputStream.BaseStream.Read(buf, 0, Math.Min(BufSize, toRead));
                    Console.WriteLine("read finished, numread={0}", numread);
                    if (numread == 0)
                    {
                        if (toRead == 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("client disconnected during post");
                        }
                    }
                    toRead -= numread;
                    ms.Write(buf, 0, numread);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            Console.WriteLine("get post data end");
            Server.HandlePostRequest(this, new StreamReader(ms));
        }

        public void WriteSuccess(string contentType = "text/html")
        {
            // this is the successful HTTP response line
            OutputStream.WriteLine("HTTP/1.0 200 OK");
            // these are the HTTP headers...          
            OutputStream.WriteLine("Content-Type: " + contentType);
            OutputStream.WriteLine("Connection: close");
            // ..add your own headers here if you like

            OutputStream.WriteLine(""); // this terminates the HTTP headers.. everything after this is HTTP body..
        }

        public void WriteNotFound()
        {
            // this is an http 404 failure response
            OutputStream.WriteLine("HTTP/1.0 404 File not found");
            // these are the HTTP headers
            OutputStream.WriteLine("Connection: close");
            // ..add your own headers here

            OutputStream.WriteLine(""); // this terminates the HTTP headers.
        }

        public void WriteRedirect(string redirectionUrl)
        {
            // this is an http 404 failure response
            OutputStream.WriteLine("HTTP/1.0 302 Found");
            // these are the HTTP headers
            OutputStream.WriteLine("Location: " + redirectionUrl);
            // ..add your own headers here

            OutputStream.WriteLine(""); // this terminates the HTTP headers.
        }

    }

    internal class SimpleHttpServer
    {
        protected int Port;
        private TcpListener listenerSocket;

        private Thread listenerThread;

        public bool IsActive { get; private set; }

        internal SimpleHttpServer(int port)
        {
            Port = port;
            IsActive = false;
        }

        public void Start()
        {
            IsActive = true;
            listenerThread = new Thread(Listen);
            listenerThread.Start();
        }
        public void Terminate()
        {
            IsActive = false;

            var listener = listenerSocket;
            listenerSocket = null;

            if (listener != null)
            {
                listener.Stop();
            }

            var thread = listenerThread;
            listenerThread = null;

            if (thread != null)
            {
                thread.Abort();
            }

        }

        public void Listen()
        {
            listenerSocket = new TcpListener(IPAddress.Any, Port);
            listenerSocket.Start();
            while (IsActive)
            {
                try
                {
                    var socket = listenerSocket.AcceptTcpClient();
                    var processor = new HttpProcessor(socket, this);
                    var processThread = new Thread(processor.Process);
                    processThread.Start();
                    Thread.Sleep(1);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }


        public event Action<HttpProcessor> HandleGet;
        public event Action<HttpProcessor, StreamReader> HandlePost;

        internal void HandleGetRequest(HttpProcessor processor)
        {
            HandleGet?.Invoke(processor);
        }

        internal void HandlePostRequest(HttpProcessor processor, StreamReader inputData)
        {
            HandlePost?.Invoke(processor, inputData);
        }
    }

}
