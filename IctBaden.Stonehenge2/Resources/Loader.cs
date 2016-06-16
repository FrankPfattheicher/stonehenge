using System;

namespace IctBaden.Stonehenge2.Resources
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Core;
    using ViewModel;

    public class Loader : IStonehengeResourceProvider
    {
        public List<IStonehengeResourceProvider> Loaders { get; }

        public Loader(List<IStonehengeResourceProvider> loaders = null)
        {
            Loaders = loaders ?? new List<IStonehengeResourceProvider>();
        }

        public void Dispose()
        {
            Loaders.ForEach(l => l.Dispose());
            Loaders.Clear();
        }

        public Resource Post(AppSession session, string resourceName, Dictionary<string, string> parameters, Dictionary<string, string> formData)
        {
            return Loaders.Select(loader => loader.Post(session, resourceName, parameters, formData))
                .FirstOrDefault(resource => resource != null);
        }
        public Resource Get(AppSession session, string resourceName, Dictionary<string, string> parameters)
        {
            var disableCache = false;

            if (resourceName.Contains("${"))
            {
                resourceName = ReplaceFields(session, resourceName);
                disableCache = true;
            }

            var loadedResource = Loaders.Select(loader => loader.Get(session, resourceName, parameters))
                .FirstOrDefault(resource => resource != null);

            if (disableCache)
            {
                loadedResource?.SetCacheMode(Resource.Cache.None);
            }

            return loadedResource;
        }
        
        private string ReplaceFields(AppSession session, string resourceName)
        {
            // support es6 format "${}"
            var replaced = string.Empty;
            while (resourceName.Length > 0)
            {
                var start = resourceName.IndexOf("${", StringComparison.InvariantCulture);
                if (start == -1)
                {
                    replaced += resourceName;
                    break;
                }
                replaced += resourceName.Substring(0, start);
                var field = resourceName.Substring(start + 2);
                resourceName = resourceName.Substring(start + 2);

                var end = field.IndexOf('}');
                field = field.Substring(0, end);

                if (session.Cookies.ContainsKey(field))
                {
                    replaced += session.Cookies[field];
                }

                resourceName = resourceName.Substring(end + 1);
            }
            return replaced;
        }

        public static Loader CreateDefaultLoader()
        {
            var assemblies = new List<Assembly>
                                 {
                                     Assembly.GetEntryAssembly(),
                                     Assembly.GetExecutingAssembly(),
                                     Assembly.GetAssembly(typeof(ResourceLoader))
                                 };

            var resLoader = new ResourceLoader(assemblies);

            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? Directory.GetCurrentDirectory();
            var fileLoader = new FileLoader(Path.Combine(path, "App"));

            var viewModelCreator = new ViewModelProvider();

            var loader = new Loader(new List<IStonehengeResourceProvider> { fileLoader, resLoader, viewModelCreator });

            return loader;
        }

    }
}
