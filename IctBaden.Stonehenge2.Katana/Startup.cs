using Microsoft.Owin.Diagnostics;
using SqueezeMe;

namespace IctBaden.Stonehenge2.Katana
{
    using Middleware;
    using Resources;

    using Owin;

    internal class Startup
    {
        public Startup(string title, IStonehengeResourceProvider loader)
        {
            StonehengeSession.AppTitle = title;
            StonehengeSession.ResourceLoader = loader;
        }

        public void Configuration(IAppBuilder app)
        {
#if DEBUG
            var errorOptions = new ErrorPageOptions
            {
                ShowExceptionDetails = true,
                ShowSourceCode = true
            };
            app.UseErrorPage(errorOptions);
#endif
            app.UseCompression();
            app.Use<StonehengeSession>();
            app.Use<StonehengeRoot>();
            app.Use<StonehengeContent>();
        }

    }
}