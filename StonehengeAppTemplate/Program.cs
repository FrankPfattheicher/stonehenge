using System;
using IctBaden.Stonehenge;

namespace StonehengeApp
{
  static class Program
  {
    public static AppEngine App;
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      App = new AppEngine("$projectname$", "start");
      App.Run(true);
    }
  }
}
