using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using IctBaden.Stonehenge.Graph;
using IctBaden.Stonehenge.Tree;
using ServiceStack.Text;

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

      // trees
      foreach (var prop in vmProps.Where(p => ((p.PropertyType == typeof(TreeNode[]) || (p.PropertyType == typeof(List<TreeNode>))) && p.Name.EndsWith("Data"))))
      {
        // TODO: move this code to JS library
        var propName = prop.Name.Substring(0, prop.Name.Length - 4);  // remove "Data"
        setData.AppendLine(string.Format("if(data.{0}Data) {{ try {{ $.fn.zTree.init($('#{0}'), viewmodel.{0}Settings(), viewmodel.{0}Data()); }} catch(e) {{ }} }}", propName));
        setData.AppendLine(string.Format("if(loading) {{ try {{ {0}Initialize(); }} catch(e) {{ }} }}", propName));
      }

      // plots
      foreach (var prop in vmProps.Where(p => ((p.PropertyType == typeof(GraphSeries[]) || (p.PropertyType == typeof(List<GraphSeries>))) && p.Name.EndsWith("Data"))))
      {
        // TODO: move this code to JS library
        var propName = prop.Name.Substring(0, prop.Name.Length - 4);  // remove "Data"
        setData.AppendLine(string.Format("if(data.{0}Data) {{ try {{ $.plot($('#{0}'), viewmodel.{0}Data(), viewmodel.{0}Options()); }} catch(e) {{ }} }}", propName));
        setData.AppendLine(string.Format("$('#{0}').resize(function() {{ try {{ $.plot($('#{0}'), viewmodel.{0}Data(), viewmodel.{0}Options()); }} catch(e) {{ }} }});", propName));
        setData.AppendLine(string.Format("if(loading) {{ try {{ {0}Initialize(); }} catch(e) {{ }} }}", propName));
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

        if (prop.GetSetMethod(false) == null) // not public writable
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
        object defaultValue;
        if (activeVm != null)
        {
          defaultValue = activeVm.TryGetMember(propName);
        }
        else
        {
          defaultValue = vmType.GetProperty(propName).GetValue(viewModel, new object[0]);
        }
        declareData.AppendLine(string.Format("var {0} = ko.observable({1});", propName, (defaultValue != null) ? JsonSerializer.SerializeToString(defaultValue) : string.Empty));
        declareData.AppendLine(string.Format("{0}.subscribe(function(newval){{IsDirty(true);}});", propName));
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
        string method;
        if (methodInfo.GetParameters().Length > 0)
        {
          method = "%method%: function (data, event, param) { post_ViewModelName_Data(self, event.currentTarget, '%method%', param); },".Replace("%method%", methodInfo.Name);
        }
        else
        {
          method = "%method%: function (data, event) { post_ViewModelName_Data(self, event.currentTarget, '%method%', null); },".Replace("%method%", methodInfo.Name);
        }
        actionMethods.AppendLine(method.Replace("_ViewModelName_", vmType.Name));
      }

      // create
      var text = ClientViewModelTemplate.Replace("_ViewModelType_", vmType.FullName).Replace("_ViewModelName_", vmType.Name);

      text = text.Replace("_SetData_();", string.Join(Environment.NewLine, setData));
      text = text.Replace("_GetData_();", string.Join(Environment.NewLine, getData));
      text = text.Replace("_DeclareData_();", string.Join(Environment.NewLine, declareData));
      text = text.Replace("_ReturnData_: 0,", string.Join(Environment.NewLine, returnData));
      text = text.Replace("_ActionMethods_: 0,", string.Join(Environment.NewLine, actionMethods));

      ViewModels.Add(vmType, text);
      return text;
    }
 }
}