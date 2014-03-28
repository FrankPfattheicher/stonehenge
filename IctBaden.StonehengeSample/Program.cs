using System;
using System.Windows.Forms;
using IctBaden.Stonehenge;

namespace IctBaden.StonehengeSample
{
    static class Program
    {
        public static AppEngine App;

        static void Main()
        {
            App = new AppEngine("Stonehenge Sample", "about");

            var options = Environment.CommandLine.ToUpper();
            var asApp = options.Contains("/APP") || options.Contains("-APP");
            App.Run(asApp);

            if (!asApp)
            {
                MessageBox.Show("Sample running on http://localhost:42000", "Stonehenge");
            }
        }
    }
}
