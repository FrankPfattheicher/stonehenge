﻿namespace IctBaden.Stonehenge2.Resources
{
    using System;

    public class Resource
    {
        public string ContentType { get; private set; }
        public string Name { get; private set; }
        public string Source { get; private set; }

        public bool IsBinary => Data != null;

        public byte[] Data { get; }
        public string Text { get; private set; }
        public string ExtProperty { get; set; }

        public Resource(string name, string source, ResourceType type, string text)
            : this(name, source, type)
        {
            if (type.IsBinary)
                throw new ArgumentException("Resource " + name + " is aspected as text");
            Text = text;
        }
        public Resource(string name, string source, ResourceType type, byte[] data)
            : this(name, source, type)
        {
            if(!type.IsBinary)
                throw new ArgumentException("Resource " + name + " is aspected as binary");
            Data = data;
        }

        private Resource(string name, string source, ResourceType type)
        {
            Name = name;
            Source = source;
            ContentType = type.ContentType;
        }

    }
}

