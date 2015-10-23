namespace IctBaden.Stonehenge.Sample
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Stonehenge;

    static class Program
    {
        public static AppEngine App;

        static void Main()
        {
            var options = Environment.CommandLine.ToUpper();
            // ReSharper disable once AssignNullToNotNullAttribute
            var asApp = options.Contains("/APP") || options.Contains("-APP") || 
                File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "IctBaden.Stonehenge.Sample.app"));

            var port = 42000;
            if (options.Contains("/P:"))
            {
                int.TryParse(options.Substring(options.IndexOf("/P:", StringComparison.CurrentCultureIgnoreCase) + 3), out port);
            }
            App = new AppEngine(asApp ? 0 : port, false, "Stonehenge Sample", "about");
            App.Run(asApp);

            if (!asApp)
            {
                MessageBox.Show("Sample running on http://localhost:" + App.UsedPort, "Stonehenge");
            }
        }
    }
}
