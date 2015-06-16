namespace IctBaden.Stonehenge2.Test.Tools
{
    using System.IO;

    using IctBaden.Stonehenge2.Core;
    using IctBaden.Stonehenge2.Resources;

    public class TestResourceLoader : IResourceProvider
    {
        private readonly string content;

        public TestResourceLoader(string content)
        {
            this.content = content;
        }

        public Resource Load(AppSession session, string resourceName)
        {
            var resourceExtension = Path.GetExtension(resourceName);
            return new Resource(resourceName, "test://TestResourceLoader.content", ResourceType.GetByExtension(resourceExtension), content);
        }
    }
}