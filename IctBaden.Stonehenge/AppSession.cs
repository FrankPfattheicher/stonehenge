using System;
using ServiceStack.CacheAccess;

namespace IctBaden.Stonehenge
{
  public class AppSession
  {
    internal ISession Session { get; set; }
    public string HostDomain { get; set; }
    public string UserAgent { get; set; }
    public DateTime ConnectedSince { get; set; }

    public string SubDomain
    {
      get
      {
        if (string.IsNullOrEmpty(HostDomain))
          return string.Empty;

        var parts = HostDomain.Split(new[] { '.' });
        int val;
        var isNumeric = int.TryParse(parts[0], out val);
        return isNumeric ? HostDomain : parts[0];
      }
    }

    internal AppSession(string hostDomain, string hostUrl, string userAgent, ISession session)
    {
      var uri = new UriBuilder(hostUrl);
      HostDomain = string.IsNullOrEmpty(hostDomain) ? uri.Host : hostDomain;
      UserAgent = userAgent;
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