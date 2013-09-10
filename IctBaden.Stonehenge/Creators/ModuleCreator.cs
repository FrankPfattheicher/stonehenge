using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IctBaden.Stonehenge.Creators
{
  public class ModuleCreator
  {
    private static string clientViewModelTemplate;
    private static string ClientViewModelTemplate
    {
      get
      {
        if (clientViewModelTemplate != null)
          return clientViewModelTemplate;

        const string resourceName = "IctBaden.Stonehenge.Creators.ClientViewModel.js";
        var assembly = Assembly.GetExecutingAssembly();

        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
          if (stream != null)
          {
            using (var reader = new StreamReader(stream))
            {
              clientViewModelTemplate = reader.ReadToEnd();
            }
          }
        }

        return clientViewModelTemplate;
      }
    }

    private static readonly Dictionary<Type, string> ViewModels = new Dictionary<Type, string>();

    public static string CreateFromViewModel(object viewModel)
    {
      if (viewModel == null)
      {
        var dummy = new StringBuilder();
        dummy.AppendLine("define(function (require) {");
        dummy.AppendLine("return {");
        dummy.AppendLine("};");
        dummy.AppendLine("});");
        return dummy.ToString();
      }

      var vmType = viewModel.GetType();

      if (ViewModels.ContainsKey(vmType))
      {
        return ViewModels[vmType];
      }

      // properties
      var vmProps = new List<PropertyDescriptor>();
      var activeVm = viewModel as ActiveViewModel;
      if (activeVm != null)
      {
        vmProps.AddRange(from PropertyDescriptor prop in activeVm.GetProperties() select prop);
      }
      else
      {
        vmProps.AddRange(TypeDescriptor.GetProperties(viewModel, true).Cast<PropertyDescriptor>());
      }

      var assignPropNames = (from prop in vmProps let bindable = prop.Attributes.OfType<BindableAttribute>().ToArray() 
                             where (bindable.Length <= 0) || bindable[0].Bindable select prop.Name).ToList();

      var setData = new StringBuilder();
      foreach (var propName in assignPropNames)
      {
        setData.AppendLine(string.Format("if(data.{0} != null) viewmodel.{0}(data.{0});", propName));
      }

      // plots
      foreach (var prop in vmProps.Where(p => (p.PropertyType == typeof(GraphSeries[]) && p.Name.EndsWith("Data"))))
      {
        var propName = prop.Name.Substring(0, prop.Name.Length - 4);  // remove "Data"
        setData.AppendLine(string.Format("if(data.{0}Data) {{ try {{ $.plot($('#{0}'), viewmodel.{0}Data(), viewmodel.{0}Options()); }} catch(e) {{ }} }}", propName));
        setData.AppendLine(string.Format("$('#{0}').resize(function() {{ try {{ $.plot($('#{0}'), viewmodel.{0}Data(), viewmodel.{0}Options()); }} catch(e) {{ }} }});", propName));
      }

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
        }
        if (!prop.CanWrite)
          continue;
        var bindable = prop.GetCustomAttributes(typeof(BindableAttribute), true);
        if ((bindable.Length > 0) && ((BindableAttribute)bindable[0]).Direction == BindingDirection.OneWay)
          continue;
        postbackPropNames.Add(prop.Name);
      }

      var getData = new StringBuilder();
      foreach (var propName in postbackPropNames)
      {
        getData.AppendLine(string.Format("if(viewmodel.{0}() != null) params += '{0}=' + encodeURIComponent(JSON.stringify(viewmodel.{0}()))+'&';", propName));
      }

      var declareData = new StringBuilder();
      foreach (var propName in assignPropNames)
      {
        declareData.AppendLine(string.Format("var {0} = ko.observable();", propName));
      }
      var returnData = new StringBuilder();
      foreach (var propName in assignPropNames)
      {
        returnData.AppendLine(string.Format("{0}: {0},", propName));
      }

      // supply functions for action methods
      var actionMethods = new StringBuilder();
      foreach (var methodInfo in vmType.GetMethods().Where(methodInfo => methodInfo.GetCustomAttributes(false).OfType<ActionMethodAttribute>().Any()))
      {
        var method = "%method%: function (data, event) { post%ViewModelName%Data(self, event.currentTarget, '%method%'); },".Replace("%method%", methodInfo.Name);
        actionMethods.AppendLine(method.Replace("%ViewModelName%", vmType.Name));
      }

      // create
      var text = ClientViewModelTemplate.Replace("%ViewModelType%", vmType.FullName).Replace("%ViewModelName%", vmType.Name);

      text = text.Replace("%SetData%", string.Join(Environment.NewLine, setData));
      text = text.Replace("%GetData%", string.Join(Environment.NewLine, getData));
      text = text.Replace("%DeclareData%", string.Join(Environment.NewLine, declareData));
      text = text.Replace("%ReturnData%", string.Join(Environment.NewLine, returnData));
      text = text.Replace("%ActionMethods%", string.Join(Environment.NewLine, actionMethods));

      ViewModels.Add(vmType, text);
      return text;
    }
 }
}