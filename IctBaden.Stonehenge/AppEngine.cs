using System;
using System.Diagnostics;
using System.Net;

namespace IctBaden.Stonehenge
{
  public class AppEngine
  {
    public string Title { get; set; }
    private readonly int port;

    public AppEngine()
      : this(42000)
    {

    }
    public AppEngine(int hostPort)
    {
      port = hostPort;
    }

    public void Run(bool newWindow)
    {
      var listeningOn = string.Format("http://*:{0}/", port);
      var appHost = new AppHost(Title);
      appHost.Init();
      try
      {
        appHost.Start(listeningOn);
      }
      catch (HttpListenerException ex)
      {
        if (ex.ErrorCode == 5)
        {
          throw new AccessViolationException("You need administartion privileges or use HttpCfg");
        }
        throw;
      }

      

      if (!newWindow)
        return;

      var cmd = Environment.OSVersion.Platform == PlatformID.Unix ? "chromium-browser" : "chrome.exe";
      var parameter = string.Format("--app=http://localhost:{0}/App/index.html?title={1} --app-window-size=800,600", port, Title);
      var ui = Process.Start(cmd, parameter);
      if (ui == null)
      {
        Console.WriteLine("AppHost failed to create UI");
        return;
      }
      Console.WriteLine("AppHost Created at {0}, listening on {1}", DateTime.Now, listeningOn);
      ui.WaitForExit();
    }
  }
}