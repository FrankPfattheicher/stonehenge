using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IctBaden.Stonehenge
{
  public class AppSessionCache
  {
    public static readonly Dictionary<Guid, AppSession> Cache = new Dictionary<Guid, AppSession>();

    public static AppSession NewSession()
    {
      lock (Cache)
      {
        var session = new AppSession();
        Cache.Add(session.Id, session);
        Trace.TraceInformation("New AppSession created: {0}, {1} sessions", session.Id, Cache.Count);
        return session;
      }
    }

    public static AppSession GetSession(Guid id)
    {
      lock (Cache)
      {
        return Cache.ContainsKey(id) ? Cache[id] : null;
      }
    }

    public static void RemoveSession(Guid id)
    {
      lock (Cache)
      {
        if (Cache.ContainsKey(id))
        {
          Trace.TraceInformation("Remove AppSession: {0}, {1} sessions", id, Cache.Count - 1);
          Cache.Remove(id);
        }
      }
    }
  }
}