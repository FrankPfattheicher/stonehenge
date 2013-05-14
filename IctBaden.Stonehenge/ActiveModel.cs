namespace IctBaden.Stonehenge
{
  public class ActiveModel
  {
    public readonly string Prefix;
    public readonly string TypeName;
    public object Model;

    public ActiveModel(string prefix, object model)
    {
      Prefix = prefix;
      Model = model;
      TypeName = model.GetType().Name;
    }
  }
}