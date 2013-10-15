using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IctBaden.Stonehenge.Creators
{
  public static class UserPages
  {
    private const string InsertPoint = "//%PAGES%";
    private const string PageTemplate = "router.mapRoute('{0}','{0}','{1}',{2});";
    private static string userPages;

    private class Map
    {
      public string Route;
      public string Title;
      public int SortIndex;
      public bool Visible = true;
    }

    // ReSharper disable StringIndexOfIsCultureSpecific.1
    // ReSharper disable StringIndexOfIsCultureSpecific.2
    public static string InsertUserPages(string rootPath, string text)
    {
      if (userPages == null)
      {
        var pageFiles = Directory.GetFiles(Path.Combine(rootPath, @"App"), "*.html", SearchOption.AllDirectories)
          .Where(dir => (dir.IndexOf("index.html") == -1) && (dir.IndexOf("shell.html") == -1) && (dir.IndexOf("durandal") == -1));

        var maps = new List<Map>();
        foreach (var pageFile in pageFiles)
        {
          var map = new Map
            {
              Route = pageFile.Substring(pageFile.IndexOf(@"\App\") + 5).Replace('\\', '/').Replace(".html", "")
            };
          map.Title = map.Route;

          var pageText = File.ReadAllText(pageFile);
          var titleIndex = pageText.IndexOf("<!--Title:");
          if (titleIndex >= 0)
          {
            titleIndex += 10;

            var endIndex = pageText.IndexOf("-->", titleIndex);
            if (endIndex > 0)
            {
              map.Title = pageText.Substring(titleIndex, endIndex - titleIndex);
              map.Visible = (map.Title.Length > 0);
              var parts = map.Title.Split(new[] {':'});
              if (parts.Length > 1)
              {
                map.Title = parts[0];
                map.SortIndex = int.Parse(parts[1]);
              }
            }
          }
          
          maps.Add(map);
        }

        var pages = new StringBuilder();
        foreach (var page in maps.OrderBy(m => m.SortIndex).Select(map => string.Format(PageTemplate, map.Route, map.Title, map.Visible ? "true" : "false")))
        {
          pages.AppendLine(page);
        }

        userPages = string.Join(Environment.NewLine, pages);
      }
      return text.Replace(InsertPoint, userPages);
    }
    // ReSharper restore StringIndexOfIsCultureSpecific.1
    // ReSharper restore StringIndexOfIsCultureSpecific.2
  
  }
}