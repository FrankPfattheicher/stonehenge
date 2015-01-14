namespace IctBaden.Stonehenge2.Hosting
{
  public interface IStonehengeHost
  {
      /// <summary>
      /// Gives the base URL the hosting service is using for the initial page.
      /// </summary>
      string BaseUrl { get; }

      /// <summary>
      /// Start hosting service.
      /// </summary>
      /// <param name="hostAddress">IP address to listen on or null to listen on all adresses.</param>
      /// <param name="hostPort">Port number to listen on or 0 for default (80 or 443 for SSL).</param>
      /// <param name="useSsl">Use secure sockets for hosting.</param>
      /// <returns>True if successfully started.</returns>
      bool Start(string hostAddress, int hostPort, bool useSsl);

      /// <summary>
      /// Terminate hosting service.
      /// </summary>
      void Terminate();
  }
}
