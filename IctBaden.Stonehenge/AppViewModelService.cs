using System;
using System.Diagnostics;
using System.Reflection;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;

namespace IctBaden.Stonehenge
{
  public class AppViewModelService : Service
  {
    public object Get(AppViewModel request)
    {
      Debug.WriteLine("AppViewModelService:" + request.Property);

      var vm = Session.Get<object>("~vm");
      if ((vm == null) || (vm.GetType().FullName != request.Property))
      {
        var asm = Assembly.GetEntryAssembly();
        var vmtype = asm.GetType(request.Property);
        vm = Activator.CreateInstance(vmtype);
        Session.Set("~vm", vm);
      }

      var ty = vm.GetType();
      Debug.WriteLine("AppViewModelService: ~vm=" + ty.Name);

      var pi = ty.GetProperty(request.Property);
      if(pi != null)
      {
        if (request.Value != null)
          pi.SetValue(vm, request.Value, null);
      }

      return new HttpResult(ServiceStack.Text.JsonSerializer.SerializeToString(vm), "application/json");
    }

    public object Post(AppViewModel request)
    {
      var vm = Session.Get<object>("~vm");
      if (vm != null && request.Property == vm.GetType().FullName)
      {
        var ty = vm.GetType();

        foreach (var query in Request.FormData.AllKeys)
        {
          var pi = ty.GetProperty(query);
          if (pi != null)
          {
            pi.SetValue(vm, Request.FormData[query], null);
          }
        }
      }
      return Get(request);
    }
  }
}