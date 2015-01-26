namespace IctBaden.Stonehenge2.Resources
{
    using System.Collections.Generic;
    using System.Linq;

    public class ResourceType
    {
        public string Extension { get; private set; }
        public string ContentType { get; private set; }

        public bool IsBinary { get; private set; }

        public ResourceType(string extension, string contentType, bool isBinary)
        {
            Extension = extension;
            ContentType = contentType;
            IsBinary = isBinary;
        }

        private readonly static List<ResourceType> KnownTypes = new List<ResourceType>
                {
                    new ResourceType( ".htm", "text/html", false),
                    new ResourceType( ".html", "text/html" , false),
                    new ResourceType( ".css", "text/css" , false),
                    new ResourceType( ".js", "text/javascript" , false),
                    new ResourceType( ".png", "image/png" , true),
                    new ResourceType( ".gif", "image/gif" , true),
                    new ResourceType( ".jpg", "image/jpeg" , true),
                    new ResourceType( ".jpeg", "image/jpeg" , true),
                    new ResourceType( ".wav", "audio/x-wav" , true),
                    new ResourceType( ".ico", "image/x-icon" , true),
                };

        public static ResourceType GetByExtension(string extension)
        {
            return KnownTypes.FirstOrDefault(rt => rt.Extension == extension) ??
                new ResourceType(extension, "application/octet-stream", true);
        }
    }
}
