using System.Diagnostics;
using System.IO;
using System.Net;

namespace IctBaden.Stonehenge2.Katana.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Owin;

    public class ServerExceptionLogger
    {
        private readonly Func<IDictionary<string, object>, Task> _next;

        public ServerExceptionLogger(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);
            try
            {
                await _next.Invoke(environment);
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException != null) message += "; " + ex.InnerException.Message;
                Trace.TraceError($"ServerExceptionHandler: {ex.GetType().Name}(HR=0x{ex.HResult:X8}): {message}" + Environment.NewLine + 
                                 $"ServerExceptionHandler: StackTrace: {ex.StackTrace}");
                return;
            }
            if (context.Response.StatusCode == (int) HttpStatusCode.InternalServerError)
            {
                using (var reader = new StreamReader(context.Response.Body))
                {
                    var message = reader.ReadToEnd();
                    Trace.TraceError($"ServerExceptionHandler: Message: {message}");
                }
            }
        }
    }
}