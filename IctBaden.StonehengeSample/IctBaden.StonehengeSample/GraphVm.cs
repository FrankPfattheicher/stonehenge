using System;

namespace IctBaden.StonehengeSample
{
  public class GraphVm
  {
    public const int Count = 180;
    public long[] GraphData { get; private set; }

    public GraphVm()
    {
      GraphData = new long[Count * 2];

      var time = (long)(DateTime.Parse("1.1.2000") - DateTime.Parse("1.1.1970")).TotalMilliseconds;
      for (var ix = 0; ix < Count; ix += 2)
      {
        GraphData[ix] = time;
        GraphData[ix + 1] = ix % 100;
      }
    }
  }
}