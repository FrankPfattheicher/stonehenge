using System.Collections.Generic;

namespace IctBaden.Stonehenge
{
  // see http://flot.googlecode.com/svn/trunk/API.txt

  // ReSharper disable InconsistentNaming
  public class GraphAxisOptions
  {
    public string show { get; set; }

    public string position { get; set; }

    public string mode { get; set; }
    public string timeformat { get; set; }

    public string color { get; set; }
    public string tickColor { get; set; }
    public string font { get; set; }

    public string min { get; set; }
    public string max { get; set; }
    public string autoscaleMargin { get; set; }

    public string transform { get; set; }
    public string inverseTransform { get; set; }

    public string ticks { get; set; }
    public string tickSize { get; set; }
    public string minTickSize { get; set; }
    public string tickFormatter { get; set; }
    public string tickDecimals { get; set; }

    public string labelWidth { get; set; }
    public string labelHeight { get; set; }
    public string reserveSpace { get; set; }

    public string tickLength { get; set; }

    public string alignTicksWithAxis { get; set; }

    public GraphAxisOptions()
    {
      show = "true";
    }
  }

  public class GraphLinesOptions
  {
    public string show { get; set; }
    public string lineWidth { get; set; }
    public string fill { get; set; }
    public string fillColor { get; set; }
    public string steps { get; set; }

    public GraphLinesOptions()
    {
      show = "true";
    }
  }

  public class GraphPointsOptions
  {
    public string show { get; set; }

    public GraphPointsOptions()
    {
      show = "true";
    }
  }

  public class GraphGridOptions
  {
    public string show { get; set; }
    public string aboveData { get; set; }
    public string color  { get; set; }
    public string backgroundColor  { get; set; }
    public string margin  { get; set; }
    public string labelMargin  { get; set; }
    public string axisMargin  { get; set; }
    public string markings  { get; set; }
    public string borderWidth  { get; set; }
    public string borderColor  { get; set; }
    public string minBorderMargin  { get; set; }
    public string clickable  { get; set; }
    public string hoverable  { get; set; }
    public string autoHighlight  { get; set; }
    public string mouseActiveRadius { get; set; }
  }

  public class GraphSeriesOptions
  {
    public GraphLinesOptions lines { get; set; }
    public GraphPointsOptions points { get; set; }
  }

  public class GraphLegendOptions
  {
    public string show { get; set; }
    public string labelFormatter { get; set; }
    public string labelBoxBorderColor { get; set; }
    public string noColumns { get; set; }
    public string position { get; set; }
    public string margin { get; set; }
    public string backgroundColor { get; set; }
    public string backgroundOpacity { get; set; }
    public string container { get; set; }

    public GraphLegendOptions()
    {
      show = "true";
    }
  }

  public struct GraphOptions
  {
    public GraphAxisOptions xaxis { get; set; }
    public GraphAxisOptions yaxis { get; set; }

    public string colors { get; set; }

    public GraphGridOptions grid { get; set; }
    public GraphSeriesOptions series { get; set; }
    public GraphLegendOptions legend { get; set; }

    private static string ToJson(object option)
    {
      var properties = new List<string>();

      foreach (var property in option.GetType().GetProperties())
      {
        var value = property.GetValue(option, null);
        if (value == null)
          continue;

        if (property.PropertyType.IsClass && (property.PropertyType != typeof(string)))
        {
          properties.Add('"' + property.Name + '"' + ": {" + ToJson(value) + "}");
        }
        else
        {
          properties.Add(string.Format("{0}: {1}", '"' + property.Name + '"', value.ToString()));
        }
      }

      return string.Join(",", properties);
    }

    public override string ToString()
    {
      return "{" + ToJson(this) + "}";
    }

    public static GraphOptions Parse(string json)
    {
      return new GraphOptions();
    }
  }

  // ReSharper restore InconsistentNaming
}