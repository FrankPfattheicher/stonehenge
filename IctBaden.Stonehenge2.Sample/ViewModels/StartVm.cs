namespace IctBaden.Stonehenge2.Sample.ViewModels
{
    using System.Reflection;

    using IctBaden.Stonehenge2.ViewModel;

    public class StartVm
    {
        public long Test { get; set; }
        public string Version { get { return Assembly.GetEntryAssembly().GetName().Version.ToString(2); } }

        public StartVm()
        {
            Test = 54321;
        }

        [ActionMethod]
        public void Save()
        {
            
        }
    }
}
