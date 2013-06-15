using IctBaden.Stonehenge;

namespace IctBaden.StonehengeSample
{
  public class LoginVm : ActiveViewModel
  {
    public LoginVm(AppSession session) : base(session)
    {
    }

    public string User { get; set; }
    public string Password { get; set; }

    public void Login(AppSession session)
    {
      MessageBox("Login", "Willkommen " + User);
    }
  }
}