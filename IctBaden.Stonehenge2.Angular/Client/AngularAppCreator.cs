namespace IctBaden.Stonehenge2.Angular.Client
{
    using System;
    using System.Collections.Generic;
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

            const string RootTemplate = "when('/', {{ templateUrl: '{0}.html', controller: '{1}' }}).";
            const string PageTemplate = "when('/{0}', {{ templateUrl: '{0}.html', controller: '{1}' }}).";

            var pages =
                angularContent.Select(
                    res =>
                    string.Format(
                        (res.Value.Name == rootPage) ? RootTemplate : PageTemplate,
                        res.Value.Name,
                        res.Value.ExtProperty));

            var routes = string.Join(Environment.NewLine, pages);
            pageText = pageText.Replace(RoutesInsertPoint, routes);
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
@"$scope.{1} = function(p1, p2, p3) {
    $http.post('/ViewModel/{0}/{1}?p1=' + encodeURIComponent(p1) + '&p2=' + encodeURIComponent(p2) + '&p3=' + encodeURIComponent(p3), {2}).
  success(function(data, status, headers, config) {
    angular.extend($scope, data);
  }).
  error(function(data, status, headers, config) {
    debugger;
  });
}
";
            var postData = "{}";

            // supply functions for action methods
            var actionMethods = new StringBuilder();
            foreach (var methodInfo in vmType.GetMethods().Where(methodInfo => methodInfo.GetCustomAttributes(false).OfType<ActionMethodAttribute>().Any()))
            {
                //var method = (methodInfo.GetParameters().Length > 0)
                //  ? "%method%: function (data, event, param) { if(!IsLoading()) post_ViewModelName_Data(self, event.currentTarget, '%method%', param); },".Replace("%method%", methodInfo.Name)
                //  : "%method%: function (data, event) { if(!IsLoading()) post_ViewModelName_Data(self, event.currentTarget, '%method%', null); },".Replace("%method%", methodInfo.Name);

                var method = MethodTemplate
                    .Replace("{0}", vmName)
                    .Replace("{1}", methodInfo.Name)
                    .Replace("{2}", postData);

                actionMethods.AppendLine(method);
            }

            return text.Replace("/*commands*/", actionMethods.ToString());
        }

    }
}