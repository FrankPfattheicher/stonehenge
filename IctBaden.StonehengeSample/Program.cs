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

            var asApp = Environment.CommandLine.ToUpper().Contains("/APP");
            App.Run(asApp);

            if (!asApp)
            {
                MessageBox.Show("Stonehenge Sample running...");
            }
        }
    }
}
