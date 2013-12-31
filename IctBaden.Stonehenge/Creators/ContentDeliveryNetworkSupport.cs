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

      var script = new Regex("(?<a><script.*src=\"(?<b>scripts/(?<c>.*\\.js))\".*)|(?<a><link.*href=\"(?<b>css/(?<c>.*\\.css))\".*)", RegexOptions.Compiled);

      var resultlines = from line in page.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                        let isScriptSource = script.Match(line)
                        let source = isScriptSource.Groups["c"].Value
                        select (isScriptSource.Success && CdnLookup.ContainsKey(source)) ? 
                               isScriptSource.Groups["a"].Value.Replace(isScriptSource.Groups["b"].Value, CdnLookup[source]) : line;

      return string.Join(Environment.NewLine, resultlines);
    }
  }
}