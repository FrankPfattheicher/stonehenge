﻿namespace IctBaden.Stonehenge2.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using System.Web;
    using System.Web.Configuration;

    using IctBaden.Stonehenge2.ViewModel;

    public class AppSession : INotifyPropertyChanged
    {
        public string HostDomain { get; private set; }
        public string ClientAddress { get; private set; }
        public string UserAgent { get; private set; }
        public string Platform { get; private set; }
        public string Browser { get; private set; }
        public bool Cookies { get; private set; }
        public bool CookieSet { get; private set; }

        public DateTime ConnectedSince { get; private set; }
        public DateTime LastAccess { get; private set; }
        public string Context { get; private set; }
        public DateTime LastUserAction { get; private set; }
        public Guid Id { get; private set; }
        public string PermanentSessionId { get; private set; }

        internal List<string> Events = new List<string>();
        internal AutoResetEvent EventRelease = new AutoResetEvent(false);
        internal readonly PassiveTimer EventPollingActive = new PassiveTimer();

        public bool IsWaitingForEvents
        {
            get
            {
                return EventPollingActive.Running && !EventPollingActive.Timeout;
            }
        }

        private object viewModel;
        public object ViewModel
        {
            get { return viewModel; }
            set
            {
                (viewModel as IDisposable)?.Dispose();

                viewModel = value;
                var npc = value as INotifyPropertyChanged;
                if (npc != null)
                {
                    npc.PropertyChanged += (sender, args) =>
                    {
                        var avm = sender as ActiveViewModel;
                        if (avm == null)
                            return;
                        lock (avm.Session.Events)
                        {
                            avm.Session.EventAdd(args.PropertyName);
                        }
                    };
                }
            }
        }

        public void ClientAddressChanged(string address)
        {
            ClientAddress = address;
            NotifyPropertyChanged("ClientAddress");
        }

        public object SetViewModelType(string typeName)
        {
            var vm = ViewModel;
            if (ViewModel != null)
            {
                if ((ViewModel.GetType().FullName == typeName))
                    return vm;

                var disposable = vm as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }


            var asm = Assembly.GetEntryAssembly();
            var vmtype = asm.GetTypes().FirstOrDefault(type => type.FullName.EndsWith(typeName));
            if (vmtype == null)
            {
                ViewModel = null;
                Debug.WriteLine("Could not create ViewModel:" + typeName);
                return null;
            }

            try
            {
                if (typeof(ActiveViewModel).IsAssignableFrom(vmtype))
                {
                    var sessionCtor = vmtype.GetConstructors().FirstOrDefault(ctor => ctor.GetParameters().Length == 1);
                    vm = (sessionCtor != null) ? Activator.CreateInstance(vmtype, this) : Activator.CreateInstance(vmtype);
                }
                else
                {
                    vm = Activator.CreateInstance(vmtype);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                vm = null;
            }

            ViewModel = vm;
            return vm;
        }

        public string SubDomain
        {
            get
            {
                if (string.IsNullOrEmpty(HostDomain))
                    return string.Empty;

                var parts = HostDomain.Split('.');
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
                if (this[key] == value)
                    return;
                userData[key] = value;
                NotifyPropertyChanged(key);
            }
        }

        public void Set<T>(string key, T value)
        {
            userData[key] = value;
        }
        public T Get<T>(string key)
        {
            if (!userData.ContainsKey(key))
                return default(T);

            return (T)userData[key];
        }
        public void Remove(string key)
        {
            userData.Remove(key);
        }

        public TimeSpan ConnectedDuration
        {
            get { return DateTime.Now - ConnectedSince; }
        }
        public TimeSpan LastAccessDuration
        {
            get { return DateTime.Now - LastAccess; }
        }
        public TimeSpan LastUserActionDuration
        {
            get { return DateTime.Now - LastUserAction; }
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
                pollSessionTimeout = new Timer(CheckSessionTimeout, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
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

        public AppSession()
        {
            userData = new Dictionary<string, object>();
            Id = Guid.NewGuid();
        }

        public bool IsInitialized { get { return UserAgent != null; } }

        public void Initialize(string ssPid, string hostDomain, string hostUrl, string clientAddress, string userAgent)
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

            PermanentSessionId = ssPid;
            ClientAddress = clientAddress;
            UserAgent = userAgent;
            ConnectedSince = DateTime.Now;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                DetectBrowser(userAgent);
            }
        }

        private void DetectBrowser(string userAgent)
        {
            var browser = new HttpBrowserCapabilities
            {
                Capabilities = new Hashtable {{string.Empty, userAgent}}
            };
            var factory = new BrowserCapabilitiesFactory();
            factory.ConfigureBrowserCapabilities(new NameValueCollection(), browser);

            Browser = browser.Browser + " " + browser.Version;
            Cookies = browser.Cookies;
            Platform = browser.Platform;
        }

        internal void Accessed(IDictionary<string, Cookie> cookies, bool userAction)
        {
            if ((PermanentSessionId == null) && cookies.ContainsKey("ss-pid"))
            {
                PermanentSessionId = cookies["ss-pid"].Value;
            }
            LastAccess = DateTime.Now;
            NotifyPropertyChanged("LastAccess");
            if (userAction)
            {
                LastUserAction = DateTime.Now;
                NotifyPropertyChanged("LastUserAction");
            }
            CookieSet = cookies.ContainsKey("stonehenge_id");
            NotifyPropertyChanged("CookieSet");
        }

        public void SetContext(string context)
        {
            Context = context;
        }

        public void EventsClear(bool forceEnd)
        {
            lock (Events)
            {
                //var privateEvents = Events.Where(e => e.StartsWith(AppService.PropertyNameId)).ToList();
                Events.Clear();
                //Events.AddRange(privateEvents);
                if (forceEnd)
                {
                    EventRelease.Set();
                    EventRelease.Set();
                }
            }
        }

        public void EventAdd(string name)
        {
            lock (Events)
            {
                if (Events.Contains(name))
                    return;
                Events.Add(name);
                EventRelease.Set();
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1} {2}", Id, ConnectedSince.ToShortDateString() + " " + ConnectedSince.ToShortTimeString(), SubDomain);
        }

        public event PropertyChangedEventHandler PropertyChanged;


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