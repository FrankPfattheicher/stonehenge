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
            var sessionId = GetSessionId();
            var appSession = GetSession(sessionId);
            if (appSession != null)
            {
                var vm = appSession.ViewModel;
                if (vm != null)
                {
                    var method = vm.GetType()
                      .GetMethods()
                      .FirstOrDefault(m => string.Compare(m.Name, "GetUserData", StringComparison.InvariantCultureIgnoreCase) == 0);
                    if (method != null)
                    {
                        if (method.ReturnType == typeof(UserData))
                        {
                            UserData data = null;
                            if (method.GetParameters().Count() == 2)
                            {
                                var parameters = Request.QueryString.AllKeys
                                    .ToDictionary(n => n, n => Request.QueryString[n]);
                                    data = (UserData)method.Invoke(vm, new object[] { request.FileName, parameters });
                            }
                            else
                            {
                                data = (UserData)method.Invoke(vm, new object[] { request.FileName });
                            }
                            if (data != null)
                            {
                                var httpResult = new HttpResult(data.Bytes, data.ContentType);
                                if (!appSession.CookieSet)
                                {
                                    httpResult.Headers.Add("Set-Cookie", "stonehenge_id=" + appSession.Id);
                                }
                                return httpResult;
                            }
                        }
                    }
                }
            }
            return new HttpResult("User file not found: " + request.FileName, HttpStatusCode.NotFound);
        }
    }
}