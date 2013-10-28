using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Profile;

namespace IctBaden.Stonehenge.Creators
{
  public static class ContentDeliveryNetworkSupport
  {
    private const string CdnConfigurationFileName = "CDN.cfg";
    private static Dictionary<string, string> cdnLookup;


    public static string RersolveHosts(string source)
    {
      if (!File.Exists(CdnConfigurationFileName))
        return source;

      if (cdnLookup == null)
      {
        cdnLookup = (from line in File.ReadAllLines(CdnConfigurationFileName)
                     where !line.StartsWith("#")
                     let elements = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                     where elements.Length == 2
                     select elements).ToDictionary(e => e[0], e => e[1]);
      }

      var lines = new List<string>();

      var reg = new Regex("<script type=\"text/javascript\" src=\"scripts/(.*\\.js)\"></script>", RegexOptions.Compiled);
      foreach (var line in source.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
      {
        var isScriptSource = reg.Match(line);
        if (!isScriptSource.Success)
        {
          lines.Add(line);
          continue;
        }
        var script = isScriptSource.Groups[1].Value;
        lines.Add(!cdnLookup.ContainsKey(script)
          ? line
          : string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", cdnLookup[script]));
      }

      return string.Join(Environment.NewLine, lines);
    }
  }
}