using System.Diagnostics;
using ServiceStack.Common.Web;

namespace IctBaden.Stonehenge.Services
{
    using System;
    using System.Linq;

    public class AppExceptionService : AppService
    {
        public object Post(AppException exception)
        {
            var message = exception.Cause;
            if (exception.Cause == "Binding")
            {
                var param = Request.FormData.GetValues("Binding");
                var binding = (param != null) ? param.FirstOrDefault() : string.Empty;
                Trace.TraceError("Client binding exception - Undefined property name: " + binding);
                message += "(" + binding + ")";
            }
            else
            {
                Trace.TraceError("Client site exception: " + exception.Cause);
            }
            var host = GetResolver() as AppHost;
            if (host != null)
            {
                host.OnClientException(new Exception(message));
            }

            return new HttpResult("{}", "application/json");
        }
    }
}