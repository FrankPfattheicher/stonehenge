namespace IctBaden.Stonehenge2.Sample
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Pipes;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using IctBaden.Stonehenge2.Angular;
    using IctBaden.Stonehenge2.Hosting;
    using IctBaden.Stonehenge2.Katana;
    using IctBaden.Stonehenge2.Resources;
    using IctBaden.Stonehenge2.SimpleHttp;

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
            var appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? ".";
            var appFilesPath = Path.Combine(appPath, "app");

            var angular = new AngularResourceProvider();
            angular.Init(appFilesPath, "start");

            var loader = Loader.CreateDefaultLoader();
            var resLoader = (ResourceLoader)loader.Loaders.First(ld => ld.GetType() == typeof(ResourceLoader));
            resLoader.AddAssembly(typeof(AngularResourceProvider).Assembly);
            loader.Loaders.Add(angular);

            Console.WriteLine(@"Starting server");
            //server = new KatanaHost(loader);
            server = new SimpleHttpHost(loader);

            if (server.Start(false, "localhost", 42000))
            {
                Console.WriteLine(@"Started server on: " + server.BaseUrl);
                while (true)
                {
                    Thread.Sleep(1000);
                }
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
