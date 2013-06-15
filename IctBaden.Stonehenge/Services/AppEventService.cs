using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using ServiceStack.Common.Web;
using ServiceStack.Text;

namespace IctBaden.Stonehenge.Services
{
  public class AppEventService : AppService
  {
    private const int MaxDelay = 10000;

    public object Get(AppEvent request)
    {
      Debug.WriteLine("EventService:" + request.ViewModel);
      
      var delay = Environment.TickCount + MaxDelay;
      while (Environment.TickCount < delay)
      {
        Thread.Sleep(100);
        if (Events.Count > 0)
          break;
      }

      var values = new Dictionary<string, object>();
      var vm = ViewModel as ActiveViewModel;
      if ((vm == null) || (vm.GetType().FullName != request.ViewModel))
      {
        Response.Close();
        return null;
      }
      lock (Events)
      {
        foreach (var name in Events.Where(name => !values.ContainsKey(name)))
        {
          if (name == "MessageBox")
          {
            var title = vm.MessageBoxTitle;
            var text = vm.MessageBoxText;
            values.Add("eval", string.Format("app.showMessage('{0}','{1}');", text, title));
          }
          else
          {
            values.Add(name, vm.TryGetMember(name));
          }
        }
        Events.Clear();
      }

			if (!string.IsNullOrEmpty(RequestContext.CompressionType))
			{
				var compressed = new CompressedResult(Encoding.UTF8.GetBytes(JsonSerializer.SerializeToString(values)), RequestContext.CompressionType)
				{
					ContentType = "application/json"
				};
				var httpResult = new HttpResult(compressed.Contents, "application/json");
				httpResult.Headers.Add("CompressionType", RequestContext.CompressionType);
				return httpResult;
			}
			return new HttpResult(values, "application/json");
    }
  }
}