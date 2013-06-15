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
		public string Message { get; set; }

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
			MessageBox("Login", "Willkommen " + User);
		}
  }
}