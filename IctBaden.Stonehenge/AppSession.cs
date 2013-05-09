using System;
using ServiceStack.CacheAccess;

namespace IctBaden.Stonehenge
{
  public class AppSession
  {
    internal ISession Session { get; set; }
    public string HostUrl { get; set; }
    public DateTime ConnectedSince { get; set; }

    public string SubDomain
    {
      get
      {
        if (string.IsNullOrEmpty(HostUrl))
          return string.Empty;

        var parts = HostUrl.Split(new[] { '.' });
        return parts[0];
      }
    }

    internal AppSession(string hostUrl, ISession session)
    {
      var uri = new UriBuilder(hostUrl);
      HostUrl = uri.Host;
      Session = session;
      ConnectedSince = DateTime.Now;
    }

    public override string ToString()
    {
      return SubDomain;
    }

    public void Set<T>(string key, T value)
    {
      if (Session == null)
        return;
      Session.Set(key, value);
    }

    public T Get<T>(string key)
    {
      if (Session == null)
        return default(T);
      return Session.Get<T>(key);
    }

  }
}