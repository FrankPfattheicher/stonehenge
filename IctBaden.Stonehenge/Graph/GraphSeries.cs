namespace IctBaden.Stonehenge.Graph
{
  public class GraphSeries
  {
    public object label { get; set; }
    public long[][] data { get; set; }
    public object xaxis { get; set; }
    public object yaxis { get; set; }
    public GraphLinesOptions lines { get; set; }
    public object bars { get; set; }
    public GraphPointsOptions points { get; set; }
    public object clickable { get; set; }
    public object hoverable { get; set; }
    public object shadowSize { get; set; }
    public object highlightColor { get; set; }

    public GraphSeries()
    {
      xaxis = 1;
      yaxis = 1;
    }

    public override string ToString()
    {
      return GraphOptions.AsJsonValue(this);
    }

  }
}