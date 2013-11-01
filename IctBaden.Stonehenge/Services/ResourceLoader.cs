using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace IctBaden.Stonehenge.Services
{
  internal static class ResourceLoader
  {
    private static readonly Dictionary<string, string> Texts = new Dictionary<string, string>();
    private static readonly Dictionary<string, byte[]> Binaries = new Dictionary<string, byte[]>();

    public static string LoadText(string filePath, string resourcePath, string name)
    {
      var resourceName = (resourcePath.Replace(Path.DirectorySeparatorChar, '.') + "." + name).Replace("..", ".");

      if (Texts.ContainsKey(resourceName))
      {
        return Texts[resourceName];
      }

      string text;

      var fullPath = Path.Combine(filePath, name);
      if (File.Exists(fullPath))
      {
        text = File.ReadAllText(fullPath);
        Texts.Add(resourceName, text);
        return text;
      }

      var assemblies = new List<Assembly> {Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly()};
      foreach (var assembly in assemblies)
      {
        using (var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + resourceName))
        {
          if (stream != null)
          {
            using (var reader = new StreamReader(stream))
            {
              text = reader.ReadToEnd();
              Texts.Add(resourceName, text);
              return text;
            }
          }
        }
      }

      return null;
    }

    public static byte[] LoadBinary(string filePath, string resourcePath, string name)
    {
      var resourceName = (resourcePath.Replace('\\', '.') + "." + name).Replace("..", ".");

      if (Binaries.ContainsKey(resourceName))
      {
        return Binaries[resourceName];
      }

      byte[] data;

      var fullPath = Path.Combine(filePath, name);
      if (File.Exists(fullPath))
      {
        data = File.ReadAllBytes(fullPath);
        Binaries.Add(resourceName, data);
        return data;
      }

      var assemblies = new List<Assembly> {Assembly.GetExecutingAssembly(), Assembly.GetEntryAssembly()};
      foreach (var assembly in assemblies)
      {
        using (var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + resourceName))
        {
          if (stream != null)
          {
            using (var reader = new BinaryReader(stream))
            {
              data = reader.ReadBytes((int) stream.Length);
              Binaries.Add(resourceName, data);
              return data;
            }
          }
        }
      }

      return null;
    }

  }
}
