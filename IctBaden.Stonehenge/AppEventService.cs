using System;
using System.Collections.Generic;
using System.Threading;
using ServiceStack.Common.Web;

namespace IctBaden.Stonehenge
{
	public class AppEventService : AppService
	{
		private const int MaxDelay = 10000;

		public object Get(AppEvent request)
		{
			var delay = Environment.TickCount + MaxDelay;
			while (Environment.TickCount < delay)
			{
				Thread.Sleep(100);
				if (Events.Count > 0)
					break;
			}

			var values = new Dictionary<string, object>();
			var vm = ViewModel as ActiveViewModel;
			lock (Events)
			{
				if (vm != null)
				{
					foreach (var name in Events)
					{
						if (values.ContainsKey(name))
							continue;
						values.Add(name, vm[name]);
					}
				}
				Events.Clear();
			}

			return new HttpResult(values, "application/json");
		}
	}
}