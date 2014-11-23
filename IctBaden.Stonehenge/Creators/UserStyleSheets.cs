using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            .Select(dir => string.Format(LinkTemplate, dir.Substring(dir.IndexOf(AppPath, StringComparison.InvariantCulture) + 1).Replace('\\', '/')));
          styleSheets = string.Join(Environment.NewLine, links);
        }

        var assembly = Assembly.GetEntryAssembly();
        var ressourceBaseName = assembly.GetName().Name + ".";
        var baseNameStyles = ressourceBaseName + "app.styles.";
        var baseNameTheme = ressourceBaseName + "app.themes.";
        var ressourceNames = assembly.GetManifestResourceNames();
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var resourceName in ressourceNames
          .Where(name => name.EndsWith(".css") && (name.StartsWith(baseNameStyles, StringComparison.InvariantCultureIgnoreCase) || name.StartsWith(baseNameTheme + theme))))
        {
          var css = resourceName.Substring(ressourceBaseName.Length).Replace(".", "/").Replace("/css", ".css");
          styleSheets += Environment.NewLine + string.Format(LinkTemplate, css);
        }

        path = Path.Combine(rootPath, "app", "themes", theme + ".css");
        if (File.Exists(path))
        {
          var css = path.Substring(path.IndexOf(AppPath, StringComparison.InvariantCulture) + 1).Replace('\\', '/');
          styleSheets += Environment.NewLine + string.Format(LinkTemplate, css);
        }

          if(!StyleSheets.ContainsKey(theme))
          {
              StyleSheets.Add(theme, styleSheets);
          }
      }
      return text.Replace(InsertPoint, StyleSheets[theme]);
    }
  }
}
