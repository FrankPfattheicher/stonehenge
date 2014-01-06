using System;
using ServiceStack.ServiceInterface;

namespace IctBaden.Stonehenge.Services
{
  public class AppService : Service
  {
    public const string PropertyNameId = "_stonehenge_prop_";
    public const string PropertyNameMessageBox = PropertyNameId + "MessageBox_";
    public const string PropertyNameNavigate = PropertyNameId + "Navigate_";
    
    public AppSession GetSession(string id, bool allowNew)
    {
      Guid sessionId;
      Guid.TryParse(id, out sessionId);
      var session = AppSessionCache.GetSession(sessionId);
      if (session == null)
      {
        if (!allowNew)
          return null;
        session = AppSessionCache.NewSession();
      }
      lock (session)
      {
        if (!session.IsInitialized)
        {
          session.Initialize(Request.QueryString.Get("hostdomain"), Request.AbsoluteUri, Request.RemoteIp, Request.UserAgent);
        }

        var host = GetResolver() as AppHost;
        if (host == null) 
          return session;

        host.OnSessionCreated(session);
        if (!host.HasSessionTimeout) 
          return session;

        session.SetTerminator(this);
        session.SetTimeout(host.SessionTimeout);
        session.TimedOut += () =>
        {
          Cache.FlushAll();
          host.OnSessionTerminated(session);
        };
      }
      return session;
    }

  }
}