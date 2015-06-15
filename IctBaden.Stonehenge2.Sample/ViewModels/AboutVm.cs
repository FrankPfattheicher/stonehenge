namespace IctBaden.Stonehenge2.Sample.ViewModels
{
    using System.Reflection;

    public class AboutVm
    {
        public long Test { get; set; }
        public string Version { get { return Assembly.GetEntryAssembly().GetName().Version.ToString(2); } }

        public AboutVm()
        {
            Test = 12345;
        }
    }
}
