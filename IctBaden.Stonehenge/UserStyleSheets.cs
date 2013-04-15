using System;
using System.IO;
using System.Linq;

namespace IctBaden.Stonehenge
{
  public static class UserStyleSheets
  {
    private const string InsertPoint = "<!--link-stylesheet-->";
    private const string LinkTemplate = "<link href='{0}' rel='stylesheet'>";
    private static string userStyleSheets;

    public static string InsertUserCssLinks(string rootPath, string text)
    {
      if (userStyleSheets == null)
      {
        var links = Directory.GetFiles(Path.Combine(rootPath, @"App\styles"), "*.css", SearchOption.AllDirectories)
          .Select(dir => string.Format(LinkTemplate, dir.Substring(dir.IndexOf(@"\App\") + 1).Replace('\\', '/')));
        userStyleSheets = string.Join(Environment.NewLine, links);
      }
      return text.Replace(InsertPoint, userStyleSheets);
    }
  }
}