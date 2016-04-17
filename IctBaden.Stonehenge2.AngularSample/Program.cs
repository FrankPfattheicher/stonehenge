using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using IctBaden.Stonehenge2.Angular;
using IctBaden.Stonehenge2.Caching;
using IctBaden.Stonehenge2.Hosting;
using IctBaden.Stonehenge2.Katana;
using IctBaden.Stonehenge2.Resources;
using IctBaden.Stonehenge2.SimpleHttp;

namespace IctBaden.Stonehenge2.AngularSample
{
    static class Program
    {
        //private static AppHost app;
        private static IStonehengeHost server;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Console.WriteLine(@"");
            Console.WriteLine(@"Stonehenge 2 sample");
            Console.WriteLine(@"");


            var appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? ".";
            var appFilesPath = Path.Combine(appPath, "app");

            var loader = Loader.CreateDefaultLoader();
            var resLoader = (ResourceLoader)loader.Loaders.First(ld => ld.GetType() == typeof(ResourceLoader));

            var cache = new MemoryCache();

            var hosting = "owin";
            if (Environment.CommandLine.Contains("/Simple")) { hosting = "simple"; }

            // Select client framework
            Console.WriteLine(@"Using client framework AngularJS");
            var angular = new AngularResourceProvider();
            angular.Init(appFilesPath, "start");
            resLoader.AddAssembly(typeof(AngularResourceProvider).Assembly);
            loader.Loaders.Add(angular);

            // Select hosting technology
            switch (hosting)
            {
                case "owin":
                    Console.WriteLine(@"Using Katana OWIN hosting");
                    server = new KatanaHost(loader);
                    break;
                case "simple":
                    Console.WriteLine(@"Using simple http hosting");
                    server = new SimpleHttpHost(loader, cache);
                    break;
            }

            Console.WriteLine(@"Starting server");
            var terminate = new AutoResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) => { terminate.Set(); };

            if (server.Start("Sample", false, "localhost", 32000))
            {
                Console.WriteLine(@"Started server on: " + server.BaseUrl);
                terminate.WaitOne();
                Console.WriteLine(@"Server terminated.");
            }
            else
            {
                Console.WriteLine(@"Failed to start server on: " + server.BaseUrl);
            }

#pragma warning disable 0162
            // ReSharper disable once HeuristicUnreachableCode
            server.Terminate();
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
