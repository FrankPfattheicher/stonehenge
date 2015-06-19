namespace IctBaden.Stonehenge2.Resources
{
    using System.IO;

    using IctBaden.Stonehenge2.Core;

    public class FileLoader : IResourceProvider
    {
        public string RootPath { get; private set; }

        public FileLoader(string path)
        {
            RootPath = path;
        }

        public Resource Post(AppSession session, string resourceName, object[] postParams)
        {
            return null;
        }
        public Resource Get(AppSession session, string resourceName)
        {
            var fullFileName = Path.Combine(RootPath, resourceName);
            if(!File.Exists(fullFileName)) return null;

            var resourceExtension = Path.GetExtension(resourceName);
            var resourceType = ResourceType.GetByExtension(resourceExtension);
            if (resourceType == null) return null;

            if (resourceType.IsBinary)
            {
                return new Resource(resourceName, "file://" + fullFileName, resourceType, File.ReadAllBytes(fullFileName));
            }

            return new Resource(resourceName, "file://" + fullFileName, resourceType, File.ReadAllText(fullFileName));
        }
    }
}
