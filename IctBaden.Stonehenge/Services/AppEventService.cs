using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        return new HttpResult("{}", "application/json");
      }
      lock (Events)
      {
        //if ((Events.Count > 0) && (Events.Last() == null))
        //{
        //  Events.Clear();
        //  return new HttpResult("{}", "application/json");
        //}
        values.Add("stonehenge_poll", 1);
        foreach (var name in Events.Where(name => !string.IsNullOrEmpty(name) && !values.ContainsKey(name)))
        {
          if (name == "MessageBox")
          {
            var title = vm.MessageBoxTitle;
            var text = vm.MessageBoxText;
            values.Add("stonehenge_eval", string.Format("app.showMessage('{0}','{1}');", text, title));
          }
          else
          {
            var pi = vm.GetPropertyInfo(name);
	          if (pi == null)
		          continue;
            var bindable = pi.GetCustomAttributes(false).OfType<BindableAttribute>().ToArray();
            if ((bindable.Length > 0) && !((BindableAttribute)bindable[0]).Bindable)
              continue;
            values.Add(name, vm.TryGetMember(name));
          }
        }
        Events.Clear();
      }

      if (!string.IsNullOrEmpty(RequestContext.CompressionType))
      {
        var properties = new List<string>();
        foreach (var value in values)
        {
          if (value.Value == null)
            continue;
          var txt = JsonSerializer.SerializeToString(value.Value);
          if (txt.StartsWith("\"{") && txt.EndsWith("}\""))
            txt = txt.Substring(1, txt.Length - 2).Replace("\\\"", "\"");
          properties.Add('"' + value.Key + '"' + ": " + txt);
        }
        var data = "{" + string.Join(",", properties) + "}";

        var compressed = new CompressedResult(Encoding.UTF8.GetBytes(data), RequestContext.CompressionType)
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