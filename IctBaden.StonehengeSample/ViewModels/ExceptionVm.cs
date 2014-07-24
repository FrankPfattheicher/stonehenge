namespace IctBaden.StonehengeSample.ViewModels
{
    using IctBaden.Stonehenge;

    public class ExceptionVm : ActiveViewModel
    {
        public string ClientException
        {
            get
            {
                return Program.ClientException;
            }
        }
    }
}
