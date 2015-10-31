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

        public Resource Post(AppSession session, string resourceName, object[] postParams, Dictionary<string, string> formData)
        {
            return Loaders.Select(loader => loader.Post(session, resourceName, postParams, formData))
                .FirstOrDefault(resource => resource != null);
        }
        public Resource Get(AppSession session, string resourceName)
        {
            return Loaders.Select(loader => loader.Get(session, resourceName))
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

            var loader = new Loader(new List<IStonehengeResourceProvider> { fileLoader, resLoader, viewModelCreator });

            return loader;
        }

    }
}
