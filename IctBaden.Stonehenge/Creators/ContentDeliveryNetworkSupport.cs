using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace IctBaden.Stonehenge.Creators
{
  public static class ContentDeliveryNetworkSupport
  {
    private const string CdnConfigurationFileName = "CDN.cfg";
    private static Dictionary<string, string> cdnLookup;

    public static Dictionary<string, string> CdnLookup
    {
      get
      {
        if (cdnLookup != null) 
          return cdnLookup;

        if (File.Exists(CdnConfigurationFileName))
        {
          cdnLookup = (from line in File.ReadAllLines(CdnConfigurationFileName)
            where !line.StartsWith("#")
            let elements = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
            where elements.Length == 2
            select elements).ToDictionary(e => e[0], e => e[1]);
        }
        else
        {
          cdnLookup = new Dictionary<string, string>();
        }

        return cdnLookup;
      }
      set
      {
        cdnLookup = value;
      }
    }

    public static string RersolveHosts(string page)
    {
      if (!File.Exists(CdnConfigurationFileName))
        return page;

      var lines = new List<string>();

      var script = new Regex("<script.*src=\"scripts/(.*\\.js)\".*", RegexOptions.Compiled);
      var css = new Regex("<link.*href=\"css/(.*\\.css)\".*", RegexOptions.Compiled);

      foreach (var line in page.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
      {
        var isScriptSource = script.Match(line);
        if (isScriptSource.Success)
        {
          var source = isScriptSource.Groups[1].Value;
          lines.Add(!CdnLookup.ContainsKey(source)
            ? line
            : isScriptSource.Groups[0].ToString().Replace(source, CdnLookup[source]));
          continue;
        }

        var isCssSource = css.Match(line);
        if (isCssSource.Success)
        {
          var source = isCssSource.Groups[1].Value;
          lines.Add(!CdnLookup.ContainsKey(source)
            ? line
            : isCssSource.Groups[0].ToString().Replace(source, CdnLookup[source]));
          continue;
        }

        lines.Add(line);
      }

      return string.Join(Environment.NewLine, lines);
    }
  }
}