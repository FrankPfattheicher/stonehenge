using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text;
using ServiceStack.Common.Web;
using ServiceStack.Text;

namespace IctBaden.Stonehenge.Services
{
  public class AppViewModelService : AppService
  {
    public object Get(AppViewModel request)
    {
      Debug.WriteLine("ViewModelService:" + request.ViewModel);

      var vm = SetViewModelType(request.ViewModel);

      var ty = vm.GetType();
      Debug.WriteLine("AppViewModelService: ~vm=" + ty.Name);

      var pi = ty.GetProperty(request.ViewModel);
      if (pi != null)
      {
        if (request.Source != null)
          pi.SetValue(vm, request.Source, null);
      }

      var data = new List<string>();
      var activeVm = vm as ActiveViewModel;
      if (activeVm != null)
      {
        foreach (var model in activeVm.ActiveModels)
        {
          data.AddRange(SerializeObject(model.Model));
        }
      }

      data.AddRange(SerializeObject(vm));

      var result = "{" + string.Join(",", data) + "}";

      EventsClear();

      if (!string.IsNullOrEmpty(RequestContext.CompressionType))
      {
        var compressed = new CompressedResult(Encoding.UTF8.GetBytes(result), RequestContext.CompressionType)
          {
            ContentType = "application/json"
          };
        var httpResult = new HttpResult(compressed.Contents, "application/json");
        httpResult.Headers.Add("CompressionType", RequestContext.CompressionType);
        return httpResult;
      }
      return new HttpResult(result, "application/json");
    }

    public object Post(AppViewModel request)
    {
      var vm = ViewModel;
      if (vm == null)
        return Get(request);

      if (request.ViewModel == vm.GetType().FullName)
      {
        foreach (var query in Request.FormData.AllKeys)
        {
          var newval = Request.FormData[query];
          SetPropertyValue(vm, query, newval);
        }
      }

      var mi = vm.GetType().GetMethod(request.Source);
      if (mi != null)
      {
        if (mi.GetParameters().Length == 0)
        {
          mi.Invoke(vm, new object[0]);
        }
        else
        {
          var parameter = Request.FormData["_stonehenge_CommandParameter_"];
          mi.Invoke(vm, new object[] { parameter });
        }
      }

      var host = GetResolver() as AppHost;
      if (host != null)
      {
        if (host.Redirect != null)
        {
          var redirect = new HttpResult { StatusCode = HttpStatusCode.Redirect };
          redirect.Headers.Add("Location", Request.Headers["Referer"] + "#/" + host.Redirect);
          host.Redirect = null;
          return redirect;
        }
      }

      return Get(request);
    }

    private static IEnumerable<string> SerializeObject(object obj)
    {
      var data = new List<string>();

      foreach (var prop in obj.GetType().GetProperties())
      {
        var bindable = prop.GetCustomAttributes(typeof(BindableAttribute), true);
        if ((bindable.Length > 0) && !((BindableAttribute)bindable[0]).Bindable)
          continue;

        var value = prop.GetValue(obj, null);
        if (value == null)
          continue;

        string json;
        if (prop.PropertyType.Name == "GraphOptions")
        {
          json = "\"" + prop.Name + "\":" + value;
        }
        else
        {
          json = "\"" + prop.Name + "\":" + JsonSerializer.SerializeToString(value);
        }
        data.Add(json);
      }

      return data;
    }

    private static void SetPropertyValue(object vm, string propName, string newval)
    {
      try
      {

        var activeVm = vm as ActiveViewModel;
        if (activeVm != null)
        {
          var pi = activeVm.GetPropertyInfo(propName);
          if ((pi == null) || !pi.CanWrite)
            return;

          var val = JsonSerializer.DeserializeFromString(newval, pi.PropertyType);
          activeVm.TrySetMember(propName, val);
        }
        else
        {
          var pi = vm.GetType().GetProperty(propName);
          if ((pi == null) || !pi.CanWrite)
            return;

          var val = JsonSerializer.DeserializeFromString(newval, pi.PropertyType);
          pi.SetValue(vm, val, null);
        }
      }
      // ReSharper disable EmptyGeneralCatchClause
      catch
      {
      }
      // ReSharper restore EmptyGeneralCatchClause
    }
  }
}