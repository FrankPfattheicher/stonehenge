// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
namespace IctBaden.Stonehenge2.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Core;

    public class ResourceLoader : IStonehengeResourceProvider
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

        public void Dispose()
        {
            assemblies.Clear();
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
                        AddAssemblyResources(assemby, dict);
                    }
                    return dict;
                });
        }

        public void AddAssembly(Assembly assembly)
        {
            var asmResources = resources.Value;
            if (asmResources.Values.Any(res => res.Assembly == assembly)) 
                return;

            AddAssemblyResources(assembly, asmResources);
        }

        private static void AddAssemblyResources(Assembly assemby, Dictionary<string, AssemblyResource> dict)
        {
            foreach (var resource in assemby.GetManifestResourceNames())
            {
                const string baseName = ".app.";
                var appBase = resource.IndexOf(baseName, StringComparison.InvariantCultureIgnoreCase);
                if (appBase == -1)
                {
                    continue;
                }

                var shortName = resource.Substring(appBase + baseName.Length);
                var resourceId = shortName
                        .Replace("@", "_")
                        .Replace("-", "_")
                        .Replace("._0", ".0")
                        .Replace("._1", ".1")
                        .Replace("._2", ".2")
                        .Replace("._3", ".3")
                        .Replace("._4", ".4")
                        .Replace("._5", ".5")
                        .Replace("._6", ".6")
                        .Replace("._7", ".7")
                        .Replace("._8", ".8")
                        .Replace("._9", ".9");
                var asmResource = new AssemblyResource(resource, shortName, assemby);
                if (!dict.ContainsKey(resourceId))
                {
                    //Debug.WriteLine($"ResourceLoader: Added {shortName}");
                    dict.Add(resourceId, asmResource);
                }
            }
        }

        public Resource Post(AppSession session, string resourceName, object[] postParams, Dictionary<string, string> formData)
        {
            return null;
        }

        public Resource Get(AppSession session, string name)
        {
            var resourceName = name
                        .Replace("@", "_")
                        .Replace("-", "_")
                        .Replace("/", ".");
            var asmResource = resources.Value
                .FirstOrDefault(res => string.Compare(res.Key, resourceName, true, CultureInfo.InvariantCulture) == 0);
            if (asmResource.Key == null)
            {
                Debug.WriteLine($"ResourceLoader({resourceName}): not found");
                return null;
            }

            var resourceExtension = Path.GetExtension(resourceName);
            var resourceType = ResourceType.GetByExtension(resourceExtension);
            if (resourceType == null)
            {
                Debug.WriteLine($"ResourceLoader({resourceName}): not found");
                return null;
            }

            using (var stream = asmResource.Value.Assembly.GetManifestResourceStream(asmResource.Value.FullName))
            {
                if (stream != null)
                {
                    if (resourceType.IsBinary)
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            var data = reader.ReadBytes((int)stream.Length);
                            Debug.WriteLine($"ResourceLoader({resourceName}): {asmResource.Value.FullName}");
                            return new Resource(resourceName, "res://" + asmResource.Value.FullName, resourceType, data, Resource.Cache.Revalidate);
                        }
                    }
                    else
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var text = reader.ReadToEnd();
                            Debug.WriteLine($"ResourceLoader({resourceName}): {asmResource.Value.FullName}");
                            return new Resource(resourceName, "res://" + asmResource.Value.FullName, resourceType, text, Resource.Cache.Revalidate);
                        }
                    }
                }
            }

            Debug.WriteLine($"ResourceLoader({resourceName}): not found");
            return null;
        }
    }
}
