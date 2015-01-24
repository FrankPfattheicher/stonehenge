namespace IctBaden.Stonehenge2.Katana.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

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


            if (context.Request.Path.Value == "/")
            {
                context.Response.Redirect("/App/Index.html");
                return;
            }

            await context.Response.WriteAsync("<h1>Hello from My First Middleware</h1>");
            await next.Invoke(environment);
        }
    }
}