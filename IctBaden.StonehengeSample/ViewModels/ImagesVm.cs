using IctBaden.Stonehenge;

namespace IctBaden.StonehengeSample.ViewModels
{
    public class ImagesVm : ActiveViewModel
    {
        public bool IsOn { get; set; }

        public string SwitchImg
        {
            get { return IsOn ? "switch_on" : "switch_off"; }
        }

        public string LampImg
        {
            get { return IsOn ? "lightbulb_on" : "lightbulb"; }
        }

        public ImagesVm(AppSession session)
            : base(session)
        {

        }

        [ActionMethod]
        public void Switch()
        {
            IsOn = !IsOn;
        }
    }
}