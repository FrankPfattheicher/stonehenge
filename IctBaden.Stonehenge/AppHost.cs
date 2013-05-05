using Funq;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Common;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.WebHost.Endpoints;

namespace IctBaden.Stonehenge
{
  public class AppHost : AppHostHttpListenerBase
  {
    public string Title { get; private set; }
    public string UserRole { get; set; }
    public string Redirect { get; set; }

    public AppHost(string title)
      : base(title, typeof(AppHost).Assembly)
    {
      Title = title;
    }

    public override void Configure(Container container)
    {
      Routes.Add<AppFile>("/App/{FileName}")
            .Add<AppFile>("/App/{Path1}/{FileName}")
            .Add<AppFile>("/App/{Path1}/{Path2}/{FileName}")
            .Add<AppFile>("/App/{Path1}/{Path2}/{Path3}/{FileName}");

      Routes.Add<AppViewModel>("/ViewModel/{ViewModel}")
            .Add<AppViewModel>("/ViewModel/{ViewModel}/{Source}");

			Routes.Add<AppEvent>("/Events/{ViewModel}");

      Plugins.Add(new SessionFeature());
      container.Register<ICacheClient>(new MemoryCacheClient());
      
      SetConfig(new EndpointHostConfig
      {
        EnableFeatures = Feature.All.Remove(Feature.Metadata),
        DefaultRedirectPath = "/App/index.html"
      }); 
    }

  }
}
