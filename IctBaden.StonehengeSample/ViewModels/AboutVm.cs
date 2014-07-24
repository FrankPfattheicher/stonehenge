using System.Reflection;

namespace IctBaden.StonehengeSample.ViewModels
{
  public class AboutVm
  {
    public string Version { get { return Assembly.GetEntryAssembly().GetName().Version.ToString(2); } } 
  }
}