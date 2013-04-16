using IctBaden.Stonehenge;

namespace IctBaden.StonehengeSample
{
  public class LoginVm
  {
    public string User { get; set; }
    public string Password { get; set; }

    public void Login(AppSession session)
    {
      session.Remove("Role");
      session.Add("Role", "admin");
    }
  }
}