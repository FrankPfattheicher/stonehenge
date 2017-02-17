using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace IctBaden.Stonehenge2.Katana.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Owin;

    public class StonehengeAcme
    {
        private readonly Func<IDictionary<string, object>, Task> _next;

        public StonehengeAcme(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);

            if (context.Request.Path.Value.StartsWith("/.well-known"))
            {
                Trace.TraceInformation("Handling ACME request: " + context.Request.Path.Value);
                var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? ".";
                var acmeFile = path + context.Request.Path.Value;
                if (File.Exists(acmeFile))
                {
                    var acmeData = File.ReadAllBytes(acmeFile);

                    var response = context.Get<Stream>("owin.ResponseBody");
                    using (var writer = new BinaryWriter(response))
                    {
                        writer.Write(acmeData);
                    }

                    return;
                }

                Trace.TraceError("No ACME data found.");
            }

            await _next.Invoke(environment);
        }
    }
}