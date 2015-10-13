namespace IctBaden.Stonehenge.Sample.ViewModels
{
    using IctBaden.Stonehenge;

    public class LoginVm : ActiveViewModel
  {
    public LoginVm(AppSession session) : base(session)
    {
      User = "Demo";
    }

    public string User { get; set; }
    public string Password { get; set; }
    public string Message { get; set; }

    [ActionMethod]
    public void Login(AppSession session)
    {
      if (string.IsNullOrEmpty(User))
      {
        Message = "A user name is required.";
        return;
      }
      if(Password != "stonehenge")
      {
        Message = "Login failed";
        return;
      }

      Message = string.Empty;
      MessageBox("Login", "You're welcome, " + User + "!");
    }

    [ActionMethod]
    public void Logout(AppSession session)
    {
      User = string.Empty;
      Password = string.Empty;
      Message = string.Empty;
    }
  }
}