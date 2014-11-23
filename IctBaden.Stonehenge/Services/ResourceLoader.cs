using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IctBaden.Stonehenge.Services
{
    public static class ResourceLoader
    {
        private static readonly Dictionary<string, string> Texts = new Dictionary<string, string>();
        private static readonly Dictionary<string, byte[]> Binaries = new Dictionary<string, byte[]>();

        private static bool IsName(this string name, string name1, string name2)
        {
            return (string.Compare(name, name1, StringComparison.InvariantCultureIgnoreCase) == 0) ||
                   (string.Compare(name, name2, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public static string LoadText(string filePath, string resourcePath, string name)
        {
            resourcePath = resourcePath.Replace('-', '_');
            var resourceName1 = (resourcePath.Replace(Path.DirectorySeparatorChar, '.') + "." + name).Replace("..", ".");

            if (Texts.ContainsKey(resourceName1))
            {
                return Texts[resourceName1];
            }

            var resourceName2 = string.Empty;
            var fullPath = string.Empty;
            string ext;
            try
            {
                ext = Path.GetExtension(name);
                fullPath = Path.Combine(filePath, name);
            }
            catch (Exception)
            {
                ext = string.Empty;
            }
            if ((ext == ".js") || (ext == ".css"))
            {
                resourcePath += ext;
                resourceName2 = (resourcePath.Replace(Path.DirectorySeparatorChar, '.') + "." + name).Replace("..", ".");
            }

            if (!string.IsNullOrEmpty(resourceName2) && Texts.ContainsKey(resourceName2))
            {
                return Texts[resourceName2];
            }

            lock (Texts)
            {
                string text;

                if (File.Exists(fullPath))
                {
                    text = File.ReadAllText(fullPath);
                    if (!Texts.ContainsKey(resourceName1))
                    {
                        Texts.Add(resourceName1, text);
                    }
                    return text;
                }

                var assemblies = new List<Assembly> { Assembly.GetEntryAssembly(), Assembly.GetExecutingAssembly() };
                foreach (var assembly in assemblies.Where(a => a != null))
                {
					var assemblyResourceName1 = assembly.GetName().Name + "." + resourceName1;
					var assemblyResourceName2 = assembly.GetName().Name + "." + resourceName2;
					var realName = assembly.GetManifestResourceNames().FirstOrDefault(n => n.IsName(assemblyResourceName1, assemblyResourceName2));
					if(realName == null)
						continue;
                    using (var stream = assembly.GetManifestResourceStream(realName))
                    {
                        if (stream != null)
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                text = reader.ReadToEnd();
                                if (!Texts.ContainsKey(resourceName1))
                                {
                                    Texts.Add(resourceName1, text);
                                }
                                return text;
                            }
                        }
                    }
                }
                var names = assemblies[1].GetManifestResourceNames().OrderBy(n => n).ToList();
                var libname = names.FirstOrDefault(n => n.EndsWith(name)) ?? names.FirstOrDefault(n => n.Contains("lib." + name));
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
                                if (!Texts.ContainsKey(resourceName1))
                                {
                                    Texts.Add(resourceName1, text);
                                }
                                return text;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static byte[] LoadBinary(string filePath, string resourcePath, string name)
        {
            var resourceName = (resourcePath.Replace(Path.DirectorySeparatorChar, '.') + "." + name).Replace("..", ".");
            if (!resourceName.StartsWith("app."))
            {
                resourceName = ("app." + resourceName).Replace("..", "."); ;
            }

            if (Binaries.ContainsKey(resourceName))
            {
                return Binaries[resourceName];
            }

            lock (Binaries)
            {
                byte[] data;

                var fullPath = Path.Combine(filePath, name);
                if (File.Exists(fullPath))
                {
                    data = File.ReadAllBytes(fullPath);
                    if (!Binaries.ContainsKey(resourceName))
                    {
                        Binaries.Add(resourceName, data);
                    }
                    return data;
                }

                var assemblies = new List<Assembly> { Assembly.GetEntryAssembly(), Assembly.GetExecutingAssembly() };
                foreach (var assembly in assemblies)
                {
                    var assemblyResourceName = assembly.GetName().Name + "." + resourceName;
                    var realName = assembly.GetManifestResourceNames().FirstOrDefault(n => n.IsName(assemblyResourceName, string.Empty));
                    if (realName == null)
                        continue;

                    using (var stream = assembly.GetManifestResourceStream(realName))
                    {
                        if (stream == null)
                            continue;

                        using (var reader = new BinaryReader(stream))
                        {
                            data = reader.ReadBytes((int)stream.Length);
                            if (!Binaries.ContainsKey(resourceName))
                            {
                                Binaries.Add(resourceName, data);
                            }
                            return data;
                        }
                    }
                }
            }
            return null;
        }

    }
}
