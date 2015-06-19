namespace IctBaden.Stonehenge2.Resources
{
    using IctBaden.Stonehenge2.Core;

    public interface IResourceProvider
    {
        Resource Get(AppSession session, string resourceName);

        Resource Post(AppSession session, string resourceName, object[] postParams);

    }
}
