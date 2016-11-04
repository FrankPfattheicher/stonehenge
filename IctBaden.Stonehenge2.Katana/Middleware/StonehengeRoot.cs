namespace IctBaden.Stonehenge2.Katana.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Owin;

    public class StonehengeRoot
    {
        private readonly Func<IDictionary<string, object>, Task> _next;

        public StonehengeRoot(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);

            if (context.Request.Path.Value == "/")
            {
                context.Response.Redirect("/Index.html");
                return;
            }

            await _next.Invoke(environment);
        }
    }
}