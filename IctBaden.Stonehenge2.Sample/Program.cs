namespace IctBaden.Stonehenge2.Sample
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using IctBaden.Stonehenge2.Angular;
    using IctBaden.Stonehenge2.Hosting;
    using IctBaden.Stonehenge2.Katana;
    using IctBaden.Stonehenge2.Resources;

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

            server = new KatanaHost(loader);

            
            server.Start(false, "localhost", 42000);

            while (true)
            {
                Thread.Sleep(1000);
            }

#pragma warning disable 0162
            // ReSharper disable once HeuristicUnreachableCode
            server.Terminate();
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
