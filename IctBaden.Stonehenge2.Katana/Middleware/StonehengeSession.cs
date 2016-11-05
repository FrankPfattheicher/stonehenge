using System.Diagnostics;
using System.Linq;
using System.Net;
using IctBaden.Stonehenge2.Core;
using IctBaden.Stonehenge2.Resources;

namespace IctBaden.Stonehenge2.Katana.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Owin;

    public class StonehengeSession
    {
        private readonly Func<IDictionary<string, object>, Task> _next;
        private static readonly List<AppSession> Sessions = new List<AppSession>();
        public static string AppTitle;
        public static IStonehengeResourceProvider ResourceLoader;

        public StonehengeSession(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);

            var timer = new Stopwatch();
            timer.Start();

            var path = context.Request.Path;
            var stonehengeId = context.Request.Cookies["stonehenge-id"] ?? context.Request.Query["stonehenge-id"];
            Trace.TraceInformation($"Stonehenge2.Katana[{stonehengeId}] Begin {context.Request.Method} {path}");

            CleanupTimedOutSessions();
            var session = Sessions.FirstOrDefault(s => s.Id == stonehengeId);
            if (string.IsNullOrEmpty(stonehengeId) || session == null)
            {
                // session not found - redirect to new session
                session = NewSession(context.Request);
                context.Response.Headers.Add("Set-Cookie", new[] { "stonehenge-id=" + session.Id });
                context.Response.Redirect("/Index.html?stonehenge-id=" + session.Id);
                Trace.TraceInformation($"Stonehenge2.Katana[{stonehengeId}] Redirect to {session.Id}");
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
                context.Environment.Add("stonehenge.AppTitle", AppTitle);
                context.Environment.Add("stonehenge.ResourceLoader", ResourceLoader);
                context.Environment.Add("stonehenge.AppSession", session);
                await _next.Invoke(environment);
            }

            timer.Stop();
            Trace.TraceInformation(
                $"Stonehenge2.Katana[{stonehengeId}] End {context.Request.Method}={context.Response.StatusCode} {path}, {timer.ElapsedMilliseconds}ms");
        }

        private void CleanupTimedOutSessions()
        {
            foreach (var timedOut in Sessions.Where(s => s.IsTimedOut))
            {
                timedOut.ViewModel = null;
                Sessions.Remove(timedOut);
                Trace.TraceInformation($"Stonehenge2.Katana Session timed out {timedOut.Id}. {Sessions.Count} sessions.");
            }
        }

        private AppSession NewSession(IOwinRequest request)
        {
            var userAgent = request.Headers["User-Agent"];
            var session = new AppSession();
            session.Initialize(request.Host.Value, request.RemoteIpAddress, userAgent);
            Sessions.Add(session);
            Trace.TraceInformation($"Stonehenge2.Katana New session {session.Id}. {Sessions.Count} sessions.");
            return session;
        }
    }
}