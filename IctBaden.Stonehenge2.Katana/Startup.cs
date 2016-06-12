using System;
using System.Net;
using Microsoft.Owin;
using Microsoft.Owin.Diagnostics;
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
#if DEBUG
            var errorOptions = new ErrorPageOptions
            {
                ShowExceptionDetails = true,
                ShowSourceCode = true
            };
            app.UseErrorPage(errorOptions);
#endif
            app.Use<StonehengeRoot>();
            if ((Environment.OSVersion.Platform == PlatformID.Win32NT)
                || (Environment.OSVersion.Platform == PlatformID.Win32Windows))
            {
                // SqueezeMe ist not compatible wit Mono
                app.UseCompression();
            }
            app.Use(async (context, next) =>
            {
                var timer = new Stopwatch();
                timer.Start();

                var path = context.Request.Path;
                Trace.TraceInformation($"Stonehenge2.Katana Begin {context.Request.Method} {path}");

                var stonehengeId = context.Request.Cookies["stonehenge-id"] ?? context.Request.Query["stonehenge-id"];
                var session = sessions.FirstOrDefault(s => s.Id == stonehengeId); 
                if (string.IsNullOrEmpty(stonehengeId) || session == null)
                {
                    // session not found - redirect to new session
                    session = NewSession(context.Request);
                    context.Response.Headers.Add("Set-Cookie", new[] { "stonehenge-id=" + session.Id });
                    context.Response.Redirect("/Index.html?stonehenge-id=" + session.Id);
                    return;
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
                Trace.TraceInformation("Stonehenge2.Katana End {0}={1} {2}, {3}ms", 
                    context.Request.Method, context.Response.StatusCode, path, timer.ElapsedMilliseconds);
            });

            app.Use<StonehengeContent>();
        }

        private AppSession NewSession(IOwinRequest request)
        {
            var userAgent = request.Headers["User-Agent"];
            var session = new AppSession();
            session.Initialize(request.Host.Value, request.RemoteIpAddress, userAgent);
            sessions.Add(session);
            Trace.TraceInformation($"Stonehenge2.Katana New session {session.Id}");

            var timedOut = sessions.Where(s => s.IsTimedOut).ToList();
            foreach (var sess in timedOut)
            {
                sessions.Remove(sess);
                Trace.TraceInformation($"Stonehenge2.Katana Session timed out {sess.Id}");
            }
            return session;
        }
    }
}