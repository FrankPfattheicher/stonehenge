using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using ServiceStack.Common.Web;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints.Support;

namespace IctBaden.Stonehenge.Services
{
  public class AppEventService : AppService
  {
    private const int MaxDelay = 10000;

    public object Get(AppEvent request)
    {
      var appSession = GetSession(request.SessionId, false);
      if (appSession == null)
        return new NotFoundHttpHandler();
      appSession.Accessed();

      Debug.WriteLine("EventService:" + request.ViewModel);

      appSession.EventRelease.WaitOne(MaxDelay);
      Thread.Sleep(10);

      var values = new Dictionary<string, object>();
      var vm = appSession.ViewModel as ActiveViewModel;
      if ((vm == null) || (vm.GetType().FullName != request.ViewModel) || !vm.SupportsEvents)
      {
        return new HttpResult("{}", "application/json");
      }
      lock (appSession.Events)
      {
        values.Add("stonehenge_poll", 1);
        foreach (var name in appSession.Events.Where(name => !string.IsNullOrEmpty(name) && !values.ContainsKey(name)))
        {
          if (name == PropertyNameMessageBox)
          {
            var title = vm.MessageBoxTitle;
            var text = vm.MessageBoxText;
            values.Add("stonehenge_eval", string.Format("app.showMessage('{0}','{1}');", HttpUtility.JavaScriptStringEncode(text), HttpUtility.JavaScriptStringEncode(title)));
          }
          else if (name == PropertyNameNavigate)
          {
            var route = vm.NavigateToRoute;
            values.Add("stonehenge_navigate", route);
          }
          else
          {
            var pi = vm.GetPropertyInfo(name);
            if (pi == null)
              continue;
            var bindable = pi.GetCustomAttributes(false).OfType<BindableAttribute>().ToArray();
            if ((bindable.Length > 0) && !bindable[0].Bindable)
              continue;
            values.Add(name, vm.TryGetMember(name));
          }
        }
        appSession.Events.Clear();
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
        httpResult.Headers.Add("Expires", "0");
        return httpResult;
      }
      return new HttpResult(values, "application/json");
    }
  }
}