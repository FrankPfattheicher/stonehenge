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

        public void Init(string appFilesPath, string appTitle, string rootPage)
        {
            aureliaContent = new Dictionary<string, Resource>();

            var appCreator = new AureliaAppCreator(appTitle, rootPage, aureliaContent);

            AddFileSystemContent(appFilesPath);
            AddResourceContent();
            appCreator.CreateApplication();
            appCreator.CreateControllers();
            appCreator.CreateElements();
        }

        public void Dispose()
        {
            aureliaContent.Clear();
        }

        private static readonly Regex ExtractName = new Regex("<!--ViewModel:(\\w+)-->");
        private static readonly Regex ExtractElement = new Regex("<!--CustomElement(:([\\w, ]+))?-->");
        private static readonly Regex ExtractTitle = new Regex("<!--Title:([^:]+)(:(\\d+))?-->");

        private static ViewModelInfo GetViewModelInfo(string route, string pageText)
        {
            route = route.Substring(0, 1).ToUpper() + route.Substring(1);
            var info = new ViewModelInfo(route + "Vm");

            var match = ExtractElement.Match(pageText);
            if (match.Success)
            {
                info.ElementName = route;
                if (!string.IsNullOrEmpty(match.Groups[2].Value))
                {
                    info.Bindings = match.Groups[2].Value
                        .Split(',')
                        .Select(b => b.Trim())
                        .ToList();
                }
                info.VmName = null;
                info.SortIndex = 0;
            }
            else
            {
                match = ExtractName.Match(pageText);
                if (match.Success)
                {
                    info.VmName = match.Groups[1].Value;
                }
                match = ExtractTitle.Match(pageText);
                if (match.Success)
                {
                    info.Title = match.Groups[1].Value;
                    if (!string.IsNullOrEmpty(match.Groups[3].Value))
                    {
                        info.SortIndex = int.Parse(match.Groups[3].Value);
                    }
                }
                else
                {
                    info.Title = route;
                }
            }
            return info;
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

                    var resource = new Resource(route, appFile, ResourceType.Html, pageText, Resource.Cache.OneDay) { ViewModel = GetViewModelInfo(route, pageText) };
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

                    var resource = new Resource(route, "res://" + resourceName, ResourceType.Html, pageText, Resource.Cache.Revalidate)
                    {
                        ViewModel = GetViewModelInfo(route, pageText)
                    };
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
