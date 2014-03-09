using System;
using System.Text;
using IctBaden.Stonehenge;

namespace IctBaden.StonehengeSample.ViewModels
{
    public class DownloadsVm : ActiveViewModel
    {
        public DownloadsVm(AppSession session)
            : base(session)
        {

        }

        public UserData GetUserData(string fileName)
        {
            if (fileName == "Test.txt")
            {
                var txt = "Hello World at " + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString();
                return new UserData(txt);
            }

            var csv = new StringBuilder();
            csv.AppendLine("TimeStamp;Data");
            var rnd = new Random(DateTime.Now.Millisecond);
            var dt = DateTime.Now;
            for (var dx = 0; dx < 100; dx++)
            {
                csv.AppendLine(dt.ToLongDateString() + "  " + dt.ToLongTimeString() + ";" + rnd.Next());
                dt += TimeSpan.FromSeconds(10);
            }
            return new UserData("text/csv", Encoding.UTF8.GetBytes(csv.ToString()));
        }

    }
}