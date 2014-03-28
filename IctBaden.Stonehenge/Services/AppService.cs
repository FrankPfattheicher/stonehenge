using System;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;

namespace IctBaden.Stonehenge.Services
{
    public class AppService : Service
    {
        public const string PropertyNameId = "_stonehenge_prop_";
        public const string PropertyNameMessageBox = PropertyNameId + "MessageBox_";
        public const string PropertyNameNavigate = PropertyNameId + "Navigate_";
        public const string PropertyNameClientScript = PropertyNameId + "ClientScript_";

        public string GetSessionId()
        {
            var sessionId = Request.QueryString.Get("stonehenge_id");
            if (!string.IsNullOrEmpty(sessionId))
                return sessionId;
            if (Request.Cookies.ContainsKey("stonehenge_id"))
            {
                sessionId = Request.Cookies["stonehenge_id"].Value;
            }
            return sessionId;
        }
        public AppSession GetSession (string id)
		{
			if (string.IsNullOrEmpty (id)) 
			{
				return null;
			}
            lock (AppSessionCache.Cache)
            {
                Guid sessionId;
                Guid.TryParse(id, out sessionId);
                var session = AppSessionCache.GetSession(sessionId);
                if (session == null)
                {
                    return null;
                }
                lock (this)
                {
                    var host = GetResolver() as AppHost;

                    if (!session.IsInitialized)
                    {
                        session.Initialize(Request.QueryString.Get("hostdomain"), Request.AbsoluteUri, Request.RemoteIp, Request.UserAgent);
                        if (host != null)
                        {
                            host.OnSessionCreated(session);
                        }
                    }

                    if (host == null)
                        return session;

                    if (!host.HasSessionTimeout)
                        return session;

                    session.SetTerminator(this);
                    session.SetTimeout(host.SessionTimeout);
                    session.TimedOut += () =>
                    {
                        Cache.FlushAll();
                        AppSessionCache.RemoveSession(session.Id);
                        host.OnSessionTerminated(session);
                    };
                }
                return session;
            }
        }

        protected object RedirectToNewSession()
        {
            var uri = Request.AbsoluteUri;
            var getsid = new Regex(@".*\?stonehenge_id\=(?<sid>[^/&]+)");
            var match = getsid.Match(uri);

            if (match.Success)
            {
                var sid = match.Groups["sid"].Value;
                Trace.TraceInformation("Invalid Session {0}", sid);
            }

            var ix = uri.IndexOf("?stonehenge_id=", StringComparison.InvariantCulture);
            if (ix != -1)
            {
                uri = uri.Substring(0, ix);
            }

            var session = AppSessionCache.NewSession();
            Trace.TraceInformation("Redirect to new session {0}", session.Id);

            //uri = request.FileName + "?stonehenge_id=" + session.Id;
            uri += "?stonehenge_id=" + session.Id;

            var redirect = new HttpResult { StatusCode = HttpStatusCode.SeeOther };
            redirect.Headers.Add("Location", uri);
            return redirect;
        }

    }
}