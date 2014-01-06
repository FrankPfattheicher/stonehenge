using System;
using System.Collections.Generic;
using System.Web.Caching;

namespace IctBaden.Stonehenge
{
  public class AppSessionCache
  {
    private static Dictionary<Guid, AppSession> Cache = new Dictionary<Guid, AppSession>();

    public static AppSession NewSession()
    {
      var session = new AppSession();
      lock (Cache)
      {
        Cache.Add(session.Id, session);
      }
      return session;
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
          Cache.Remove(id);
        }
      }
    }
  }
}