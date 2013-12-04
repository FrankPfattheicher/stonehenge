using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using IctBaden.Stonehenge.Annotations;
using ServiceStack.CacheAccess;

namespace IctBaden.Stonehenge
{
  public class AppSession : INotifyPropertyChanged
  {
    internal ISession Session { get; set; }
    public string HostDomain { get; set; }
    public string UserAgent { get; set; }
    public DateTime ConnectedSince { get; private set; }
    public DateTime LastAccess { get; private set; }
    public long Id { get; set; }
    private static long nextId;

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

    private readonly Dictionary<string, object> userData;
    public object this[string key]
    {
      get
      {
        return userData.ContainsKey(key) ? userData[key] : null;
      }
      set
      {
        userData[key] = value;
        NotifyPropertyChanged(key);
      }
    }

    public TimeSpan ConnectedDuration
    {
      get { return DateTime.Now - ConnectedSince; }
    }
    public TimeSpan LastAccessDuration
    {
      get { return DateTime.Now - LastAccess; }
    }

    public event Action TimedOut;
    private Timer pollSessionTimeout;
    public TimeSpan SessionTimeout { get; private set; }

    public void SetTimeout(TimeSpan timeout)
    {
      if (pollSessionTimeout != null)
      {
        pollSessionTimeout.Dispose();
      }
      SessionTimeout = timeout;
      if (Math.Abs(timeout.TotalMilliseconds) > 0.1)
      {
        pollSessionTimeout = new Timer(CheckSessionTimeout, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
      }
    }

    private void CheckSessionTimeout(object _)
    {
      if ((LastAccessDuration > SessionTimeout) && (terminator != null))
      {
        pollSessionTimeout.Dispose();
        terminator.Dispose();
        if (TimedOut != null)
        {
          TimedOut();
        }
      }
      NotifyPropertyChanged("ConnectedDuration");
      NotifyPropertyChanged("LastAccessDuration");
    }

    private IDisposable terminator;

    public void SetTerminator(IDisposable disposable)
    {
      terminator = disposable;
    }

    internal AppSession(string hostDomain, string hostUrl, string userAgent, ISession session)
    {
      if (!string.IsNullOrEmpty(hostUrl))
      {
        var uri = new UriBuilder(hostUrl);
        HostDomain = string.IsNullOrEmpty(hostDomain) ? uri.Host : hostDomain;
      }
      else
      {
        HostDomain = hostDomain;
      }
      userData = new Dictionary<string, object>();
      UserAgent = userAgent;
      Session = session;
      ConnectedSince = DateTime.Now;
      Id = ++nextId;
    }

    internal void Accessed()
    {
      LastAccess = DateTime.Now;
      NotifyPropertyChanged("LastAccess");
    }

    public override string ToString()
    {
      return string.Format("[{0}] {1} {2}", Id, ConnectedSince.ToShortDateString() + " " + ConnectedSince.ToShortTimeString(), SubDomain);
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

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void NotifyPropertyChanged(string propertyName)
    {
      var handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}