using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Drawing;

namespace IctBaden.Stonehenge
{
  public class AppEngine
  {
    public string Title { get; set; }
    public string StartPage { get; set; }
    public Point WindowSize { get; set; }
    private readonly int port;
    private AppHost host;

    public event Action<AppSession> NewSession;

    public string UserRole
    {
      get { return (host != null) ? host.UserRole : null; }
      set { if (host != null) host.UserRole = value; }
    }
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
      var listeningOn = string.Format("http://*:{0}/", port);
      host = new AppHost(Title, StartPage);
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

      host.NewSession += OnNewSession;

      if (!newWindow)
        return;

      
  		var cmd = Environment.OSVersion.Platform == PlatformID.Unix ? "chromium-browser" : "chrome.exe";
			var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

	    var dir = Directory.CreateDirectory(path);
			var parameter = string.Format("--app=http://localhost:{0}/App/index.html?title={1} --app-window-size={2},{3} --disable-translate --user-data-dir=\"{4}\"",
        port, Title, WindowSize.X, WindowSize.Y, path);
      var ui = Process.Start(cmd, parameter);
      if (ui == null)
      {
        Console.WriteLine("AppHost failed to create UI");
        return;
      }
      Console.WriteLine("AppHost Created at {0}, listening on {1}", DateTime.Now, listeningOn);
      ui.WaitForExit();
			dir.Delete(true);
    }

    private void OnNewSession(AppSession session)
    {
      var handler = NewSession;
      if (handler == null)
        return;
      handler(session);
    }
  }
}