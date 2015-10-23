using System.Threading;

namespace IctBaden.Stonehenge.Sample.ViewModels
{
    using Stonehenge;
    using Sample;

    public class ExceptionVm : ActiveViewModel
    {
        public string ClientException { get; private set; }

        public ExceptionVm(AppSession session)
            :base(session)
        {
            Program.App.ClientException += exception =>
            {
                ClientException = exception.Message;
                Thread.Sleep(100);
                NotifyPropertyChanged("ClientException");
            };
        }
    }
}
