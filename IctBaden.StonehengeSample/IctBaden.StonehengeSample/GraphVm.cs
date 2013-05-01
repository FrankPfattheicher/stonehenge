using System;

namespace IctBaden.StonehengeSample
{
  public class GraphVm
  {
    public const int Count = 180;
    public long[][] GraphData { get; private set; }

    public GraphVm()
    {
      GraphData = new long[Count][];

      var time = (long)(DateTime.Parse("1.1.2000") - DateTime.Parse("1.1.1970")).TotalMilliseconds;
      for (var ix = 0; ix < Count; ix++)
      {
        GraphData[ix] = new long[2];
        GraphData[ix][0] = time;
        GraphData[ix][1] = (long) (Math.Sin(ix * Math.PI / 36) * 40) + 50;
        time += 360000;
      }
    }
  }
}