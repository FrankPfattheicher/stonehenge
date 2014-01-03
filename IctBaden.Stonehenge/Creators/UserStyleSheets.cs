using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IctBaden.Stonehenge.Creators
{
  internal static class UserStyleSheets
  {
    private const string InsertPoint = "<!--link-stylesheet-->";
    private const string LinkTemplate = "<link href='{0}' rel='stylesheet'>";
    private static readonly Dictionary<string, string> StyleSheets = new Dictionary<string, string>();
    private static readonly string AppPath = Path.DirectorySeparatorChar + "app" + Path.DirectorySeparatorChar;

    public static string InsertUserCssLinks(string rootPath, string text, string theme)
    {
      if (!StyleSheets.ContainsKey(theme))
      {
        var styleSheets = string.Empty;

        var path = Path.Combine(rootPath, "app", "styles");
        if (Directory.Exists(path))
        {
          var links = Directory.GetFiles(path, "*.css", SearchOption.AllDirectories)
            .Select(dir => string.Format(LinkTemplate, dir.Substring(dir.IndexOf(AppPath) + 1).Replace('\\', '/')));
          styleSheets = string.Join(Environment.NewLine, links);
        }

        path = Path.Combine(rootPath, "app", "themes", theme + ".css");
        if (File.Exists(path))
        {
          var css = path.Substring(path.IndexOf(AppPath) + 1).Replace('\\', '/');
          styleSheets += Environment.NewLine + string.Format(LinkTemplate, css);
        }

        var assembly = Assembly.GetEntryAssembly();
        var ressourceBaseName = assembly.GetName().Name + ".";
        var baseNameStyles = ressourceBaseName + "app.styles.";
        var baseNameTheme = ressourceBaseName + "app.themes.";
        foreach (var resourceName in assembly.GetManifestResourceNames()
          .Where(name => name.EndsWith(".css") && (name.StartsWith(baseNameStyles) || name.StartsWith(baseNameTheme + theme))))
        {
          var css = resourceName.Replace(ressourceBaseName, string.Empty).Replace(".", "/").Replace("/css", ".css");
          styleSheets += Environment.NewLine + string.Format(LinkTemplate, css);
        }

        UserStyleSheets.StyleSheets.Add(theme, styleSheets);
      }
      return text.Replace(InsertPoint, StyleSheets[theme]);
    }
  }
}
