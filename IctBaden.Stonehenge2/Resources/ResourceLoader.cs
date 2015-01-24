﻿namespace IctBaden.Stonehenge2.Resources
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class ResourceLoader : IResourceProvider
    {
        internal class AssemblyResource
        {
            public string FullName { get; private set; }

            public string ShortName { get; private set; }

            public Assembly Assembly { get; private set; }

            public AssemblyResource(string fullName, string shortName, Assembly assembly)
            {
                FullName = fullName;
                ShortName = shortName;
                Assembly = assembly;
            }
        }
        private readonly List<Assembly> assemblies;
        private readonly Lazy<Dictionary<string, AssemblyResource>> resources;

        public ResourceLoader()
            : this(new []
                       {
                           Assembly.GetEntryAssembly(), 
                           Assembly.GetExecutingAssembly()
                       })
        {

        }
        public ResourceLoader(IEnumerable<Assembly> assembliesToUse)
        {
            assemblies = assembliesToUse.ToList();
            resources = new Lazy<Dictionary<string, AssemblyResource>>(
                () =>
                {
                    var dict = new Dictionary<string, AssemblyResource>();
                    foreach (var assemby in assemblies.Where(a => a != null).Distinct())
                    {
                        foreach (var resource in assemby.GetManifestResourceNames())
                        {
                            const string BaseName = ".App.";
                            var appBase = resource.IndexOf(BaseName, System.StringComparison.Ordinal);
                            if (appBase == -1) continue;

                            var shortName = resource.Substring(appBase + BaseName.Length);
                            dict.Add(shortName, new AssemblyResource(resource, shortName, assemby));
                        }
                    }
                    return dict;
                });
        }
        public Resource Load(string resourceName)
        {
            var asmResource = resources.Value
                .FirstOrDefault(res => System.String.Compare(res.Key, resourceName, System.StringComparison.Ordinal) == 0);
            if (asmResource.Key == null) return null;

            var resourceExtension = Path.GetExtension(resourceName);
            var resourceType = ResourceType.GetByExtension(resourceExtension);
            if (resourceType == null) return null;

            using (var stream = asmResource.Value.Assembly.GetManifestResourceStream(asmResource.Value.FullName))
            {
                if (stream != null)
                {
                    if (resourceType.IsBinary)
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            var data = reader.ReadBytes((int)stream.Length);
                            return new Resource(resourceName, "res://" + asmResource.Value.FullName, resourceType, data);
                        }
                    }
                    else
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var text = reader.ReadToEnd();
                            return new Resource(resourceName, "res://" + asmResource.Value.FullName, resourceType, text);
                        }
                    }
                }
            }

            return null;
        }
    }
}