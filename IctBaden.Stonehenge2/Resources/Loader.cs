namespace IctBaden.Stonehenge2.Resources
{
    using System.Collections.Generic;
    using System.Linq;

    public class Loader : IResourceProvider
    {
        public List<IResourceProvider> Loaders { get; private set; }

        public Loader(List<IResourceProvider> loaders = null)
        {
            Loaders = loaders ?? new List<IResourceProvider>();
        }

        public Resource Load(string resourceName)
        {
            return Loaders.Select(loader => loader.Load(resourceName))
                .FirstOrDefault(resource => resource != null);
        }
    }
}
