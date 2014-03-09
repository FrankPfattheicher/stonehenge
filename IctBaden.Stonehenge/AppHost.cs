using System;
using System.Diagnostics;
using Funq;
using IctBaden.Stonehenge.Services;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Common;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.WebHost.Endpoints.Support;

namespace IctBaden.Stonehenge
{
    internal class AppHost : AppHostHttpListenerBase
    {
        public string Title { get; private set; }
        public string StartPage { get; private set; }
        public bool MessageBoxContentHtml { get; private set; }
        public string Redirect { get; set; }
        public TimeSpan EventTimeout { get; set; }
        public TimeSpan SessionTimeout { get; set; }
        public bool HasSessionTimeout { get { return SessionTimeout.TotalMilliseconds > 0.1; } }

        public event Action<AppSession> SessionCreated;
        public event Action<AppSession> SessionTerminated;

        public AppHost(string title, string startPage, bool messageBoxContentHtml)
            : base(title, typeof(AppHost).Assembly)
        {
            Title = title;
            StartPage = startPage;
            MessageBoxContentHtml = messageBoxContentHtml;
            EventTimeout = TimeSpan.FromSeconds(10);
        }

        internal void OnSessionCreated(AppSession session)
        {
            var handler = SessionCreated;
            if (handler == null)
                return;
            handler(session);
        }
        internal void OnSessionTerminated(AppSession session)
        {
            var handler = SessionTerminated;
            if (handler == null)
                return;
            handler(session);
        }

        public override void Configure(Container container)
        {
            Config.AllowRouteContentTypeExtensions = false; // otherwise extensions are stripped out

            Routes.Add<AppFile>("/robots.txt")
                  .Add<AppFile>("/app/{FileName}")
                  .Add<UserFile>("/app/user/{FileName}")
                  .Add<AppFile>("/app/{Path1}/{FileName}")
                  .Add<AppFile>("/app/{Path1}/{Path2}/{FileName}")
                  .Add<AppFile>("/app/{Path1}/{Path2}/{Path3}/{FileName}")
                  .Add<AppFile>("/app/{Path1}/{Path2}/{Path3}/{Path4}/{FileName}");

            Routes.Add<AppViewModel>("/ViewModel/{ViewModel}")
                  .Add<AppViewModel>("/ViewModel/{ViewModel}/{Source}");

            Routes.Add<AppEvent>("/Events/{ViewModel}");

            Plugins.Add(new SessionFeature());
            container.Register<ICacheClient>(new MemoryCacheClient());
            var authRepository = new InMemoryAuthRepository();
            container.Register<IUserAuthRepository>(authRepository);

            CatchAllHandlers.Add((httpMethod, pathInfo, filePath) =>
              {
                  Debug.WriteLine("CatchAllHandler({0}, {1}, {2})", httpMethod, pathInfo, filePath);
                  if (pathInfo != "/")
                  {
                      return new NotFoundHttpHandler();
                  }
                  var session = AppSessionCache.NewSession();
                  return new RootRedirectHandler { RelativeUrl = string.Format("/app/index.html#/{0}?stonehenge_id={1}", StartPage, session.Id) };
              });

            SetConfig(new EndpointHostConfig
            {
                EnableFeatures = Feature.All.Remove(Feature.Metadata)
            });
        }

    }
}
