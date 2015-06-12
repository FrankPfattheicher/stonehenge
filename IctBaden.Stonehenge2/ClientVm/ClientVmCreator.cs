namespace IctBaden.Stonehenge2.ClientVm
{
    using System.Text;

    using IctBaden.Stonehenge2.Resources;

    public class ClientVmCreator : IResourceProvider
    {
        public Resource Load(string resourceName)
        {
            if (!resourceName.StartsWith("ViewModels")) return null;

            return new Resource(resourceName, "ClientVmCreator", ResourceType.GetByExtension("json"), Encoding.UTF8.GetBytes("{ \"test\": \"123456\" }"));
        }
    }
}