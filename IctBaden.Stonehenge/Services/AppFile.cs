using System.IO;

namespace IctBaden.Stonehenge
{
  public class AppFile
  {
    public AppFile()
    {
      Path1 = "App";
      Path2 = string.Empty;
      Path3 = string.Empty;
    }

    public string Path1 { get; set; }
    public string Path2 { get; set; }
    public string Path3 { get; set; }
    public string FileName { get; set; }

    public string BasePath(string root)
    {
      if (Path2 == "viewmodels")
        Path2 = string.Empty;
      return Path.Combine(root, Path1, Path2, Path3);
    }
    public string FullPath(string root)
    {
      return Path.Combine(BasePath(root), FileName);
    }
  }
}