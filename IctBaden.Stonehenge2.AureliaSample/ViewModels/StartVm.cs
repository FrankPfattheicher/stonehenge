using System;
using System.Reflection;
using System.Threading;
using IctBaden.Stonehenge2.Core;
using IctBaden.Stonehenge2.Resources;
using IctBaden.Stonehenge2.ViewModel;

namespace IctBaden.Stonehenge2.Aurelia.Sample.ViewModels
{
    public class StartVm : ActiveViewModel, IDisposable
    {
        public string TimeStamp => DateTime.Now.ToLongTimeString();
        public double Numeric { get; set; }
        public string Test { get; set; }
        public string Version => Assembly.GetEntryAssembly().GetName().Version.ToString(2);

        private readonly Thread _updater;

        public StartVm(AppSession session) : base (session)
        {
            Numeric = 123.456;
            Test = "54321";
            _updater = new Thread(
                () =>
                    {
                        while (true)
                        {
                            Thread.Sleep(10000);
                            NotifyPropertyChanged(nameof(TimeStamp));
                        }
                        // ReSharper disable once FunctionNeverReturns
                    });
            _updater.Start();
        }

        public void Dispose()
        {
            _updater.Abort();
        }

        [ActionMethod]
        public void Save(int number, string text)
        {
            Test = number + Test + text;
        }
        
        public override Resource GetDataResource(string resourceName)
        {
            if (resourceName.EndsWith(".ics"))
            {
                var cal = @"BEGIN:VCALENDAR
PRODID:-//ICT Baden GmbH//Framework Library 2016//DE
VERSION:2.0
CALSCALE:GREGORIAN
METHOD:PUBLISH
BEGIN:VEVENT
UID:902af1f31c454e5983d707c6d7ee3d4a
DTSTART:20160501T181500Z
DTEND:20160501T194500Z
DTSTAMP:20160501T202905Z
CREATED:20160501T202905Z
LAST-MODIFIED:20160501T202905Z
TRANSP:OPAQUE
STATUS:CONFIRMED
ORGANIZER:ARD
SUMMARY:Tatort
END:VEVENT
END:VCALENDAR
";
                return new Resource(resourceName, "Sample", ResourceType.Calendar, cal, Resource.Cache.None);
            }
            return new Resource(resourceName, "Sample", ResourceType.Text, $"This ist the content of {resourceName} file ;-)", Resource.Cache.None);
        }
    }
}
