namespace IctBaden.Stonehenge2.Angular
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using IctBaden.Stonehenge2.Angular.Client;
    using IctBaden.Stonehenge2.Core;
    using IctBaden.Stonehenge2.Resources;

    public class AngularResourceProvider : IResourceProvider
    {
        private Dictionary<string, Resource> angularContent;

        public void Init(string appFilesPath, string rootPage)
        {
            angularContent = new Dictionary<string, Resource>();

            AddFileSystemContent(appFilesPath);
            AddResourceContent();
            AddAppJs(rootPage);
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

                    var resource = new Resource(route, appFile, ResourceType.Html, pageText) { ExtProperty = GetViewModelName(route, pageText) };
                    angularContent.Add(resourceId, resource);
                }
            }
        }

        private void AddResourceContent()
        {
            var assembly = Assembly.GetEntryAssembly();
            var baseName = assembly.GetName().Name + ".app.";

            foreach (var resourceName in assembly.GetManifestResourceNames()
              .Where(name => (name.EndsWith(".html")) && (!name.Contains("index.html"))).OrderBy(name => name))
            {
                var resourceId = resourceName.Substring(baseName.Length);
                if (angularContent.ContainsKey(resourceId))
                {
                    Trace.TraceWarning("AngularResourceProvider.AddResourceContent: Resource with id {0} already exits", resourceId);
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

                var resource = new Resource(route, "res://" + resourceName, ResourceType.Html, pageText) { ExtProperty = GetViewModelName(route, pageText) };
                angularContent.Add(resourceId, resource);
            }
        }

        private void AddAppJs(string rootPage)
        {
            var appCreator = new AngularAppCreator(rootPage, angularContent);
            var resource = new Resource("stonehengeApp.js", "AngularResourceProvider", ResourceType.Html,
                appCreator.CreateApplicationJs());
            angularContent.Add("stonehengeApp.js", resource);
        }


        public Resource Post(AppSession session, string resourceName, object[] postParams)
        {
            return null;
        }
        public Resource Get(AppSession session, string resourceName)
        {
            if (angularContent.ContainsKey(resourceName))
            {
                return angularContent[resourceName];
            }
            return null;
        }
    }
}
