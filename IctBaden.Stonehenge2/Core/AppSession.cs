using System.Text.RegularExpressions;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace IctBaden.Stonehenge2.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using ViewModel;

    public class AppSession : INotifyPropertyChanged
    {
        public string AppInstanceId { get; private set; }

        public string HostDomain { get; private set; }
        public string ClientAddress { get; private set; }
        public string UserAgent { get; private set; }
        public string Platform { get; private set; }
        public string Browser { get; private set; }

        public bool CookiesSupported { get; private set; }
        public bool StonehengeCookieSet { get; private set; }
        public Dictionary<string, string> Cookies { get; private set; }

        public DateTime ConnectedSince { get; private set; }
        public DateTime LastAccess { get; private set; }
        public string Context { get; private set; }
        public DateTime LastUserAction { get; private set; }

        private readonly Guid _id;
        public string Id => _id.ToString("N");

        public string PermanentSessionId { get; private set; }

        private const int EventTimeoutSeconds = 30;
        private readonly List<string> _events = new List<string>();
        private readonly AutoResetEvent _eventRelease = new AutoResetEvent(false);

        public bool IsWaitingForEvents { get; private set; }
        public List<string> CollectEvents()
        {
            IsWaitingForEvents = true;
            _eventRelease.WaitOne(TimeSpan.FromSeconds(EventTimeoutSeconds));
            // wait for maximum 500ms for more events - if there is none within 100ms - continue
            var max = 5;
            while (_eventRelease.WaitOne(100) && (max > 0))
            {
                max--;
            }
            IsWaitingForEvents = false;
            return _events;
        }

        private object _viewModel;
        public object ViewModel
        {
            get { return _viewModel; }
            set
            {
                (_viewModel as IDisposable)?.Dispose();

                _viewModel = value;
                var npc = value as INotifyPropertyChanged;
                if (npc != null)
                {
                    npc.PropertyChanged += (sender, args) =>
                    {
                        var avm = sender as ActiveViewModel;
                        if (avm == null)
                            return;
                        lock (avm.Session._events)
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

        private readonly Dictionary<string, object> _userData;
        public object this[string key]
        {
            get
            {
                return _userData.ContainsKey(key) ? _userData[key] : null;
            }
            set
            {
                if (this[key] == value)
                    return;
                _userData[key] = value;
                NotifyPropertyChanged(key);
            }
        }

        public void Set<T>(string key, T value)
        {
            _userData[key] = value;
        }
        public T Get<T>(string key)
        {
            if (!_userData.ContainsKey(key))
                return default(T);

            return (T)_userData[key];
        }
        public void Remove(string key)
        {
            _userData.Remove(key);
        }

        public TimeSpan ConnectedDuration => DateTime.Now - ConnectedSince;

        public TimeSpan LastAccessDuration => DateTime.Now - LastAccess;

        public TimeSpan LastUserActionDuration => DateTime.Now - LastUserAction;

        public event Action TimedOut;
        private Timer _pollSessionTimeout;
        public TimeSpan SessionTimeout { get; private set; }
        public bool IsTimedOut => LastAccessDuration > SessionTimeout;

        public void SetTimeout(TimeSpan timeout)
        {
            _pollSessionTimeout?.Dispose();
            SessionTimeout = timeout;
            if (Math.Abs(timeout.TotalMilliseconds) > 0.1)
            {
                _pollSessionTimeout = new Timer(CheckSessionTimeout, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
            }
        }

        private void CheckSessionTimeout(object _)
        {
            if ((LastAccessDuration > SessionTimeout) && (_terminator != null))
            {
                _pollSessionTimeout.Dispose();
                _terminator.Dispose();
                TimedOut?.Invoke();
            }
            NotifyPropertyChanged("ConnectedDuration");
            NotifyPropertyChanged("LastAccessDuration");
        }

        private IDisposable _terminator;


        public void SetTerminator(IDisposable disposable)
        {
            _terminator = disposable;
        }

        public AppSession()
        {
            _userData = new Dictionary<string, object>();
            _id = Guid.NewGuid();
            AppInstanceId = Guid.NewGuid().ToString("N");
            SessionTimeout = TimeSpan.FromMinutes(15);
            Cookies = new Dictionary<string, string>();
            LastAccess = DateTime.Now;
        }

        public bool IsInitialized => UserAgent != null;

        public void Initialize(string hostDomain, string clientAddress, string userAgent)
        {
            HostDomain = hostDomain;
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
            CookiesSupported = true;
            Platform = "OS";
        }

        public void Accessed(IDictionary<string, string> cookies, bool userAction)
        {
            foreach (var cookie in cookies)
            {
                if (Cookies.ContainsKey(cookie.Key))
                {
                    Cookies[cookie.Key] = cookie.Value;
                }
                else
                {
                    Cookies.Add(cookie.Key, cookie.Value);
                }
            }


            if ((PermanentSessionId == null) && cookies.ContainsKey("ss-pid"))
            {
                PermanentSessionId = cookies["ss-pid"];
            }
            LastAccess = DateTime.Now;
            NotifyPropertyChanged("LastAccess");
            if (userAction)
            {
                LastUserAction = DateTime.Now;
                NotifyPropertyChanged("LastUserAction");
            }
            StonehengeCookieSet = cookies.ContainsKey("stonehenge-id");
            NotifyPropertyChanged("StonehengeCookieSet");
        }

        public void SetContext(string context)
        {
            Context = context;
        }

        public void EventsClear(bool forceEnd)
        {
            lock (_events)
            {
                //var privateEvents = Events.Where(e => e.StartsWith(AppService.PropertyNameId)).ToList();
                _events.Clear();
                //Events.AddRange(privateEvents);
                if (forceEnd)
                {
                    _eventRelease.Set();
                    _eventRelease.Set();
                }
            }
        }

        public void EventAdd(string name)
        {
            lock (_events)
            {
                if (!_events.Contains(name))
                    _events.Add(name);
                _eventRelease.Set();
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