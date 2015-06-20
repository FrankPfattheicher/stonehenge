namespace IctBaden.Stonehenge2.Angular.Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using IctBaden.Stonehenge2.Resources;
    using IctBaden.Stonehenge2.ViewModel;

    public class AngularAppCreator
    {
        private readonly string rootPage;
        private readonly Dictionary<string, Resource> angularContent;

        public AngularAppCreator(string rootPage, Dictionary<string, Resource> angularContent)
        {
            this.rootPage = rootPage;
            this.angularContent = angularContent;
        }

        public string CreateApplicationJs()
        {
            const string AppJsResourceName = "IctBaden.Stonehenge2.Angular.Client.stonehengeApp.js";
            var assembly = Assembly.GetAssembly(GetType());
            var applicationJs = string.Empty;

            using (var stream = assembly.GetManifestResourceStream(AppJsResourceName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        applicationJs = reader.ReadToEnd();
                    }
                }
            }

            applicationJs = InsertRoutes(applicationJs);
            applicationJs += GetControllers();

            return applicationJs;
        }

        private string InsertRoutes(string pageText)
        {
            const string RoutesInsertPoint = "//stonehengeAppRoutes";
            const string RootPageInsertPoint = "stonehengeRootPage";
            const string PageTemplate = "when('/{0}', {{ templateUrl: '{0}.html', controller: '{1}' }}).";

            var pages = angularContent
                .Select(res => string.Format(PageTemplate, res.Value.Name, res.Value.ExtProperty));

            var routes = string.Join(Environment.NewLine, pages);
            pageText = pageText
                .Replace(RoutesInsertPoint, routes)
                .Replace(RootPageInsertPoint, rootPage);
            return pageText;
        }

        private string GetControllers()
        {
            var controllerTexts = angularContent
                .Select(res => res.Value.ExtProperty)
                .Distinct()
                .Select(GetController);

            return string.Join(Environment.NewLine, controllerTexts);
        }


        private static Type GetVmType(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(type => type.Name == name);
        }

        private static string GetController(string vmName)
        {
            const string ControllerTemplate =
@"stonehengeApp.controller('{0}', ['$scope', '$http',
  function ($scope, $http) {
      /*commands*/
      $http.get('ViewModel/{0}.json').
        success(function (data, status, headers, config) {
            angular.extend($scope, data);
        }).
        error(function (data, status, headers, config) {
            debugger;
        });

  }]);
";

            var text = ControllerTemplate.Replace("{0}", vmName);

            var vmType = GetVmType(vmName);

            const string MethodTemplate =
@"$scope.{1} = function({paramNames}) {
    /*postData*/
    $http.post('/ViewModel/{0}/{1}{paramValues}', $scope.GetStonehengePostData($scope)).
    success(function(data, status, headers, config) {
      angular.extend($scope, data);
    }).
    error(function(data, status, headers, config) {
      debugger;
    });
  }
";
            var postbackPropNames = GetPostbackPropNames(vmType).Select(name => "'" + name + "'");

            var postData = @"$scope.GetStonehengePostData = function(scope) {" + Environment.NewLine;
            postData += "  var props = [" + string.Join(",", postbackPropNames) + "];" + Environment.NewLine;
            postData += "  var formData = '';" + Environment.NewLine;
            postData += "  props.forEach(function (prop) {" + Environment.NewLine;
            postData += "    formData += prop+'='+encodeURIComponent(JSON.stringify(scope[prop]))+'&';" + Environment.NewLine;
            postData += "  });" + Environment.NewLine;
            postData += "  return formData;" + Environment.NewLine;
            postData += "}" + Environment.NewLine;

            // supply functions for action methods
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

                var method = MethodTemplate
                    .Replace("{0}", vmName)
                    .Replace("{1}", methodInfo.Name)
                    .Replace("/*postData*/", postData)
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
            var viewModel = Activator.CreateInstance(vmType);
            var activeVm = viewModel as ActiveViewModel;
            if (activeVm != null)
            {
                vmProps.AddRange(from PropertyDescriptor prop in activeVm.GetProperties() select prop);
            }
            else
            {
                vmProps.AddRange(TypeDescriptor.GetProperties(viewModel, true).Cast<PropertyDescriptor>());
            }

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

                if ((prop == null) || (prop.GetSetMethod(false) == null)) // not public writable
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