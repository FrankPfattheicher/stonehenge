using System;
using System.Threading;
using IctBaden.Stonehenge;

namespace IctBaden.StonehengeSample
{
	public class GraphVm : ActivePresenter
	{
		public const int Count = 180;
		public long[][] GraphData { get; private set; }
		private Timer timer;
		private int start;

		public GraphVm()
		{
			GraphData = new long[Count][];
			for (var ix = 0; ix < Count; ix++)
			{
				GraphData[ix] = new long[2];
			}

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
			var time = (long)(DateTime.Parse("1.1.2000") - DateTime.Parse("1.1.1970")).TotalMilliseconds;
			for (var ix = 0; ix < Count; ix++)
			{
				GraphData[ix][0] = time;
				GraphData[ix][1] = (long)(Math.Sin((ix + start) * Math.PI / 36) * 40) + 50;
				time += 360000;
			}
			start += 1;
		}

	}
}