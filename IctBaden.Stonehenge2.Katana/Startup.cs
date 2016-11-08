using System.Collections.Generic;
using IctBaden.Stonehenge2.Core;
using Microsoft.Owin.Diagnostics;
using SqueezeMe;

namespace IctBaden.Stonehenge2.Katana
{
    using Middleware;
    using Resources;

    using Owin;

    internal class Startup
    {
        private readonly string appTitle;
        private readonly IStonehengeResourceProvider resourceLoader;
        private readonly List<AppSession> appSessions = new List<AppSession>();

        public Startup(string title, IStonehengeResourceProvider loader)
        {
            appTitle = title;
            resourceLoader = loader;
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
            app.Use((context, next) =>
            {
                context.Environment.Add("stonehenge.AppTitle", appTitle);
                context.Environment.Add("stonehenge.ResourceLoader", resourceLoader);
                context.Environment.Add("stonehenge.AppSessions", appSessions);
                return next.Invoke();
            });
            app.Use<StonehengeSession>();
            app.Use<StonehengeRoot>();
            app.Use<StonehengeContent>();
        }

    }
}