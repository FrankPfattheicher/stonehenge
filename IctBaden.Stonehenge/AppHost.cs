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
  public class AppHost : AppHostHttpListenerBase
  {
    public string Title { get; private set; }
    public string StartPage { get; private set; }
    public string UserRole { get; set; }
    public string Redirect { get; set; }

    public event Action<AppSession> NewSession;

    public AppHost(string title, string startPage)
      : base(title, typeof(AppHost).Assembly)
    {
      Title = title;
      StartPage = startPage;
    }

    internal void OnNewSession(AppSession session)
    {
      var handler = NewSession;
      if (handler == null)
        return;
      handler(session);
    }

    public override void Configure(Container container)
    {
      Config.AllowRouteContentTypeExtensions = false; // otherwise extensions are stripped out

      Routes.Add<AppFile>("/App/{FileName}")
            .Add<AppFile>("/App/{Path1}/{FileName}")
            .Add<AppFile>("/App/{Path1}/{Path2}/{FileName}")
            .Add<AppFile>("/App/{Path1}/{Path2}/{Path3}/{FileName}");

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
            return new NotFoundHttpHandler();
          return new RootRedirectHandler { RelativeUrl = "/App/index.html#/" + StartPage };
        });

      SetConfig(new EndpointHostConfig
      {
        EnableFeatures = Feature.All.Remove(Feature.Metadata)
      });
    }

  }
}
