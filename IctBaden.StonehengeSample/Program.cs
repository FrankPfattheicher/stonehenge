namespace IctBaden.Stonehenge.Sample
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Stonehenge;

    static class Program
    {
        public static AppEngine App;
        public static string ClientException { get; set; }

        static void Main()
        {
            var options = Environment.CommandLine.ToUpper();
            // ReSharper disable once AssignNullToNotNullAttribute
            var asApp = options.Contains("/APP") || options.Contains("-APP") || 
                File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "IctBaden.Stonehenge.Sample.app"));

            App = new AppEngine(asApp ? 0 : 42000, false, "Stonehenge Sample", "about");

            App.ClientException += exception =>
            {
                ClientException = DateTime.Now.ToLongTimeString() + " - " + exception.Message;
            };

            App.Run(asApp);

            if (!asApp)
            {
                MessageBox.Show("Sample running on http://localhost:" + App.UsedPort, "Stonehenge");
            }
        }
    }
}
