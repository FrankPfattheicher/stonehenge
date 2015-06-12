namespace IctBaden.Stonehenge2.Sample
{
    using System;
    using System.Linq;
    using System.Threading;

    using IctBaden.Stonehenge2.Angular;
    using IctBaden.Stonehenge2.Hosting;
    using IctBaden.Stonehenge2.Katana;
    using IctBaden.Stonehenge2.Resources;

    static class Program
    {
        private static IStonehengeHost server;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var loader = Loader.CreateDefaultLoader();
            var resLoader = (ResourceLoader)loader.Loaders.First(ld => ld.GetType() == typeof(ResourceLoader));
            resLoader.AddAssembly(typeof(AngularVmCreator).Assembly);

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
