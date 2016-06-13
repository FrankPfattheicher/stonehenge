namespace IctBaden.Stonehenge2.Resources
{
    using System.Collections.Generic;
    using System.Linq;

    public class ResourceType
    {
        public string Extension { get; }
        public string ContentType { get; private set; }

        public bool IsBinary { get; private set; }

        public ResourceType(string extension, string contentType, bool isBinary)
        {
            Extension = extension;
            ContentType = contentType;
            IsBinary = isBinary;
        }

        public static readonly ResourceType Text = new ResourceType("txt", "text/plain", false);
        public static readonly ResourceType Htm = new ResourceType("htm", "text/html", false);
        public static readonly ResourceType Html = new ResourceType("html", "text/html" , false);
        public static readonly ResourceType Css = new ResourceType("css", "text/css", false);
        public static readonly ResourceType Js = new ResourceType("js", "text/javascript", false);
        public static readonly ResourceType Calendar = new ResourceType("ics", "text/calendar", false);
        public static readonly ResourceType Json = new ResourceType("json", "application/json; charset=utf-8", false);

        public static readonly ResourceType Png = new ResourceType("png", "image/png", true);
        public static readonly ResourceType Gif = new ResourceType("gif", "image/gif", true);
        public static readonly ResourceType Jpg = new ResourceType("jpg", "image/jpeg", true);
        public static readonly ResourceType Jpeg = new ResourceType("jpeg", "image/jpeg", true);
        public static readonly ResourceType Wav = new ResourceType("wav", "audio/x-wav", true);
        public static readonly ResourceType Ico = new ResourceType("ico", "image/x-icon", true);

        public static readonly List<ResourceType> KnownTypes = new List<ResourceType>
                {
                    Htm, Html,
                    Css,
                    Js,
                    Json,
                    Png, Gif, Jpg, Jpeg,
                    Wav,
                    Ico
                };

        public static ResourceType GetByExtension(string extension)
        {
            extension = extension.Replace(".", "").ToLower();
            return KnownTypes.FirstOrDefault(rt => rt.Extension == extension) ??
                new ResourceType(extension, "application/octet-stream", true);
        }
    }
}
