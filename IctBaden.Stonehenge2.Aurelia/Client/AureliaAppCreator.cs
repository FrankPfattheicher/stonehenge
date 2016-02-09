using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IctBaden.Stonehenge2.Resources;

namespace IctBaden.Stonehenge2.Aurelia.Client
{
    internal class AureliaAppCreator
    {
        private readonly string rootPage;
        private readonly Dictionary<string, Resource> aureliaContent;

        public AureliaAppCreator(string rootPage, Dictionary<string, Resource> aureliaContent)
        {
            this.rootPage = rootPage;
            this.aureliaContent = aureliaContent;
        }

        private static string LoadResourceText(string resourceName)
        {
            var assembly = Assembly.GetAssembly(typeof(AureliaAppCreator));
            var resourceText = string.Empty;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        resourceText = reader.ReadToEnd();
                    }
                }
            }

            return resourceText;
        }

        public string CreateApplicationJs()
        {
            var applicationJs = LoadResourceText("IctBaden.Stonehenge2.Aurelia.Client.stonehengeApp.js");

            applicationJs = InsertRoutes(applicationJs);

            return applicationJs;
        }

        private string InsertRoutes(string pageText)
        {
            const string routesInsertPoint = "//stonehengeAppRoutes";
            const string stonehengeAppTitleInsertPoint = "stonehengeAppTitle";
            const string pageTemplate = "{{ route: [{0}'{1}'], name: '{1}', moduleId: './{2}', nav: true, title:'{2}' }}";
            
            var pages = aureliaContent
                .Select(res => string.Format(pageTemplate, res.Value.Name == rootPage ? "''," : "",
                res.Value.Name, res.Value.ExtProperty));

            var routes = string.Join("," + Environment.NewLine, pages);
            pageText = pageText
                .Replace(routesInsertPoint, routes)
                .Replace(stonehengeAppTitleInsertPoint, "stonehenge");

            return pageText;
        }


    }
}