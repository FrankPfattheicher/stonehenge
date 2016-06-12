namespace IctBaden.Stonehenge2.Resources
{
    using System;

    public class Resource
    {
        public string ContentType { get; private set; }
        public string Name { get; private set; }
        public string Source { get; private set; }

        public bool IsBinary => Data != null;

        public enum Cache
        {
            None,
            Revalidate,
            OneDay
        };
        /// <summary>
        /// Is allowed to be cached at the client.
        /// </summary>
        public Cache CacheMode { get; private set; }

        public byte[] Data { get; }
        public string Text { get; set; }

        public ViewModelInfo ViewModel { get; set; }


        public Resource(string name, string source, ResourceType type, string text, Cache cacheMode)
            : this(name, source, type, cacheMode)
        {
            if (type.IsBinary)
                throw new ArgumentException("Resource " + name + " is aspected as text");
            Text = text;
        }
        public Resource(string name, string source, ResourceType type, byte[] data, Cache cacheMode)
            : this(name, source, type, cacheMode)
        {
            if(!type.IsBinary)
                throw new ArgumentException("Resource " + name + " is aspected as binary");
            Data = data;
        }

        private Resource(string name, string source, ResourceType type, Cache cacheMode)
        {
            Name = name;
            Source = source;
            ContentType = type.ContentType;
            CacheMode = cacheMode;
        }

        internal void SetCacheMode(Cache mode)
        {
            CacheMode = mode;
        }
    }
}

