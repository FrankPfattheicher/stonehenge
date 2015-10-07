namespace IctBaden.Stonehenge2.Sample.ViewModels
{
    using System.Reflection;

    using ViewModel;

    public class AboutVm
    {
        public long Test { get; set; }
        public string Version => Assembly.GetEntryAssembly().GetName().Version.ToString(2);

        public AboutVm()
        {
            Test = 12345;
        }

        [ActionMethod]
        public void MoreInfo()
        {
            
        }
    }
}
