using System;
using System.Threading;
using IctBaden.Stonehenge;

namespace IctBaden.StonehengeSample
{
  static class Program
  {
    static void Main()
    {
      var host = new AppEngine {Title = "Stonehenge Sample"};
      host.Run(false);
      Thread.Sleep(TimeSpan.FromDays(1));
    }
  }
}
