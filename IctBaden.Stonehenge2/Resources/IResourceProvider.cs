namespace IctBaden.Stonehenge2.Resources
{
    using IctBaden.Stonehenge2.Core;

    public interface IResourceProvider
    {
        Resource Load(AppSession session, string resourceName);
    }
}
