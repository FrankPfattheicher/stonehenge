using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace IctBaden.Stonehenge
{
  // see http://flot.googlecode.com/svn/trunk/API.txt

  // ReSharper disable InconsistentNaming
  public class GraphAxisOptions
  {
    public object show { get; set; }

    public object position { get; set; }

    public object mode { get; set; }
    public object timeformat { get; set; }

    public object color { get; set; }
    public object tickColor { get; set; }
    public object font { get; set; }

    public object min { get; set; }
    public object max { get; set; }
    public object autoscaleMargin { get; set; }

    public object transform { get; set; }
    public object inverseTransform { get; set; }

    public object ticks { get; set; }
    public object tickSize { get; set; }
    public object minTickSize { get; set; }
    public object tickFormatter { get; set; }
    public object tickDecimals { get; set; }

    public object labelWidth { get; set; }
    public object labelHeight { get; set; }
    public object reserveSpace { get; set; }

    public object tickLength { get; set; }

    public object alignTicksWithAxis { get; set; }

    public GraphAxisOptions()
    {
      show = true;
    }
  }

  public class GraphLinesOptions
  {
    public object show { get; set; }
    public object lineWidth { get; set; }
    public object fill { get; set; }
    public object fillColor { get; set; }
    public object steps { get; set; }

    public GraphLinesOptions()
    {
      show = true;
    }
  }

  public class GraphPointsOptions
  {
    public object show { get; set; }

    public GraphPointsOptions()
    {
      show = true;
    }
  }

  public class GraphGridOptions
  {
    public object show { get; set; }
    public object aboveData { get; set; }
    public object color { get; set; }
    public object backgroundColor { get; set; }
    public object margin { get; set; }
    public object labelMargin { get; set; }
    public object axisMargin { get; set; }
    public object markings { get; set; }
    public object borderWidth { get; set; }
    public object borderColor { get; set; }
    public object minBorderMargin { get; set; }
    public object clickable { get; set; }
    public object hoverable { get; set; }
    public object autoHighlight { get; set; }
    public object mouseActiveRadius { get; set; }
  }

  public class GraphSeriesOptions
  {
    public GraphLinesOptions lines { get; set; }
    public GraphPointsOptions points { get; set; }
  }

  public class GraphLegendOptions
  {
    public object show { get; set; }
    public object labelFormatter { get; set; }
    public object labelBoxBorderColor { get; set; }
    public object noColumns { get; set; }
    public object position { get; set; }
    public object margin { get; set; }
    public object backgroundColor { get; set; }
    public object backgroundOpacity { get; set; }
    public object container { get; set; }

    public GraphLegendOptions()
    {
      show = true;
    }
  }

  public class GraphColors
  {
    public object colors { get; set; }
  }
  public class GraphOpacity
  {
    public object opacity { get; set; }
  }

  public class GraphOptions
  {
    public GraphAxisOptions xaxis { get; set; }
    public GraphAxisOptions yaxis { get; set; }
    public GraphAxisOptions[] xaxes { get; set; }
    public GraphAxisOptions[] yaxes { get; set; }

    public object colors { get; set; }

    public GraphGridOptions grid { get; set; }
    public GraphSeriesOptions series { get; set; }
    public GraphLegendOptions legend { get; set; }

    internal static string AsJsonValue(object option)
    {
      var oType = option.GetType();
      if (oType == typeof(bool))
      {
        return option.ToString().ToLower();
      }
      if ((oType == typeof(float)) || (oType == typeof(double)))
      {
        return ((double)option).ToString(CultureInfo.InvariantCulture);
      }
      if (oType == typeof(string))
      {
        var txt = option.ToString();
        if (txt.StartsWith("{") || txt.StartsWith("["))
          return txt;
        return '\"' + option.ToString() + '\"';
      }
      if (oType.IsValueType)
      {
        return option.ToString();
      }

      var array = option as Array;
      if (array != null)
      {
        var elements = new List<string>();
        for (var ix = 0; ix < array.Length; ix++)
        {
          var av = array.GetValue(ix);
          elements.Add(AsJsonValue(av));
        }
        return "[" + string.Join(",", elements) + "]";
      }

      var properties = new List<string>();
      foreach (var property in oType.GetProperties())
      {
        try
        {
          var value = property.GetValue(option, null);
          if (value == null)
            continue;

          properties.Add('"' + property.Name + "\":" + AsJsonValue(value));
        }
        catch (Exception ex)
        {
          Debug.WriteLine("Could not serialize property {0}: {1}", property.Name, ex.Message);
        }
      }

      return '{' + string.Join(",", properties) + '}';
    }

    public override string ToString()
    {
      return AsJsonValue(this);
    }

    public static GraphOptions Parse(string json)
    {
      return new GraphOptions();
    }
  }

  // ReSharper restore InconsistentNaming
}