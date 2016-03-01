namespace IctBaden.Stonehenge2.Resources
{
    using System;

    public class Resource
    {
        public string ContentType { get; private set; }
        public string Name { get; private set; }
        public string Source { get; private set; }

        public bool IsBinary => Data != null;
        /// <summary>
        /// Is allowed to be cached at the client.
        /// </summary>
        public bool IsCachable { get; private set; }

        public byte[] Data { get; }
        public string Text { get; private set; }

        public ViewModelInfo ViewModel { get; set; }


        public Resource(string name, string source, ResourceType type, string text, bool cachable)
            : this(name, source, type, cachable)
        {
            if (type.IsBinary)
                throw new ArgumentException("Resource " + name + " is aspected as text");
            Text = text;
        }
        public Resource(string name, string source, ResourceType type, byte[] data, bool cachable)
            : this(name, source, type, cachable)
        {
            if(!type.IsBinary)
                throw new ArgumentException("Resource " + name + " is aspected as binary");
            Data = data;
        }

        private Resource(string name, string source, ResourceType type, bool cachable)
        {
            Name = name;
            Source = source;
            ContentType = type.ContentType;
            IsCachable = cachable;
        }

    }
}

