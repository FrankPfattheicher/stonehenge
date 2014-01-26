using System;
using System.Linq;
using System.Net;
using ServiceStack.Common.Web;

namespace IctBaden.Stonehenge.Services
{
  public class UserFileService : AppService
  {
    public object Get(UserFile request)
    {
      var appSession = GetSession(request.SessionId);
      if (appSession != null)
      {
        var vm = appSession.ViewModel;
        if (vm != null)
        {
          var methodName = "Get" + request.FileName.Replace(".", "");
          var method = vm.GetType()
            .GetMethods()
            .FirstOrDefault(m => string.Compare(m.Name, methodName, StringComparison.InvariantCultureIgnoreCase) == 0);
          if (method != null)
          {
            if (method.ReturnType == typeof(UserData))
            {
              var data = (UserData)method.Invoke(vm, new object[0]);
              return new HttpResult(data.Bytes, data.ContentType);
            }
          }
        }
      }
      return new HttpResult("User file not found: " + request.FileName, HttpStatusCode.NotFound);
    }
  }
}