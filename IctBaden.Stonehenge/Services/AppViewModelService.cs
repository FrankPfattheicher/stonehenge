using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using IctBaden.Stonehenge.Services;
using ServiceStack.Common.Web;
using ServiceStack.Text;

namespace IctBaden.Stonehenge
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
      if(pi != null)
      {
        if (request.Source != null)
          pi.SetValue(vm, request.Source, null);
      }

      var data = new List<string>();
      var activeVm = vm as ActiveViewModel;
      string values;
      if (activeVm != null)
      {
        foreach (var model in activeVm.ActiveModels)
        {
          values = JsonSerializer.SerializeToString(model.Model);
          data.Add(values.Substring(1, values.Length - 2));
        }
      }

      foreach (var prop in vm.GetType().GetProperties())
      {
        var bindable = prop.GetCustomAttributes(typeof (BindableAttribute), true);
        if ((bindable.Length > 0) && !((BindableAttribute) bindable[0]).Bindable)
          continue;

        var value = prop.GetValue(vm, null);
        if(value == null)
          continue;
        
        values = "\"" + prop.Name + "\":" + JsonSerializer.SerializeToString(value);
        data.Add(values);
      }
      //values = JsonSerializer.SerializeToString(vm);
      //data.Add(values.Substring(1, values.Length - 2));

      var result = new HttpResult("{" + string.Join(",", data) + "}", "application/json");
      result.Headers.Add("Cache-Control", "no-cache");
      return result;
    }

    public object Post(AppViewModel request)
    {
      var vm = ViewModel;
      if (vm == null)
        return Get(request);

      if (request.ViewModel == vm.GetType().FullName)
      {
        var ty = vm.GetType();

        foreach (var query in Request.FormData.AllKeys)
        {
          var pi = ty.GetProperty(query);
          if ((pi == null) || !pi.CanWrite)
            continue;

          var newval = Request.FormData[query];

          SetPropertyValue(vm, pi, newval);
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
          var session = Session.Get<object>("~session") as AppSession;
          mi.Invoke(vm, new object[]{ session });
        }
      }

      var host = GetResolver() as AppHost;
      if (host != null)
      {
        if (host.Redirect != null)
        {
          var redirect = new HttpResult {StatusCode = HttpStatusCode.Redirect};
          redirect.Headers.Add("Location", Request.Headers["Referer"] + "#/" + host.Redirect);
          host.Redirect = null;
          return redirect;
        }
      }

      return Get(request);
    }

    private static void SetPropertyValue(object vm, PropertyInfo pi, string newval)
    {
      try
      {
        var val = TypeSerializer.DeserializeFromString(newval, pi.PropertyType);
        pi.SetValue(vm, val, null);
      }
      catch
      {
      }
    }
  }
}