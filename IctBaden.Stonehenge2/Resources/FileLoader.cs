namespace IctBaden.Stonehenge2.Resources
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    using Core;

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

        public Resource Post(AppSession session, string resourceName, Dictionary<string, string> parameters, Dictionary<string, string> formData)
        {
            return null;
        }
        public Resource Get(AppSession session, string resourceName, Dictionary<string, string> parameters)
        {
            var fullFileName = Path.Combine(RootPath, resourceName);
            if(!File.Exists(fullFileName)) return null;

            var resourceExtension = Path.GetExtension(resourceName);
            var resourceType = ResourceType.GetByExtension(resourceExtension);
            if (resourceType == null)
            {
                Debug.WriteLine($"FileLoader({resourceName}): not found");
                return null;
            }

            Debug.WriteLine($"FileLoader({resourceName}): {fullFileName}");
            if (resourceType.IsBinary)
            {
                return new Resource(resourceName, "file://" + fullFileName, resourceType, File.ReadAllBytes(fullFileName), Resource.Cache.OneDay);
            }

            return new Resource(resourceName, "file://" + fullFileName, resourceType, File.ReadAllText(fullFileName), Resource.Cache.OneDay);
        }

    }
}
