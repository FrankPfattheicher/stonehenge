using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IctBaden.Stonehenge
{
  public static class UserPages
  {
    private const string InsertPoint = "//%PAGES%";
    private const string PageTemplate = "router.mapNav('{0}','{0}','{1}');";
    private static string userPages;

    private struct Map
    {
      public string Route;
      public string Title;
      public int SortIndex;
    }

    public static string InsertUserPages(string rootPath, string text)
    {
      if (userPages == null)
      {
        var pageFiles = Directory.GetFiles(Path.Combine(rootPath, @"App"), "*.html", SearchOption.AllDirectories)
          .Where(dir => (dir.IndexOf("index.html") == -1) && (dir.IndexOf("shell.html") == -1) && (dir.IndexOf("durandal") == -1));

        var maps = new List<Map>();
        foreach (var pageFile in pageFiles)
        {
          var map = new Map();
          map.Route = pageFile.Substring(pageFile.IndexOf(@"\App\") + 5).Replace('\\', '/').Replace(".html", "");
          map.Title = map.Route;

          var pageText = File.ReadAllText(pageFile);
          var titleIndex = pageText.IndexOf("<!--Title:");
          if (titleIndex > 0)
          {
            titleIndex += 10;
            var endIndex = pageText.IndexOf("-->", titleIndex);
            if (endIndex > 0)
            {
              map.Title = pageText.Substring(titleIndex, endIndex - titleIndex);
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
        foreach (var page in maps.OrderBy(m => m.SortIndex).Select(map => string.Format(PageTemplate, map.Route, map.Title)))
        {
          pages.AppendLine(page);
        }

        userPages = string.Join(Environment.NewLine, pages);
      }
      return text.Replace(InsertPoint, userPages);
    }
  }
}