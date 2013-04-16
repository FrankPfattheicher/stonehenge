using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.Text;

namespace IctBaden.Stonehenge
{
  public class AppViewModelService : Service
  {
    public object Get(AppViewModel request)
    {
      Debug.WriteLine("AppViewModelService:" + request.ViewModel);

      var vm = Session.Get<object>("~vm");
      if ((vm == null) || (vm.GetType().FullName != request.ViewModel))
      {
        var asm = Assembly.GetEntryAssembly();
        var vmtype = asm.GetType(request.ViewModel);
        vm = Activator.CreateInstance(vmtype);
        Session.Set("~vm", vm);
      }

      var ty = vm.GetType();
      Debug.WriteLine("AppViewModelService: ~vm=" + ty.Name);

      var pi = ty.GetProperty(request.ViewModel);
      if(pi != null)
      {
        if (request.Source != null)
          pi.SetValue(vm, request.Source, null);
      }
      return new HttpResult(ServiceStack.Text.JsonSerializer.SerializeToString(vm), "application/json");
    }

    public object Post(AppViewModel request)
    {
      var vm = Session.Get<object>("~vm");
      if (vm != null && request.ViewModel == vm.GetType().FullName)
      {
        var ty = vm.GetType();

        foreach (var query in Request.FormData.AllKeys)
        {
          var pi = ty.GetProperty(query);
          if (pi == null) 
            continue;
          if(pi.CanWrite)
            pi.SetValue(vm, Request.FormData[query], null);
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
          var redirect = new HttpResult();
          redirect.StatusCode = HttpStatusCode.Redirect;
          redirect.Headers.Add("Location", Request.Headers["Referer"] + "#/" + host.Redirect);
          host.Redirect = null;
          return redirect;
        }
      }

      return Get(request);
    }
  }
}