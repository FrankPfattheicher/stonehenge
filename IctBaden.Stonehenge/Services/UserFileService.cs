using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using ServiceStack.Common.Web;

namespace IctBaden.Stonehenge.Services
{
    // ReSharper disable once UnusedMember.Global
    public class UserFileService : AppService
    {
        public object Get(UserFile request)
        {
            var appSession = new AppSession();
            object vm;

            if (request.Path1 == "rest")
            {
                vm = appSession.SetViewModelType(request.FileName);
            }
            else
            {
                var sessionId = GetSessionId();
                appSession = GetSession(sessionId);
                vm = appSession?.ViewModel;
            }

            if (vm == null)
            {
                var message = "UserFileService: ViewModel not found: " + request.FileName;
                Trace.TraceWarning(message);
                return new HttpResult(message, HttpStatusCode.NotFound);
            }
            var method = vm.GetType()
                .GetMethods()
                .FirstOrDefault(m => string.Compare(m.Name, "GetUserData", StringComparison.InvariantCultureIgnoreCase) == 0);

            if (method == null)
            {
                var message = "UserFileService: Method GetUserData not found: " + request.FileName;
                Trace.TraceWarning(message);
                return new HttpResult(message, HttpStatusCode.NotFound);
            }
            if (method.ReturnType != typeof(UserData))
            {
                var message = "UserFileService: Return type is not UserData: " + request.FileName;
                Trace.TraceWarning(message);
                return new HttpResult(message, HttpStatusCode.NotFound);
            }
            UserData data;
            if (method.GetParameters().Length == 2)
            {
                var parameters = Request.QueryString.AllKeys
                    .ToDictionary(n => n, n => Request.QueryString[n]);
                data = (UserData)method.Invoke(vm, new object[] { request.FileName, parameters });
            }
            else
            {
                data = (UserData)method.Invoke(vm, new object[] { request.FileName });
            }
            if (data == null)
            {
                var message = "UserFileService: No Data returned: " + request.FileName;
                Trace.TraceWarning(message);
                return new HttpResult(message, HttpStatusCode.NotFound);
            }

            var httpResult = new HttpResult(data.Bytes, data.ContentType);
            if (!appSession.CookieSet)
            {
                httpResult.Headers.Add("Set-Cookie", "stonehenge_id=" + appSession.Id);
            }
            return httpResult;
        }

        
        public object Any(UserFile request)
        {
            var message = "UserFileService: No handler for HTTP method: " + Request.HttpMethod;
            Trace.TraceWarning(message);
            return new HttpResult(HttpStatusCode.NotFound, message);
        }
    }

}
