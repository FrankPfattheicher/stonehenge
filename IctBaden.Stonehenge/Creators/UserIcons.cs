using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace IctBaden.Stonehenge.Creators
{
  internal static class UserIcons
  {
    private const string InsertPoint = "<!--more-icons-->";
    private const string ImageTemplate = "<link rel=\"apple-touch-icon\" sizes=\"{0}x{1}\" href=\"app/{2}\">\r\n" +
                                         "<link rel=\"icon\" sizes=\"{0}x{1}\" href=\"app/{2}\">";

    private class Icon
    {
      public int Size;
      public string Link;
    }

    public static string InsertUserIconLinks(string rootPath, string text)
    {
      var icons = new List<Icon>();

      var assembly = Assembly.GetEntryAssembly();
      var baseName = assembly.GetName().Name + ".app.";
      foreach (var resourceName in assembly.GetManifestResourceNames()
        .Where(name => (name.Contains(baseName + "icon_"))).OrderBy(name => name))
      {
        var imageName = resourceName.Replace(baseName, string.Empty);
        var sizeExpr = new Regex("icon_([0-9]+)x([0-9]+)\\.png");
        var match = sizeExpr.Match(imageName.ToLower());
        if (!match.Success) 
          continue;

        int x, y;
        if (int.TryParse(match.Groups[1].Value, out x) && int.TryParse(match.Groups[2].Value, out y))
        {
          icons.Add(new Icon {Size = x, Link = string.Format(ImageTemplate, x, y, imageName) } );
        }
      }

      if (icons.Count > 0)
      {
        text = text.Replace(InsertPoint, string.Join(Environment.NewLine, icons.OrderBy(i => i.Size).Select(i => i.Link)));
      }
      return text;
    }
  }
}