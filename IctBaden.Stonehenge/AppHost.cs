using System;
using System.Diagnostics;
using System.Net;
using Funq;
using IctBaden.Stonehenge.Services;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.WebHost.Endpoints.Support;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace IctBaden.Stonehenge
{
    internal class AppHost : AppHostHttpListenerLongRunningBase
    {
        public string Title { get; private set; }
        public string StartPage { get; private set; }
        public bool MessageBoxContentHtml { get; private set; }
        public string Redirect { get; set; }
        public TimeSpan EventTimeout { get; set; }
        public TimeSpan SessionTimeout { get; set; }
        public bool HasSessionTimeout => SessionTimeout.TotalMilliseconds > 0.1;

        public event Action<AppSession> SessionCreated;
        public event Action<AppSession> SessionTerminated;
        public event Action<Exception> ClientException;

        public AppHost(string title, string startPage, bool messageBoxContentHtml)
            : base(title, typeof(AppHost).Assembly)
        {
            Title = title;
            StartPage = startPage;
            MessageBoxContentHtml = messageBoxContentHtml;
            EventTimeout = TimeSpan.FromSeconds(10);

            var eventlog = new EventLogTraceListener("stonehenge") {Filter = new EventTypeFilter(SourceLevels.Error)};
            Trace.Listeners.Add(eventlog);
            Trace.AutoFlush = true;
        }

        internal void OnSessionCreated(AppSession session)
        {
            SessionCreated?.Invoke(session);
        }
        internal void OnSessionTerminated(AppSession session)
        {
            SessionTerminated?.Invoke(session);
        }
        internal void OnClientException(Exception exception)
        {
            ClientException?.Invoke(exception);
        }

        public override void Configure(Container container)
        {
            Config.AllowRouteContentTypeExtensions = false; // otherwise extensions are stripped out
            
            Routes.Add<AppFile>("/robots.txt")
                  .Add<AppFile>("/favicon.ico")
                  .Add<AppFile>("/.well-known/{Path1}/{FileName}")
                  .Add<AppFile>("/app/{FileName}")
                  .Add<UserFile>("/app/user/{FileName}")
                  .Add<AppFile>("/app/{Path1}/{FileName}")
                  .Add<AppFile>("/app/{Path1}/{Path2}/{FileName}")
                  .Add<AppFile>("/app/{Path1}/{Path2}/{Path3}/{FileName}")
                  .Add<AppFile>("/app/{Path1}/{Path2}/{Path3}/{Path4}/{FileName}");

            Routes.Add<AppViewModel>("/ViewModel/{ViewModel}")
                  .Add<AppViewModel>("/ViewModel/{ViewModel}/{Source}");

            Routes.Add<AppEvent>("/Events/{ViewModel}");

            Routes.Add<AppException>("/Exception/{Cause}");

            Plugins.Add(new SessionFeature());
            container.Register<ICacheClient>(new MemoryCacheClient());
            var authRepository = new InMemoryAuthRepository();
            container.Register<IUserAuthRepository>(authRepository);

            CatchAllHandlers.Add((httpMethod, pathInfo, filePath) =>
              {
                  Debug.WriteLine($"CatchAllHandler({httpMethod}, {pathInfo}, {filePath})");
                  if (pathInfo != "/")
                  {
                      return new NotFoundHttpHandler();
                  }
                  return new RootRedirectHandler { RelativeUrl = $"/app/index.html#/{StartPage}"};
              });

            ExceptionHandler = (req, res, name, exception) =>
            {
                var message = exception.Message;
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    message += Environment.NewLine + exception.Message;
                }
                message += Environment.NewLine + exception.StackTrace;
                Trace.TraceError("Stonehenge exception: " + message);
            };

            ServiceExceptionHandler = (req, request, exception) =>
            {
                var message = exception.Message;
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    message += Environment.NewLine + exception.Message;
                }
                message += Environment.NewLine + exception.StackTrace;
                Trace.TraceError("Stonehenge exception: " + message);
                return new HttpResult(HttpStatusCode.InternalServerError, exception.Message + Environment.NewLine + exception.StackTrace);
            };

            SetConfig(new EndpointHostConfig
            {
                EnableFeatures = Feature.All.Remove(Feature.Metadata),
                DebugMode = true
            });
        }
    }
}
