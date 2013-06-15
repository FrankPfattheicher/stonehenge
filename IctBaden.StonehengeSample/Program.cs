using System;
using System.Threading;
using IctBaden.Stonehenge;

namespace IctBaden.StonehengeSample
{
  static class Program
  {
    public static AppEngine App;

    static void Main()
    {
      App = new AppEngine("Stonehenge Sample", "about");
      App.Run(true);
      //Thread.Sleep(TimeSpan.FromDays(1));
    }
  }
}
