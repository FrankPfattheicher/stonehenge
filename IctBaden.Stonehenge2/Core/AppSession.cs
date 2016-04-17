using System.Text.RegularExpressions;

namespace IctBaden.Stonehenge2.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading;

    using ViewModel;

    public class AppSession : INotifyPropertyChanged
    {
        public string AppVersionId { get; private set; }

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

        private readonly Guid id;
        public string Id => id.ToString("N");

        public string PermanentSessionId { get; private set; }

        private const int EventTimeoutSeconds = 30;
        private readonly List<string> events = new List<string>();
        private readonly AutoResetEvent eventRelease = new AutoResetEvent(false);

        public bool IsWaitingForEvents { get; private set; }
        public List<string> CollectEvents()
        {
            IsWaitingForEvents = true;
            eventRelease.WaitOne(TimeSpan.FromSeconds(EventTimeoutSeconds));
            // wait for maximum 500ms for more events - if there is none within 100ms - continue
            var max = 5;
            while (eventRelease.WaitOne(100) && (max > 0))
            {
                max--;
            }
            IsWaitingForEvents = false;
            return events;
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
                        lock (avm.Session.events)
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
                disposable?.Dispose();
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

        public TimeSpan ConnectedDuration => DateTime.Now - ConnectedSince;

        public TimeSpan LastAccessDuration => DateTime.Now - LastAccess;

        public TimeSpan LastUserActionDuration => DateTime.Now - LastUserAction;

        public event Action TimedOut;
        private Timer pollSessionTimeout;
        public TimeSpan SessionTimeout { get; private set; }

        public void SetTimeout(TimeSpan timeout)
        {
            pollSessionTimeout?.Dispose();
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
                TimedOut?.Invoke();
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
            id = Guid.NewGuid();

            AppVersionId = Assembly.GetEntryAssembly()?.ManifestModule.ModuleVersionId.ToString("N") ?? Guid.NewGuid().ToString("N");
        }

        public bool IsInitialized => UserAgent != null;

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
            // Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.87 Safari/537.36
            var decoder = new Regex(@"\w+/[\d.]+ \(");
            //TODO: Decocder
            Browser = "";
            Cookies = true;
            Platform = "OS";
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
            lock (events)
            {
                //var privateEvents = Events.Where(e => e.StartsWith(AppService.PropertyNameId)).ToList();
                events.Clear();
                //Events.AddRange(privateEvents);
                if (forceEnd)
                {
                    eventRelease.Set();
                    eventRelease.Set();
                }
            }
        }

        public void EventAdd(string name)
        {
            lock (events)
            {
                if (!events.Contains(name))
                    events.Add(name);
                eventRelease.Set();
            }
        }

        public override string ToString()
        {
            // ReSharper disable once UseStringInterpolation
            return string.Format("[{0}] {1} {2}", Id, ConnectedSince.ToShortDateString() + " " + ConnectedSince.ToShortTimeString(), SubDomain);
        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}