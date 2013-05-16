using System;
using System.Threading;
using IctBaden.Stonehenge;

namespace IctBaden.StonehengeSample
{
	public class GraphVm : ActiveViewModel
	{
		public const int Count = 180;
		public GraphSeries[] GraphData { get; private set; }
		private Timer timer;
		private int start;

		public GraphVm(AppSession session) : base(session)
		{
			timer = new Timer(UpdateGraph, this, 200, 200);
			GenerateData();
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

		  GraphData = new GraphSeries[1]
		    {
		      new GraphSeries
		        {
		          label = "sin(x)",
		          data = points
		        }
		    };
    }

	}
}