using System.Reflection;

namespace IctBaden.StonehengeSample
{
  public class AboutVm
  {
    public string Version { get { return Assembly.GetEntryAssembly().GetName().Version.ToString(2); } } 
  }
}