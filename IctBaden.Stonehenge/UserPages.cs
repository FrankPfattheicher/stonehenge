using System;
using System.IO;
using System.Linq;

namespace IctBaden.Stonehenge
{
  public static class UserPages
  {
    private const string InsertPoint = "//%PAGES%";
    private const string PageTemplate = "router.mapNav('{0}');";
    private static string userPages;

    public static string InsertUserPages(string rootPath, string text)
    {
      if (userPages == null)
      {
        var routes = Directory.GetFiles(Path.Combine(rootPath, @"App"), "*.html", SearchOption.AllDirectories)
          .Where(dir => (dir.IndexOf("index.html") == -1) && (dir.IndexOf("shell.html") == -1) && (dir.IndexOf("durandal") == -1))
          .Select(dir => string.Format(PageTemplate, dir.Substring(dir.IndexOf(@"\App\") + 5).Replace('\\', '/').Replace(".html", "")));
        userPages = string.Join(Environment.NewLine, routes);
      }
      return text.Replace(InsertPoint, userPages);
    }
  }
}