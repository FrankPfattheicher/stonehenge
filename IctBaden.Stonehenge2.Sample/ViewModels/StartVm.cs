namespace IctBaden.Stonehenge2.Sample.ViewModels
{
    using System;
    using System.Reflection;

    using IctBaden.Stonehenge2.ViewModel;

    public class StartVm
    {
        public string Test { get; set; }
        public string Version { get { return Assembly.GetEntryAssembly().GetName().Version.ToString(2); } }

        public StartVm()
        {
            Test = "54321";
        }

        [ActionMethod]
        public void Save(int number, string text)
        {
            Test = DateTime.UtcNow.Ticks.ToString() + number + text;
        }
    }
}
