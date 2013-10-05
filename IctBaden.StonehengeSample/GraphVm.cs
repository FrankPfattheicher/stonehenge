using System;
using System.Threading;
using IctBaden.Stonehenge;

namespace IctBaden.StonehengeSample
{
  public class GraphVm : ActiveViewModel, IDisposable
  {
    public const int Count = 180;

    public GraphSeries[] GraphData { get; private set; }
    public GraphOptions GraphOptions { get; private set; }

    private readonly Timer timer;
    private int start;

    private static long GetEpoch(DateTime timeStamp)
    {
      return (long)(timeStamp - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds * 1000;
    }

    public GraphVm(AppSession session)
      : base(session)
    {
      timer = new Timer(UpdateGraph, this, 200, 200);
      GenerateData();

      GraphOptions = new GraphOptions
        {
          yaxis = new GraphAxisOptions
            {
              position = "left",
              min = 0, 
              max = 100
            },
          xaxis = new GraphAxisOptions
            {
              mode = "time",
              timeformat = "%H:%M",
              min = GetEpoch(new DateTime(1970,1,1)).ToString(),
              max = GetEpoch(new DateTime(1970, 1, 1, 23, 59, 0)).ToString()
            },
          colors = new [] { "#1010FF" },
          series = new GraphSeriesOptions
          {
            lines = new GraphLinesOptions
            {
              lineWidth = 2,
              fill = true,
              fillColor = new GraphColors { colors = new[] { new GraphOpacity { opacity = 0.6 }, new GraphOpacity { opacity = 0.2 } } }
            }
          },
        };
    }

    private void UpdateGraph(object state)
    {
      GenerateData();
      NotifyPropertyChanged("GraphData");
    }

    private void GenerateData()
    {
      var points = new long[Count][];
      var time = (long)(DateTime.Parse("1.1.1970") - DateTime.Parse("1.1.1970")).TotalMilliseconds;
      for (var ix = 0; ix < Count; ix++)
      {
        points[ix] = new long[2];
        points[ix][0] = time;
        points[ix][1] = (long)(Math.Sin((ix + start) * Math.PI / 36) * 40) + 50;
        time += 360000;
      }
      start += 1;

      GraphData = new[]
        {
          new GraphSeries
            {
              label = "sin(x)",
              data = points
            }
        };
    }

    public void Dispose()
    {
      if (timer != null)
      {
        timer.Dispose();
      }
    }

    [ActionMethod]
    public void SetParameter(object param)
    {
      GraphOptions.colors = new[] { param.ToString() };
    }

  }
}