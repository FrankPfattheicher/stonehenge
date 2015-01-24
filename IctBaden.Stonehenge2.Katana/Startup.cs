namespace IctBaden.Stonehenge2.Katana
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

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

            app.Properties.Add("test", 2323);
            app.Use<StonehengeRoot>();
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path;
                Debug.WriteLine("Begin Request: " + path);
                
                var response = context.Get<Stream>("owin.ResponseBody");
                using (var writer = new StreamWriter(response))
                {
                    var content = resourceLoader.Load(path.ToString());
                    context.Response.ContentType = content.ContentType;
                    await writer.WriteAsync(content.Text);
                }
                var headers = context.Get<IDictionary<string, string[]>>("owin.ResponseHeaders");

                Debug.WriteLine("End Request: " + path);
            });
        } 
    }
}