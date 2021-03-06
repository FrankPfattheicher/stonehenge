﻿namespace IctBaden.Stonehenge.Test
{
    using IctBaden.Stonehenge.Graph;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class FlotTests
  {
    [TestMethod]
    public void SerializeGraphSeries()
    {
      var series = new GraphSeries
      {
        data = new[] { new[] { 0L, 0L }, new[] { 1L, 1L } },
        clickable = false,
        hoverable = true,
        xaxis = 2,
        yaxis = 3,
        lines = new GraphLinesOptions
        {
          show = true,
          lineWidth = 2,
          steps = false,
          fill = true,
          fillColor = new GraphColors { colors = new[] { new GraphOpacity { opacity = 0.6 }, new GraphOpacity { opacity = 0.2 } } }
        },
        points = new GraphPointsOptions
        {
          show = true
        },
      };

      string expected = "{\"data\":[[0,0],[1,1]],\"xaxis\":2,\"yaxis\":3,\"lines\":{\"show\":true,\"lineWidth\":2,\"fill\":true,\"fillColor\":{\"colors\":[{\"opacity\":0.6},{\"opacity\":0.2}]},\"steps\":false},\"points\":{\"show\":true},\"clickable\":false,\"hoverable\":true}";
      var json = series.ToString();
      Assert.AreEqual(expected, json);

      expected = expected.Replace("opacity\":0.6", "opacity\":0.55");
      ((object[])((GraphColors)series.lines.fillColor).colors)[0] = new GraphOpacity { opacity = 0.55 };
      json = series.ToString();
      Assert.AreEqual(expected, json);
    }


  }
}
