﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using IctBaden.Stonehenge.Annotations;
using IctBaden.Stonehenge.Services;
using ServiceStack.CacheAccess;

namespace IctBaden.Stonehenge
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.Net;
    using System.Web;
    using System.Web.Configuration;

    public class AppSession : INotifyPropertyChanged, ISession
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
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public Guid Id { get; private set; }
        public string StackId { get; set; }

        internal List<string> Events = new List<string>();
        internal AutoResetEvent EventRelease = new AutoResetEvent(false);
        internal readonly PassiveTimer EventPollingActive = new PassiveTimer();

        [Obsolete("This returns false always now - event handling will be rewritten")]
        public bool IsWaitingForEvents => false; //EventPollingActive.Running && !EventPollingActive.Timeout;

        private object _viewModel;
        public object ViewModel
        {
            get => _viewModel;
            set
            {
                if (_viewModel is INotifyPropertyChanged npc)
                {
                    npc.PropertyChanged -= ViewModelPropertyChanged;
                }

                _viewModel = value;
                npc = value as INotifyPropertyChanged;
                if (npc != null)
                {
                    npc.PropertyChanged += ViewModelPropertyChanged;
                }
            }
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (!(sender is ActiveViewModel avm))
                return;
            lock (avm.Session.Events)
            {
                avm.Session.EventAdd(args.PropertyName);
            }
        }

        public void ClientAddressChanged(string address)
        {
            ClientAddress = address;
            NotifyPropertyChanged(nameof(ClientAddress));
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
                ViewModel = null;
            }

            // view model changed
            EventsClear(true);

            var asm = Assembly.GetEntryAssembly();
            var vmtype = asm.GetTypes().FirstOrDefault(type => type.FullName?.EndsWith(typeName) ?? false);
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
                    var sessionCtor = vmtype.GetConstructors()
                        .FirstOrDefault(ctor => (ctor.GetParameters().Length == 1) && (ctor.GetParameters()[0].ParameterType == GetType()));
                    vm = sessionCtor != null 
                        ? Activator.CreateInstance(vmtype, this) 
                        : Activator.CreateInstance(vmtype);
                }
                else
                {
                    vm = Activator.CreateInstance(vmtype);
                }
            }
            catch (Exception ex)
            {
                var message = $"AppSession.SetViewModelType({typeName})" + Environment.NewLine;
                message += ex.Message;
                if (ex.InnerException != null)
                {
                    message += Environment.NewLine + ex.InnerException.Message;
                    message += Environment.NewLine + ex.InnerException.StackTrace;
                }
                else
                {
                    message += Environment.NewLine + ex.StackTrace;
                }
                message += Environment.NewLine;
                
                Trace.TraceError(message);
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
                var isNumeric = int.TryParse(parts[0], out _);
                return isNumeric ? HostDomain : parts[0];
            }
        }

        public Dictionary<string, string> Parameters = new Dictionary<string, string>();

        private readonly Dictionary<string, object> _userData;
        public object this[string key]
        {
            get => _userData.ContainsKey(key) ? _userData[key] : null;
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

        public event Action Terminated;
        public event Action TimedOut;
        private Timer _pollSessionTimeout;
        public TimeSpan SessionTimeout { get; private set; }

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
            NotifyPropertyChanged(nameof(ConnectedDuration));
            NotifyPropertyChanged(nameof(LastAccessDuration));
        }

        private IDisposable _terminator;

        public void SetTerminator(IDisposable disposable)
        {
            _terminator = disposable;
        }

        public AppSession()
        {
            _userData = new Dictionary<string, object>();
            Id = Guid.NewGuid();
        }

        internal bool IsInitialized => UserAgent != null;

        internal void Initialize(string hostDomain, string hostUrl, string clientAddress, string userAgent)
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

        public void Accessed(IDictionary<string, Cookie> cookies, Dictionary<string, string> parameters, bool userAction)
        {
            if ((StackId == null) && cookies.ContainsKey("ss-pid"))
            {
                StackId = cookies["ss-pid"].Value;
            }

            foreach (var parameter in parameters)
            {
                if (Parameters.ContainsKey(parameter.Key))
                {
                    Parameters[parameter.Key] = parameter.Value;
                }
                else
                {
                    Parameters.Add(parameter.Key, parameter.Value);
                }
            }

            LastAccess = DateTime.Now;
            NotifyPropertyChanged(nameof(LastAccess));
            if (userAction)
            {
                LastUserAction = DateTime.Now;
                NotifyPropertyChanged(nameof(LastUserAction));
            }
            CookieSet = cookies.ContainsKey("stonehenge_id");
            NotifyPropertyChanged(nameof(CookieSet));
        }

        public void SetContext(string context)
        {
            Context = context;
        }

        public void EventsClear(bool forceEnd)
        {
            lock (Events)
            {
                var privateEvents = Events.Where(e => e.StartsWith(AppService.PropertyNameId)).ToList();
                Events.Clear();
                Events.AddRange(privateEvents);
                if (forceEnd)
                {
                    EventRelease.Set();
                    EventRelease.Set();
                }
            }
        }

        public void EventAdd(string name)
        {
#if DEBUG
            Trace.TraceInformation($"EventAdd({name})");
#endif
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
            return $"[{Id}] {ConnectedSince.ToShortDateString() + " " + ConnectedSince.ToShortTimeString()} {SubDomain}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Terminate()
        {
            var vm = ViewModel as IDisposable;
            ViewModel = null;
            vm?.Dispose();
            Terminated?.Invoke();
        }

        public string GetETag(string url)
        {
            var etag = $"{AppSessionCache.InstanceId}{SubDomain.GetHashCode():x}{url.GetHashCode():x}";
            return etag;
        }

    }
}