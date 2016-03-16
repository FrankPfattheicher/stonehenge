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
        private readonly IStonehengeResourceProvider resourceLoader;

        private readonly List<AppSession> sessions = new List<AppSession>();

        public Startup(IStonehengeResourceProvider loader)
        {
            resourceLoader = loader;
        }

        public void Configuration(IAppBuilder app)
        {
#if DEBUG
            app.UseErrorPage();
#endif
            app.Use(async (context, next) =>
            {
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

                context.Environment.Add("stonehenge.ResourceLoader", resourceLoader);
                context.Environment.Add("stonehenge.AppSession", session);
                await next.Invoke();

                Trace.TraceInformation("Stonehenge2.Katana End request {0} {1}", context.Request.Method, path);
            });

            app.Use<StonehengeContent>();
            app.Use<StonehengeRoot>();
        } 
    }
}