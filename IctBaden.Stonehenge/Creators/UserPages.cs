using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IctBaden.Stonehenge.Creators
{
  internal static class UserPages
  {
    private const string InsertPoint = "//%PAGES%";
    private const string StartPageTemplate = "{{ route: ['', '{0}'], moduleId: '{0}', title: '{1}', nav: 1, visible: {2} }},";
    private const string PageTemplate = "{{ route: '{0}', moduleId: '{0}', title: '{1}', nav: true, visible: {2} }},";
    private static string userPages = null;
    private static readonly string AppPath = Path.DirectorySeparatorChar + "app" + Path.DirectorySeparatorChar;

    private class Map
    {
      public string Route;
      public string Title;
      public int SortIndex;
      public bool Visible = true;
    }

    // ReSharper disable StringIndexOfIsCultureSpecific.1
    // ReSharper disable StringIndexOfIsCultureSpecific.2
    private static Map GetMapFromPageText(string route, string pageText)
    {
      var map = new Map { Route = route, Title = route };

      var titleIndex = pageText.IndexOf("<!--Title:");
      if (titleIndex >= 0)
      {
        titleIndex += 10;

        var endIndex = pageText.IndexOf("-->", titleIndex);
        if (endIndex > 0)
        {
          map.Title = pageText.Substring(titleIndex, endIndex - titleIndex);
          map.Visible = (map.Title.Length > 0);
          var parts = map.Title.Split(new[] { ':' });
          if (parts.Length > 1)
          {
            map.Title = parts[0];
            map.SortIndex = int.Parse(parts[1]);
          }
        }
      }

      return map;
    }

    public static void Init(string rootPath, string startPage)
    {
      userPages = string.Empty;
      var maps = new List<Map>();

      var pageFilesPath = Path.Combine(rootPath, @"app");
      if (Directory.Exists(pageFilesPath))
      {
        var pageFiles = Directory.GetFiles(pageFilesPath, "*.html", SearchOption.AllDirectories)
          .Where(dir => (dir.IndexOf("index.html") == -1) && (dir.IndexOf("shell.html") == -1) && (dir.IndexOf("durandal") == -1));

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var pageFile in pageFiles)
        {
          var route = pageFile.Substring(pageFile.IndexOf(AppPath) + 5).Replace('\\', '/').Replace(".html", string.Empty);
          var pageText = File.ReadAllText(pageFile);
          maps.Add(GetMapFromPageText(route, pageText));
        }
      }

      var assembly = Assembly.GetEntryAssembly();
      var baseName = assembly.GetName().Name + ".app.";
      foreach (var resourceName in assembly.GetManifestResourceNames()
        .Where(name => (name.EndsWith(".html")) && (!name.Contains("shell.html"))).OrderBy(name => name))
      {
        var route = resourceName.Replace(baseName, string.Empty).Replace(".html", string.Empty);
        var pageText = "";
        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
          if (stream != null)
          {
            using (var reader = new StreamReader(stream))
            {
              pageText = reader.ReadToEnd();
            }
          }
        }

        maps.Add(GetMapFromPageText(route, pageText));
      }

      var pages = maps.OrderBy(m => m.SortIndex)
        .Select(map => string.Format((map.Route == startPage) ? StartPageTemplate : PageTemplate, map.Route, map.Title, map.Visible ? "true" : "false"));
      userPages = string.Join(Environment.NewLine, pages);
    }

    public static string InsertUserPages(string rootPath, string startPage, string text)
    {
      if (userPages == null)
      {
        Init(rootPath, startPage);
      }
      return text.Replace(InsertPoint, userPages);
    }
    // ReSharper restore StringIndexOfIsCultureSpecific.1
    // ReSharper restore StringIndexOfIsCultureSpecific.2
  }
}