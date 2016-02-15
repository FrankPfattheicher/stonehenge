using System;
using System.Reflection;
using System.Threading;
using IctBaden.Stonehenge2.Core;
using IctBaden.Stonehenge2.ViewModel;

namespace IctBaden.Stonehenge2.AngularSample.ViewModels
{
    public class StartVm : ActiveViewModel, IDisposable
    {
        public string TimeStamp => DateTime.Now.ToLongTimeString();
        public double Numeric { get; set; }
        public string Test { get; set; }
        public string Version => Assembly.GetEntryAssembly().GetName().Version.ToString(2);

        private readonly Thread updater;

        public StartVm(AppSession session) : base (session)
        {
            Numeric = 123.456;
            Test = "54321";
            updater = new Thread(
                () =>
                    {
                        while (true)
                        {
                            Thread.Sleep(10000);
                            NotifyPropertyChanged(nameof(TimeStamp));
                        }
                        // ReSharper disable once FunctionNeverReturns
                    });
            updater.Start();
        }

        public void Dispose()
        {
            updater.Abort();
        }

        [ActionMethod]
        public void Save(int number, string text)
        {
            Test = number + Test + text;
        }

    }
}
