using IctBaden.Stonehenge;
using IctBaden.Stonehenge.Annotations;

namespace IctBaden.StonehengeSample.ViewModels
{
  public class ImagesVm : ActiveViewModel
  {
    private bool isOn;

    public string SwitchImg
    {
      get { return isOn ? "switch_on" : "switch_off"; }
    }

    public string LampImg
    {
      get { return isOn ? "lightbulb_on" : "lightbulb"; }
    }

    public ImagesVm(AppSession session)
      : base(session)
    {
      
    }

    [ActionMethod]
    public void Switch()
    {
      isOn = !isOn;
    }
  }
}