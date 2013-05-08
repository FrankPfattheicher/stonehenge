using System.Collections.Generic;
using ServiceStack.CacheAccess;

namespace IctBaden.Stonehenge
{
  public class AppSession
  {
    internal ISession Session { get; set; }

    internal AppSession(ISession session)
    {
      Session = session;
    }

    public void Set<T>(string key, T value)
    {
      Session.Set<T>(key, value);
    }

    public T Get<T>(string key)
    {
      return Session.Get<T>(key);
    }

  }
}