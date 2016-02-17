using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using IctBaden.Stonehenge2.Core;
using IctBaden.Stonehenge2.Resources;
using IctBaden.Stonehenge2.ViewModel;

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

        public void CreateApplication()
        {
            var applicationJs = LoadResourceText("IctBaden.Stonehenge2.Aurelia.Client.stonehengeApp.js");
            applicationJs = InsertRoutes(applicationJs);

            var resource = new Resource("src.app.js", "AureliaResourceProvider", ResourceType.Html, applicationJs);
            aureliaContent.Add("src.app.js", resource);
        }

        private string InsertRoutes(string pageText)
        {
            const string routesInsertPoint = "//stonehengeAppRoutes";
            const string stonehengeAppTitleInsertPoint = "stonehengeAppTitle";
            const string pageTemplate = "{{ route: [{0}'{1}'], name: '{1}', moduleId: './{1}', nav: true, title:'{1}' }}";
            
            var pages = aureliaContent
                .Select(res => string.Format(pageTemplate, res.Value.Name == rootPage ? "''," : "", res.Value.Name));

            var routes = string.Join("," + Environment.NewLine, pages);
            pageText = pageText
                .Replace(routesInsertPoint, routes)
                .Replace(stonehengeAppTitleInsertPoint, "stonehenge");

            return pageText;
        }


        public void CreateControllers()
        {
            var viewModels = aureliaContent
                .Where(res => !string.IsNullOrEmpty(res.Value.ViewModelName))
                .Select(res => res.Value)
                .Distinct()
                .ToList();

            foreach (var viewModel in viewModels)
            {
                var controllerJs = GetController(viewModel.ViewModelName);
                if (!string.IsNullOrEmpty(controllerJs))
                {
                    var resource = new Resource($"src.{viewModel.Name}.js", "AureliaResourceProvider", ResourceType.Js, controllerJs);
                    aureliaContent.Add(resource.Name, resource);
                }
            }
        }

        private static Type GetVmType(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(type => type.Name == name);
        }

        private static string GetController(string vmName)
        {
            var vmType = GetVmType(vmName);
            if (vmType == null)
            {
                Debug.Assert(false, "No VM for type " + vmName + " defined.");
                // ReSharper disable once HeuristicUnreachableCode
                return null;
            }

            var controllerTemplate = LoadResourceText("IctBaden.Stonehenge2.Aurelia.Client.stonehengeController.js");

            var text = controllerTemplate.Replace("{0}", vmName);

            var postbackPropNames = GetPostbackPropNames(vmType).Select(name => "'" + name + "'");
            text = text.Replace("'propNames'", string.Join(",", postbackPropNames));

            // supply functions for action methods
            const string methodTemplate = @"{1} = function({paramNames}) { this.StonehengePost(this, '/ViewModel/{0}/{1}{paramValues}'); }";

            var actionMethods = new StringBuilder();
            foreach (var methodInfo in vmType.GetMethods().Where(methodInfo => methodInfo.GetCustomAttributes(false).OfType<ActionMethodAttribute>().Any()))
            {
                //var method = (methodInfo.GetParameters().Length > 0)
                //  ? "%method%: function (data, event, param) { if(!IsLoading()) post_ViewModelName_Data(self, event.currentTarget, '%method%', param); },".Replace("%method%", methodInfo.Name)
                //  : "%method%: function (data, event) { if(!IsLoading()) post_ViewModelName_Data(self, event.currentTarget, '%method%', null); },".Replace("%method%", methodInfo.Name);

                var paramNames = methodInfo.GetParameters().Select(p => p.Name).ToArray();
                var paramValues = paramNames.Any()
                ? "?" + string.Join("&", paramNames.Select(n => string.Format("{0}='+encodeURIComponent({0})+'", n)))
                : string.Empty;

                var method = methodTemplate
                    .Replace("{0}", vmName)
                    .Replace("{1}", methodInfo.Name)
                    .Replace("{paramNames}", string.Join(",", paramNames))
                    .Replace("{paramValues}", paramValues)
                    .Replace("+''", string.Empty);

                actionMethods.AppendLine(method);
            }


            return text.Replace("/*commands*/", actionMethods.ToString());
        }

        private static List<string> GetPostbackPropNames(Type vmType)
        {
            // properties
            var vmProps = new List<PropertyDescriptor>();
            var sessionCtor = vmType.GetConstructors().FirstOrDefault(ctor => ctor.GetParameters().Length == 1);
            var session = new AppSession();
            var viewModel = (sessionCtor != null) ? Activator.CreateInstance(vmType, session) : Activator.CreateInstance(vmType);
            var activeVm = viewModel as ActiveViewModel;
            if (activeVm != null)
            {
                vmProps.AddRange(from PropertyDescriptor prop in activeVm.GetProperties() select prop);
            }
            else
            {
                vmProps.AddRange(TypeDescriptor.GetProperties(viewModel, true).Cast<PropertyDescriptor>());
            }
            var disposeVm = viewModel as IDisposable;
            disposeVm?.Dispose();

            var assignPropNames = (from prop in vmProps
                                   let bindable = prop.Attributes.OfType<BindableAttribute>().ToArray()
                                   where (bindable.Length <= 0) || bindable[0].Bindable
                                   select prop.Name).ToList();

            // do not send ReadOnly or OneWay bound properties back
            var postbackPropNames = new List<string>();
            foreach (var propName in assignPropNames)
            {
                var prop = vmType.GetProperty(propName);
                if (prop == null)
                {
                    if (activeVm == null)
                        continue;
                    prop = activeVm.GetPropertyInfo(propName);
                    if ((prop != null) && activeVm.IsPropertyReadOnly(propName))
                        continue;
                }

                if (prop?.GetSetMethod(false) == null) // not public writable
                    continue;
                var bindable = prop.GetCustomAttributes(typeof(BindableAttribute), true);
                if ((bindable.Length > 0) && ((BindableAttribute)bindable[0]).Direction == BindingDirection.OneWay)
                    continue;
                postbackPropNames.Add(propName);
            }

            return postbackPropNames;
        }

    }
}