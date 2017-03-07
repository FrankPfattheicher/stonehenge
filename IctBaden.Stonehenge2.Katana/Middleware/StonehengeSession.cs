using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using IctBaden.Stonehenge2.Core;

namespace IctBaden.Stonehenge2.Katana.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Owin;

    public class StonehengeSession
    {
        private readonly Func<IDictionary<string, object>, Task> _next;

        public StonehengeSession(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);
            var canceled = context.Request.CallCancelled;

            var timer = new Stopwatch();
            timer.Start();

            var path = context.Request.Path;
            //var stonehengeId = context.Request.Cookies["stonehenge-id"] ?? context.Request.Query["stonehenge-id"];
            var cookie = context.Request.Headers.FirstOrDefault(h => h.Key == "Cookie");
            var stonehengeId = context.Request.Query["stonehenge-id"];
            if ((cookie.Value != null) && (cookie.Value.Length > 0))
            {
                // workaround for double stonehenge-id values in cookie - take the last one
                var match = new Regex("stonehenge-id=([a-f0-9A-F]+)", RegexOptions.RightToLeft)
                    .Match(cookie.Value[cookie.Value.Length - 1]);
                if (match.Success)
                {
                    stonehengeId = match.Groups[1].Value;
                }
            }
            Trace.TraceInformation($"Stonehenge2.Katana[{stonehengeId}] Begin {context.Request.Method} {path}");

            var appSessions = context.Environment["stonehenge.AppSessions"] as List<AppSession>;
            CleanupTimedOutSessions(appSessions);
            var session = appSessions?.FirstOrDefault(s => s.Id == stonehengeId);
            if (string.IsNullOrEmpty(stonehengeId) || session == null)
            {
                // session not found - redirect to new session
                session = NewSession(appSessions, context.Request);
                context.Response.Headers.Add("Set-Cookie", new[] { "stonehenge-id=" + session.Id });
                context.Response.Redirect("/Index.html?stonehenge-id=" + session.Id);
                Trace.TraceInformation($"Stonehenge2.Katana[{stonehengeId}] From IP {context.Request.RemoteIpAddress}:{context.Request.RemotePort} - redirect to {session.Id}");
                return;
            }

            var etag = context.Request.Headers["If-None-Match"];
            if (context.Request.Method == "GET" && etag == session.AppInstanceId)
            {
                Debug.WriteLine("ETag match.");
                context.Response.StatusCode = (int)HttpStatusCode.NotModified;
            }
            else
            {
                context.Environment.Add("stonehenge.AppSession", session);
                await _next.Invoke(environment);
            }

            timer.Stop();

            if (canceled.IsCancellationRequested)
            {
                Trace.TraceWarning(
                    $"Stonehenge2.Katana[{stonehengeId}] Canceled {context.Request.Method}={context.Response.StatusCode} {path}, {timer.ElapsedMilliseconds}ms");
                throw new TaskCanceledException();
            }

            Trace.TraceInformation(
                $"Stonehenge2.Katana[{stonehengeId}] End {context.Request.Method}={context.Response.StatusCode} {path}, {timer.ElapsedMilliseconds}ms");
        }

        private void CleanupTimedOutSessions(List<AppSession> appSessions)
        {
            var timedOutSessions = appSessions.Where(s => s.IsTimedOut).ToArray();
            foreach (var timedOut in timedOutSessions)
            {
                var vm = timedOut.ViewModel as IDisposable;
                vm?.Dispose();
                timedOut.ViewModel = null;
                appSessions.Remove(timedOut);
                Trace.TraceInformation($"Stonehenge2.Katana Session timed out {timedOut.Id}.");
            }
            Trace.TraceInformation($"Stonehenge2.Katana {appSessions.Count} sessions.");
        }

        private AppSession NewSession(List<AppSession> appSessions, IOwinRequest request)
        {
            var userAgent = request.Headers["User-Agent"];
            var session = new AppSession();
            session.Initialize(request.Host.Value, request.RemoteIpAddress, userAgent);
            appSessions.Add(session);
            Trace.TraceInformation($"Stonehenge2.Katana New session {session.Id}. {appSessions.Count} sessions.");
            return session;
        }
    }
}