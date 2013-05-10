using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IctBaden.Stonehenge
{
  public static class UserStyleSheets
  {
    private const string InsertPoint = "<!--link-stylesheet-->";
    private const string LinkTemplate = "<link href='{0}' rel='stylesheet'>";
    private static Dictionary<string, string> userStyleSheets = new Dictionary<string, string>();

    public static string InsertUserCssLinks(string rootPath, string text, string theme)
    {
      if (!userStyleSheets.ContainsKey(theme))
      {
        var styleSheets = string.Empty;
        var path = Path.Combine(rootPath, @"App\styles");
        if (Directory.Exists(path))
        { 
          var links = Directory.GetFiles(path, "*.css", SearchOption.AllDirectories)
            .Select(dir => string.Format(LinkTemplate, dir.Substring(dir.IndexOf(@"\App\") + 1).Replace('\\', '/')));
          styleSheets = string.Join(Environment.NewLine, links);
        }
        path = Path.Combine(rootPath, @"App\themes", theme + ".css");
        if (File.Exists(path))
        {
          var css = path.Substring(path.IndexOf(@"\App\") + 1).Replace('\\', '/');
          styleSheets += Environment.NewLine + string.Format(LinkTemplate, css);
        }
        userStyleSheets.Add(theme, styleSheets);
      }
      return text.Replace(InsertPoint, userStyleSheets[theme]);
    }
  }
}