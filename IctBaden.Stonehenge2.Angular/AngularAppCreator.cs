namespace IctBaden.Stonehenge2.Angular
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

        private static string GetController(string name)
        {
            const string ControllerTemplate =
@"stonehengeApp.controller('{0}', ['$scope', '$http',
  function ($scope, $http) {

      /*commands*/

      $http.get('ViewModels/{0}.json').
        success(function (data, status, headers, config) {
            angular.extend($scope, data);
        }).
        error(function (data, status, headers, config) {
            debugger;
        });

  }]);
";

            var text = ControllerTemplate.Replace("{0}", name);

            var vmType = GetVmType(name);

            const string MethodTemplate =
@"$scope.{0} = function() {
    alert('{0}')
}
";

            // supply functions for action methods
            var actionMethods = new StringBuilder();
            foreach (var methodInfo in vmType.GetMethods().Where(methodInfo => methodInfo.GetCustomAttributes(false).OfType<ActionMethodAttribute>().Any()))
            {
                //var method = (methodInfo.GetParameters().Length > 0)
                //  ? "%method%: function (data, event, param) { if(!IsLoading()) post_ViewModelName_Data(self, event.currentTarget, '%method%', param); },".Replace("%method%", methodInfo.Name)
                //  : "%method%: function (data, event) { if(!IsLoading()) post_ViewModelName_Data(self, event.currentTarget, '%method%', null); },".Replace("%method%", methodInfo.Name);

                var method = MethodTemplate.Replace("{0}", methodInfo.Name);

                actionMethods.AppendLine(method);
            }

            return text.Replace("/*commands*/", actionMethods.ToString());
        }

    }
}