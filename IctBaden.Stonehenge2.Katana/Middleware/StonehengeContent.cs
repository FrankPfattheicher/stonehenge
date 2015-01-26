namespace IctBaden.Stonehenge2.Katana.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using IctBaden.Stonehenge2.Resources;

    using Microsoft.Owin;

    public class StonehengeContent
    {
        readonly Func<IDictionary<string, object>, Task> next;

        public StonehengeContent(Func<IDictionary<string, object>, Task> next)
        {
            this.next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);

            var path = context.Request.Path.Value;
            if (path == "/")
            {
                await next.Invoke(environment);
                return;
            }

            var response = context.Get<Stream>("owin.ResponseBody");
            var resourceLoader = context.Get<IResourceProvider>("stonehenge.ResourceLoader");
            var content = resourceLoader.Load(path.ToString().Substring(1));

            if (content == null)
            {
                await next.Invoke(environment);
                return;
            }
            context.Response.ContentType = content.ContentType;
            if (content.IsBinary)
            {
                using (var writer = new BinaryWriter(response))
                {
                    writer.Write(content.Data);
                }
            }
            else
            {
                using (var writer = new StreamWriter(response))
                {
                    writer.Write(content.Text);
                }
            }

        }
    }
}