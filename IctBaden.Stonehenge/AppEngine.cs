using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Drawing;
using IctBaden.Stonehenge.Services;
// ReSharper disable UnusedMember.Global

namespace IctBaden.Stonehenge
{
    public class AppEngine
    {
        public Point WindowSize { get; set; }
        public string Title { get; set; }
        public string StartPage { get; set; }
        public bool MessageBoxContentHtml { get; set; }
        public TimeSpan EventTimeout { get; set; }
        public bool HasEventTimeout => EventTimeout.TotalMilliseconds > 0.1;
        public TimeSpan SessionTimeout { get; set; }
        public bool HasSessionTimeout => SessionTimeout.TotalMilliseconds > 0.1;
        public string Protocol => _useSsl ? "https" : "http";
        public string ListeningOn { get; private set; }

        private readonly string _desiredHost = "*";
        private readonly int _desiredPort;
        public string UsedHost { get; private set; }
        public int UsedPort { get; private set; }
        private readonly bool _useSsl;
        private AppHost _host;

        public event Action<AppSession> SessionCreated;
        public event Action<AppSession> SessionTerminated;
        public event Action<Exception> ClientException;

        public void Redirect(string page)
        {
            if (_host != null)
            {
                _host.Redirect = page;
            }
        }

        // ReSharper disable once UnusedMember.Global
        public AppEngine(string title, string startPage)
            : this(42000, false, title, startPage)
        {
        }

        public AppEngine(string hostAddress, int hostPort, bool secure, string title, string startPage)
            : this(hostPort, secure, title, startPage)
        {
            _desiredHost = hostAddress;
        }
        public AppEngine(int hostPort, bool secure, string title, string startPage)
        {
            _desiredPort = hostPort;
            _useSsl = secure;
            Title = title;
            StartPage = startPage;
            WindowSize = new Point(800, 600);
        }

        public void Run(bool newWindow)
        {
            UsedHost = _desiredHost;
            UsedPort = (_desiredPort != 0) ? _desiredPort : 42000;
            ListeningOn = $"{Protocol}://{UsedHost}:{UsedPort}/";

            _host = new AppHost(Title, StartPage, MessageBoxContentHtml);
            if (HasEventTimeout)
            {
                _host.EventTimeout = EventTimeout;
            }
            if (HasSessionTimeout)
            {
                _host.SessionTimeout = SessionTimeout;
            }
            _host.Init();
            for (; ; )
            {
                try
                {
                    _host.Start(ListeningOn);
                    break;
                }
                catch (HttpListenerException ex)
                {
                    if (ex.ErrorCode == 5)
                    {
                        throw new AccessViolationException("You need administartion privileges or use HttpCfg");
                    }
                    if (((ex.ErrorCode == 32) || (ex.ErrorCode == 183)) && (_desiredPort == 0))
                    {
                        UsedPort = new Random().Next(5000, 65500);
                        ListeningOn = $"{Protocol}://{UsedHost}:{UsedPort}/";
                        continue;
                    }
                    if (ex.ErrorCode == 32)
                    {
                        // Skype ?
                        throw new AccessViolationException($"Http port {_desiredPort} already used by other application");
                    }
                    throw;
                }
            }

            _host.SessionCreated += OnSessionCreated;
            _host.SessionTerminated += OnSessionTerminated;
            _host.ClientException += OnClientException;

            if (!newWindow)
                return;

            if (!ShowWindowMidori() &&
                !ShowWindowChrome() &&
                !ShowWindowInternetExplorer() &&
                !ShowWindowFirefox() &&
                !ShowWindowSafari())
            {
                Debug.WriteLine("Could not create main window");
            }
        }

        public void Terminate()
        {
            var h = _host;
            _host = null;
            h?.Dispose();

            AppSessionCache.Terminate();
        }

        private bool ShowWindowChrome()
        {
            var cmd = Environment.OSVersion.Platform == PlatformID.Unix ? "chromium-browser" : "chrome.exe";
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            var dir = Directory.CreateDirectory(path);
            var parameter =
                $"--app={Protocol}://localhost:{UsedPort}/?title={Title} --window-size={WindowSize.X},{WindowSize.Y} --disable-translate --user-data-dir=\"{path}\"";
            var ui = Process.Start(cmd, parameter);
            if (ui == null)
            {
                return false;
            }
            Console.WriteLine("AppHost Created at {0}, listening on {1}", DateTime.Now, ListeningOn);
            ui.WaitForExit();
            dir.Delete(true);
            return true;
        }

        private bool ShowWindowMidori()
        {
            if (Environment.OSVersion.Platform != PlatformID.Unix)
                return false;

            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

            var dir = Directory.CreateDirectory(path);
            var parameter = $"-e Navigationbar -c {path} -a {Protocol}://localhost:{UsedPort}/?title={Title}";
            var ui = Process.Start("midori", parameter);
            if (ui == null)
            {
                return false;
            }
            Console.WriteLine("AppHost Created at {0}, listening on {1}", DateTime.Now, ListeningOn);
            ui.WaitForExit();
            dir.Delete(true);
            return true;
        }

        private bool ShowWindowInternetExplorer()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                return false;

            const string cmd = "iexplore.exe";
            var parameter = $"-private {Protocol}://localhost:{UsedPort}/?title={Title}";
            var ui = Process.Start(cmd, parameter);
            if (ui == null)
            {
                return false;
            }
            Console.WriteLine("AppHost Created at {0}, listening on {1}", DateTime.Now, ListeningOn);
            ui.WaitForExit();
            return true;
        }

        private bool ShowWindowFirefox()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                return false;

            const string cmd = "firefox.exe";
            var parameter =
                $"{Protocol}://localhost:{UsedPort}/?title={Title} -width {WindowSize.X} -height {WindowSize.Y}";
            var ui = Process.Start(cmd, parameter);
            if (ui == null)
            {
                return false;
            }
            Console.WriteLine("AppHost Created at {0}, listening on {1}", DateTime.Now, ListeningOn);
            ui.WaitForExit();
            return true;
        }

        private bool ShowWindowSafari()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                return false;

            const string cmd = "safari.exe";
            var parameter =
                $"-url {Protocol}://localhost:{UsedPort}/?title={Title} -width {WindowSize.X} -height {WindowSize.Y}";
            var ui = Process.Start(cmd, parameter);
            if (ui == null)
            {
                return false;
            }
            Console.WriteLine("AppHost Created at {0}, listening on {1}", DateTime.Now, ListeningOn);
            ui.WaitForExit();
            return true;
        }

        private void OnSessionCreated(AppSession session)
        {
            SessionCreated?.Invoke(session);
        }
        private void OnSessionTerminated(AppSession session)
        {
            SessionTerminated?.Invoke(session);
        }
        private void OnClientException(Exception exception)
        {
            ClientException?.Invoke(exception);
        }

        public void InvalidateCache()
        {
            ResourceLoader.InvalidateCache();
        }
    }
}