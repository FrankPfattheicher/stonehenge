namespace IctBaden.StonehengeSample
{
  public class LoginVm
  {
    public string User { get; set; }
    public string Password { get; set; }

    public void Login()
    {
      Program.App.UserRole = "admin";
      Program.App.Redirect("form");
    }
  }
}