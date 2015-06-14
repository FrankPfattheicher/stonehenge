namespace IctBaden.Stonehenge2.Core
{
    using IctBaden.Stonehenge2.Hosting;

    public class AppHost
    {
        private readonly IStonehengeHost appHost;

        public AppHost(IStonehengeHost host)
        {
            appHost = host;
        }

        public bool Run(bool applicationWindow)
        {
            return appHost.Start(false, "localhost", 42000);
        }
    }
}
