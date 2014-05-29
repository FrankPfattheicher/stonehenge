using System;
using System.IO;
using System.Windows.Forms;
using IctBaden.Stonehenge;

namespace IctBaden.StonehengeSample
{
    static class Program
    {
        public static AppEngine App;

        static void Main()
        {
            var options = Environment.CommandLine.ToUpper();
            var asApp = options.Contains("/APP") || options.Contains("-APP") || File.Exists("IctBaden.StonehengeSample.app");

            App = new AppEngine(asApp ? 0 : 42000, false, "Stonehenge Sample", "about");

            App.ClientException += exception => MessageBox.Show(exception.Message, "stonehenge - Client exception");
            App.Run(asApp);

            if (!asApp)
            {
                MessageBox.Show("Sample running on http://localhost:" + App.UsedPort, "Stonehenge");
            }
        }
    }
}
