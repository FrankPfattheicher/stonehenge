namespace IctBaden.Stonehenge.Sample.ViewModels
{
    using System.Reflection;

    public class AboutVm
  {
    public string Version { get { return Assembly.GetEntryAssembly().GetName().Version.ToString(2); } } 
  }
}