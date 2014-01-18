using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Drawing;

namespace IctBaden.Stonehenge
{
  public class AppEngine
  {
    public Point WindowSize { get; set; }
    public string Title { get; set; }
    public string StartPage { get; set; }
    public bool MessageBoxContentHtml { get; set; }

    public TimeSpan SessionTimeout { get; set; }
    public bool HasSessionTimeout { get { return SessionTimeout.TotalMilliseconds > 0.1; } }

    private readonly int port;
    private AppHost host;
    private string listeningOn;

    public event Action<AppSession> SessionCreated;
    public event Action<AppSession> SessionTerminated;

    public void Redirect(string page)
    {
      if (host != null) 
        host.Redirect = page;
    }

    public AppEngine(string title, string startPage)
      : this(42000, title, startPage)
    {
    }
    public AppEngine(int hostPort, string title, string startPage)
    {
      port = hostPort;
      Title = title;
      StartPage = startPage;
      WindowSize = new Point(800, 600);
    }

    public void Run(bool newWindow)
    {
      listeningOn = string.Format("http://*:{0}/", port);
      host = new AppHost(Title, StartPage, MessageBoxContentHtml) { SessionTimeout = SessionTimeout };
      host.Init();
      try
      {
        host.Start(listeningOn);
      }
      catch (HttpListenerException ex)
      {
        if (ex.ErrorCode == 5)
        {
          throw new AccessViolationException("You need administartion privileges or use HttpCfg");
        }
        throw;
      }

      host.SessionCreated += OnSessionCreated;
      host.SessionTerminated += OnSessionTerminated;

      if (!newWindow)
        return;

      if (!ShowWindowChrome() &&
          !ShowWindowInternetExplorer() &&
          !ShowWindowFirefox() &&
          !ShowWindowSafari())
      {
        Debug.WriteLine("Could not create main window");
      }
    }

    public void Terminate()
    {
      if (host == null)
        return;

      host.Dispose();
      host = null;
    }

    private bool ShowWindowChrome()
    {
      var cmd = Environment.OSVersion.Platform == PlatformID.Unix ? "chromium-browser" : "chrome.exe";
      var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

      var dir = Directory.CreateDirectory(path);
      var parameter =
        string.Format(
          "--app=http://localhost:{0}/?title={1} --app-window-size={2},{3} --disable-translate --user-data-dir=\"{4}\"",
          port, Title, WindowSize.X, WindowSize.Y, path);
      var ui = Process.Start(cmd, parameter);
      if (ui == null)
      {
        return false;
      }
      Console.WriteLine("AppHost Created at {0}, listening on {1}", DateTime.Now, listeningOn);
      ui.WaitForExit();
      dir.Delete(true);
      return true;
    }

    private bool ShowWindowInternetExplorer()
    {
      if (Environment.OSVersion.Platform == PlatformID.Unix)
        return false;

      const string cmd = "iexplore.exe";
      var parameter = string.Format("-private http://localhost:{0}/?title={1}", port, Title);
      var ui = Process.Start(cmd, parameter);
      if (ui == null)
      {
        return false;
      }
      Console.WriteLine("AppHost Created at {0}, listening on {1}", DateTime.Now, listeningOn);
      ui.WaitForExit();
      return true;
    }

    private bool ShowWindowFirefox()
    {
      if (Environment.OSVersion.Platform == PlatformID.Unix)
        return false;

      const string cmd = "firefox.exe";
      var parameter = string.Format("http://localhost:{0}/?title={1} -width {2} -height {3}", port, Title, WindowSize.X, WindowSize.Y);
      var ui = Process.Start(cmd, parameter);
      if (ui == null)
      {
        return false;
      }
      Console.WriteLine("AppHost Created at {0}, listening on {1}", DateTime.Now, listeningOn);
      ui.WaitForExit();
      return true;
    }

    private bool ShowWindowSafari()
    {
      if (Environment.OSVersion.Platform == PlatformID.Unix)
        return false;

      const string cmd = "safari.exe";
      var parameter = string.Format("-url http://localhost:{0}/?title={1} -width {2} -height {3}", port, Title, WindowSize.X, WindowSize.Y);
      var ui = Process.Start(cmd, parameter);
      if (ui == null)
      {
        return false;
      }
      Console.WriteLine("AppHost Created at {0}, listening on {1}", DateTime.Now, listeningOn);
      ui.WaitForExit();
      return true;
    }

    private void OnSessionCreated(AppSession session)
    {
      var handler = SessionCreated;
      if (handler == null)
        return;
      handler(session);
    }
    private void OnSessionTerminated(AppSession session)
    {
      var handler = SessionTerminated;
      if (handler == null)
        return;
      handler(session);
    }
  }
}