namespace IctBaden.Stonehenge2.Resources
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using IctBaden.Stonehenge2.Core;
    using IctBaden.Stonehenge2.ViewModel;

    public class Loader : IResourceProvider
    {
        public List<IResourceProvider> Loaders { get; private set; }

        public Loader(List<IResourceProvider> loaders = null)
        {
            Loaders = loaders ?? new List<IResourceProvider>();
        }

        public Resource Load(AppSession session, string resourceName)
        {
            return Loaders.Select(loader => loader.Load(session, resourceName))
                .FirstOrDefault(resource => resource != null);
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

            var loader = new Loader(new List<IResourceProvider> { fileLoader, resLoader, viewModelCreator });

            return loader;
        }

    }
}
