namespace IctBaden.Stonehenge2.Sample.ViewModels
{
    using System.Reflection;

    using IctBaden.Stonehenge2.ViewModel;

    public class StartVm
    {
        public double Numeric { get; set; }
        public string Test { get; set; }
        public string Version => Assembly.GetEntryAssembly().GetName().Version.ToString(2);

        public StartVm()
        {
            Numeric = 123.456;
            Test = "54321";
        }

        [ActionMethod]
        public void Save(int number, string text)
        {
            Test = number + Test + text;
        }
    }
}
