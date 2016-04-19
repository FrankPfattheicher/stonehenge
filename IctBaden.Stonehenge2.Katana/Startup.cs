using System.Net;
using SqueezeMe;

namespace IctBaden.Stonehenge2.Katana
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Core;
    using Middleware;
    using Resources;

    using Owin;

    internal class Startup
    {
        private readonly string appTitle;
        private readonly IStonehengeResourceProvider resourceLoader;

        private readonly List<AppSession> sessions = new List<AppSession>();

        public Startup(string title, IStonehengeResourceProvider loader)
        {
            appTitle = title;
            resourceLoader = loader;
        }

        public void Configuration(IAppBuilder app)
        {
            app.UseCompression();
#if DEBUG
            app.UseErrorPage();
#endif
            app.Use(async (context, next) =>
            {
                var timer = new Stopwatch();
                timer.Start();

                var path = context.Request.Path;
                Trace.TraceInformation("Stonehenge2.Katana Begin request {0} {1}", context.Request.Method, path);

                var ssPid = context.Request.Cookies["ss-pid"];
                var session = sessions.FirstOrDefault(s => s.PermanentSessionId == ssPid);
                if (session == null)
                {
                    var userAgent = context.Request.Headers["User-Agent"];
                    session = new AppSession();
                    session.Initialize(ssPid, context.Request.Host.Value, context.Request.Host.Value, context.Request.RemoteIpAddress.ToString(), userAgent);
                    sessions.Add(session);
                }

                var etag = context.Request.Headers["If-None-Match"];
                if (context.Request.Method == "GET" && etag == session.AppVersionId)
                {
                    Debug.WriteLine("ETag match.");
                    context.Response.StatusCode = (int)HttpStatusCode.NotModified;
                }
                else
                {
                    context.Environment.Add("stonehenge.AppTitle", appTitle);
                    context.Environment.Add("stonehenge.ResourceLoader", resourceLoader);
                    context.Environment.Add("stonehenge.AppSession", session);
                    await next.Invoke();
                }

                timer.Stop();
                Trace.TraceInformation("Stonehenge2.Katana End request {0} {1}, {2}ms", 
                    context.Request.Method, path, timer.ElapsedMilliseconds);
            });

            app.Use<StonehengeContent>();
            app.Use<StonehengeRoot>();
        }
    }
}