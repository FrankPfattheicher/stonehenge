using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IctBaden.Stonehenge.Creators
{
    internal static class UserLibs
    {
        private const string InsertPoint = "<!--UserLibs-->";
        private const string IncludeLibTemplate = "<script type=\"text/javascript\" src=\"app/lib/{0}\"></script>\r\n";
        private static readonly string LibPath = "app" + Path.DirectorySeparatorChar + "lib" + Path.DirectorySeparatorChar;
        private static string userLibs;

        public static void Init(string rootPath)
        {
            userLibs = string.Empty;

            // from files
            var libFilesPath = Path.Combine(rootPath, LibPath);
            if (Directory.Exists(libFilesPath))
            {
                var libFiles = Directory.GetFiles(libFilesPath, "*.js", SearchOption.AllDirectories);

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var libFile in libFiles)
                {
                    userLibs += string.Format(IncludeLibTemplate, libFile);
                }
            }

            // from resources
            var assembly = Assembly.GetEntryAssembly();
            var baseName = assembly.GetName().Name + ".app.lib.";
            foreach (var resourceName in assembly.GetManifestResourceNames()
              .Where(name => (name.StartsWith(baseName) && name.EndsWith(".js")))
              .OrderBy(name => name))
            {
                var libName = resourceName.Substring(baseName.Length);
                userLibs += string.Format(IncludeLibTemplate, libName);
            }

        }

        public static string InsertUserLibs(string rootPath, string text)
        {
            if (userLibs == null)
            {
                Init(rootPath);
            }
            return text.Replace(InsertPoint, userLibs);
        }
    }
}
