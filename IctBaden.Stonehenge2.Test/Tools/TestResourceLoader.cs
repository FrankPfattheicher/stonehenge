namespace IctBaden.Stonehenge2.Test.Tools
{
    using System.Collections.Generic;
    using System.IO;

    using Core;
    using Stonehenge2.Resources;

    public class TestResourceLoader : IResourceProvider
    {
        private readonly string content;

        public TestResourceLoader(string content)
        {
            this.content = content;
        }

        public void Dispose()
        {
        }

        public Resource Post(AppSession session, string resourceName, object[] postParams, Dictionary<string, string> formData)
        {
            return null;
        }
        public Resource Get(AppSession session, string resourceName)
        {
            var resourceExtension = Path.GetExtension(resourceName);
            return new Resource(resourceName, "test://TestResourceLoader.content", ResourceType.GetByExtension(resourceExtension), content);
        }
    }
}