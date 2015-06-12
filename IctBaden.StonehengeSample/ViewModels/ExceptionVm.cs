namespace IctBaden.Stonehenge.Sample.ViewModels
{
    using IctBaden.Stonehenge;
    using IctBaden.Stonehenge.Sample;

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
