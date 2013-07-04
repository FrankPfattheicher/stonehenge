using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace IctBaden.Stonehenge.Services
{
  internal static class ResourceLoader
  {
    private const string BaseName = "IctBaden.Stonehenge.";
    private static readonly Dictionary<string, string> Texts = new Dictionary<string, string>();
    private static readonly Dictionary<string, byte[]> Binaries = new Dictionary<string, byte[]>();

    public static string LoadText(string filePath, string resourcePath, string name)
    {
      var resourceName = BaseName + resourcePath.Replace('\\', '.') + "." + name;

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

      var assembly = Assembly.GetExecutingAssembly();
      using (var stream = assembly.GetManifestResourceStream(resourceName))
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

      return null;
    }

    public static byte[] LoadBinary(string filePath, string resourcePath, string name)
    {
      var resourceName = BaseName + resourcePath.Replace('\\', '.') + "." + name;

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

      var assembly = Assembly.GetExecutingAssembly();
      using (var stream = assembly.GetManifestResourceStream(resourceName))
      {
        if (stream != null)
        {
          using (var reader = new BinaryReader(stream))
          {
            data = reader.ReadBytes((int)stream.Length);
            Binaries.Add(resourceName, data);
            return data;
          }
        }
      }

      return null;
    }

  }
}
