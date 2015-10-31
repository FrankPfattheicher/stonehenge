namespace IctBaden.Stonehenge2.Resources
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    using IctBaden.Stonehenge2.Core;

    public class FileLoader : IStonehengeResourceProvider
    {
        public string RootPath { get; }

        public FileLoader(string path)
        {
            RootPath = path;
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
            var fullFileName = Path.Combine(RootPath, resourceName);
            if(!File.Exists(fullFileName)) return null;

            var resourceExtension = Path.GetExtension(resourceName);
            var resourceType = ResourceType.GetByExtension(resourceExtension);
            if (resourceType == null)
            {
                Debug.WriteLine("ResourceLoader({0}): null", resourceName);
                return null;
            }

            Debug.WriteLine("ResourceLoader({0}): {1}", resourceName, fullFileName);
            if (resourceType.IsBinary)
            {
                return new Resource(resourceName, "file://" + fullFileName, resourceType, File.ReadAllBytes(fullFileName));
            }

            return new Resource(resourceName, "file://" + fullFileName, resourceType, File.ReadAllText(fullFileName));
        }

    }
}
