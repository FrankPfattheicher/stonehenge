using System;
using System.Net.Mime;
using System.Threading;
using IctBaden.Stonehenge;

namespace StonehengeApp
{
  static class Program
  {
    public static AppEngine App;
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main()
    {
      App = new AppEngine("$projectname$", "start");
      var asServer = Environment.CommandLine.Contains("/SVR");
      App.Run(!asServer);
      while (asServer && Thread.CurrentThread.IsAlive)
      {
        Thread.Sleep(1000);
      }
    }
  }
}
