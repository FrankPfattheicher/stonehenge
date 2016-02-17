using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using IctBaden.Stonehenge2.Aurelia.Client;
using IctBaden.Stonehenge2.Core;
using IctBaden.Stonehenge2.Resources;

namespace IctBaden.Stonehenge2.Aurelia
{
    public class AureliaResourceProvider : IStonehengeResourceProvider
    {
        private Dictionary<string, Resource> aureliaContent;

        public void Init(string appFilesPath, string rootPage)
        {
            aureliaContent = new Dictionary<string, Resource>();

            var appCreator = new AureliaAppCreator(rootPage, aureliaContent);

            AddFileSystemContent(appFilesPath);
            AddResourceContent();
            appCreator.CreateApplication();
            appCreator.CreateControllers();
        }

        public void Dispose()
        {
            aureliaContent.Clear();
        }

        private static string GetViewModelName(string route, string pageText)
        {
            var name = route.Substring(0, 1).ToUpper() + route.Substring(1) + "Vm";

            var extractName = new Regex("ng-controller=\\\"(\\w+)\\\"");
            var match = extractName.Match(pageText);
            if (match.Success)
            {
                name = match.Groups[1].Value;
            }

            return name;
        }

        private void AddFileSystemContent(string appFilesPath)
        {
            if (Directory.Exists(appFilesPath))
            {
                var appPath = Path.DirectorySeparatorChar + "app" + Path.DirectorySeparatorChar;
                var appFiles = Directory.GetFiles(appFilesPath, "*.html", SearchOption.AllDirectories);

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var appFile in appFiles)
                {
                    var resourceId = appFile.Substring(appFile.IndexOf(appPath, StringComparison.InvariantCulture) + 5).Replace('\\', '/');
                    var route = resourceId.Replace(".html", string.Empty);
                    var pageText = File.ReadAllText(appFile);

                    var resource = new Resource(route, appFile, ResourceType.Html, pageText) { ViewModelName = GetViewModelName(route, pageText) };
                    aureliaContent.Add(resourceId, resource);
                }
            }
        }

        private void AddResourceContent()
        {
            var assemblies = new List<Assembly> { Assembly.GetEntryAssembly(), Assembly.GetAssembly(GetType()) };

            foreach (var assembly in assemblies)
            {
                var baseName = assembly.GetName().Name + ".app.";

                foreach (var resourceName in assembly.GetManifestResourceNames()
                  .Where(name => (name.EndsWith(".html")) && !name.Contains("index.html") && !name.Contains("src.app.html"))
                  .OrderBy(name => name))
                {
                    var resourceId = resourceName
                        .Substring(baseName.Length)
                        .Replace("@", "_")
                        .Replace("-", "_")
                        .Replace("._0", ".0")
                        .Replace("._1", ".1")
                        .Replace("._2", ".2")
                        .Replace("._3", ".3")
                        .Replace("._4", ".4")
                        .Replace("._5", ".5")
                        .Replace("._6", ".6")
                        .Replace("._7", ".7")
                        .Replace("._8", ".8")
                        .Replace("._9", ".9");
                    if (aureliaContent.ContainsKey(resourceId))
                    {
                        Trace.TraceWarning("AureliaResourceProvider.AddResourceContent: Resource with id {0} already exits", resourceId);
                        continue;
                    }

                    var route = resourceId.Replace(".html", string.Empty);
                    var pageText = "";
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                pageText = reader.ReadToEnd();
                            }
                        }
                    }

                    var resource = new Resource(route, "res://" + resourceName, ResourceType.Html, pageText) { ViewModelName = GetViewModelName(route, pageText) };
                    aureliaContent.Add("src." + resourceId, resource);
                }
            }
        }





        public Resource Post(AppSession session, string resourceName, object[] postParams, Dictionary<string, string> formData)
        {
            return null;
        }
        public Resource Get(AppSession session, string resourceName)
        {
            resourceName = resourceName.Replace("/", ".").Replace("@", "_").Replace("-", "_");
            if (aureliaContent.ContainsKey(resourceName))
            {
                return aureliaContent[resourceName];
            }
            return null;
        }
    }
}
