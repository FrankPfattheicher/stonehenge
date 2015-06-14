namespace IctBaden.Stonehenge2.Client
{
    using IctBaden.Stonehenge2.Resources;

    public class ViewModelProvider : IResourceProvider
    {
        public Resource Load(string resourceName)
        {
            if (!resourceName.StartsWith("ViewModels")) return null;

            return new Resource(resourceName, "ViewModelProvider", ResourceType.Json,
                "{ \"Test\": \"123456\", \"Version\": \"2.00.001\" }");
        }
    }
}