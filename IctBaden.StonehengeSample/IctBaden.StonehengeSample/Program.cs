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
      App = new AppEngine {Title = "Stonehenge Sample"};
      App.Run(false);
      Thread.Sleep(TimeSpan.FromDays(1));
    }
  }
}
