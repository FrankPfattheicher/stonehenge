namespace IctBaden.Stonehenge2.Katana
{
    using System.Diagnostics;

    using IctBaden.Stonehenge2.Katana.Middleware;
    using IctBaden.Stonehenge2.Resources;

    using Owin;

    internal class Startup
    {
        private readonly IResourceProvider resourceLoader;

        public Startup(IResourceProvider loader)
        {
            resourceLoader = loader;
        }

        public void Configuration(IAppBuilder app)
        {
#if DEBUG
            app.UseErrorPage();
#endif
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path;
                Debug.WriteLine("Begin Request: " + path);
                context.Environment.Add("stonehenge.ResourceLoader", resourceLoader);
                await next.Invoke();
                Debug.WriteLine("End Request: " + path);
            });

            app.Use<StonehengeContent>();
            app.Use<StonehengeRoot>();
        } 
    }
}