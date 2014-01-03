using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IctBaden.Stonehenge.Services
{
  internal static class ResourceLoader
  {
    private static readonly Dictionary<string, string> Texts = new Dictionary<string, string>();
    private static readonly Dictionary<string, byte[]> Binaries = new Dictionary<string, byte[]>();

    public static string LoadText(string filePath, string resourcePath, string name)
    {
      resourcePath = resourcePath.Replace('-', '_');
      var resourceName1 = (resourcePath.Replace(Path.DirectorySeparatorChar, '.') + "." + name).Replace("..", ".");

      if (Texts.ContainsKey(resourceName1))
      {
        return Texts[resourceName1];
      }

      var resourceName2 = string.Empty;
      var ext = Path.GetExtension(name);
      if ((ext == ".js") || (ext == ".css"))
      {
        resourcePath += ext;
        resourceName2 = (resourcePath.Replace(Path.DirectorySeparatorChar, '.') + "." + name).Replace("..", ".");
      }

      if (!string.IsNullOrEmpty(resourceName2) && Texts.ContainsKey(resourceName2))
      {
        return Texts[resourceName2];
      }

      string text;

      var fullPath = Path.Combine(filePath, name);
      if (File.Exists(fullPath))
      {
        text = File.ReadAllText(fullPath);
        Texts.Add(resourceName1, text);
        return text;
      }

      var assemblies = new List<Assembly> {Assembly.GetEntryAssembly(), Assembly.GetExecutingAssembly()};
      foreach (var assembly in assemblies)
      {
        using (var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + resourceName1))
        {
          if (stream != null)
          {
            using (var reader = new StreamReader(stream))
            {
              text = reader.ReadToEnd();
              Texts.Add(resourceName1, text);
              return text;
            }
          }
        }

        if(string.IsNullOrEmpty(resourceName2))
          continue;

        using (var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + resourceName2))
        {
          if (stream != null)
          {
            using (var reader = new StreamReader(stream))
            {
              text = reader.ReadToEnd();
              Texts.Add(resourceName1, text);
              return text;
            }
          }
        }
      }
      var names = assemblies[1].GetManifestResourceNames().OrderBy(n => n).ToList();
      var libname = names.FirstOrDefault(n => n.EndsWith(name))?? names.FirstOrDefault(n => n.Contains("lib." + name));
      //libname = names.FirstOrDefault(n => n.Contains("lib." + name)) ?? names.FirstOrDefault(n => n.EndsWith(name));
      if (libname != null)
      {
        using (var stream = assemblies[1].GetManifestResourceStream(libname))
        {
          if (stream != null)
          {
            using (var reader = new StreamReader(stream))
            {
              text = reader.ReadToEnd();
              Texts.Add(resourceName1, text);
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

      var assemblies = new List<Assembly> {Assembly.GetEntryAssembly(), Assembly.GetExecutingAssembly()};
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
