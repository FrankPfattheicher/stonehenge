using IctBaden.Stonehenge2.Core;
using IctBaden.Stonehenge2.ViewModel;

namespace IctBaden.Stonehenge2.Aurelia.Sample.ViewModels
{
    public class CookieVm : ActiveViewModel
    {
        public string Theme => Session.Cookies.ContainsKey("theme")
            ? Session.Cookies["theme"]
            : string.Empty;

        public int ThemeIndex => (Theme == "blue") ? 2 : ((Theme == "dark") ? 1 : 0);

        public CookieVm(AppSession session)
            : base(session)
        {
        }

    }
}