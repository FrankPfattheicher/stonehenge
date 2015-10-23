using System;

namespace IctBaden.Stonehenge.Sample.ViewModels
{
    using System.Reflection;

    public class AboutVm
    {
        public string Version => Assembly.GetEntryAssembly().GetName().Version.ToString(2);
        public string Machine => Environment.MachineName;
    }
}
