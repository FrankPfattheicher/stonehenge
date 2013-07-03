using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;

namespace IctBaden.Stonehenge.Creators
{
  public class ModuleCreator
  {
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

      // properties
      var assignSelf = new StringBuilder();
      var plotSelf = new StringBuilder();

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

      var assignPropNames = new List<string>();
      // ReSharper disable LoopCanBeConvertedToQuery
      foreach (var prop in vmProps)
      {
        var bindable = prop.Attributes.OfType<BindableAttribute>().ToArray();
        if ((bindable.Length > 0) && !((BindableAttribute)bindable[0]).Bindable)
          continue;
        assignPropNames.Add(prop.Name);
      }
      // ReSharper restore LoopCanBeConvertedToQuery


      foreach (var propName in assignPropNames)
      {
        assignSelf.AppendLine(string.Format("if(data.{0} != null)", propName));
        assignSelf.AppendLine(string.Format("self.{0}(data.{0});", propName));
      }

      // plots
      foreach (var prop in vmProps.Where(p => (p.PropertyType == typeof (GraphSeries[]) && p.Name.EndsWith("Data"))))
      {
        var propName = prop.Name.Substring(0, prop.Name.Length - 4);  // remove "Data"
        plotSelf.AppendLine(string.Format("if(data.{0}Data) $.plot($('#{0}'), self.{0}Data(), self.{0}Options());", propName));
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
        var bindable = prop.GetCustomAttributes(typeof (BindableAttribute), true);
        if ((bindable.Length > 0) && ((BindableAttribute) bindable[0]).Direction == BindingDirection.OneWay)
          continue;
        postbackPropNames.Add(prop.Name);
      }


      var lines = new StringBuilder();
      lines.AppendLine("//ViewModel:" + vmType.FullName);

      var supportsEvents = typeof(ActiveViewModel).IsAssignableFrom(vmType);
      var eventFunction = string.Format("poll{0}Events", vmType.Name);
      if (supportsEvents)
      {
        lines.AppendLine("function " + eventFunction + "(self) {");

        lines.AppendLine("var app = require('durandal/app');");
        lines.AppendLine("var ts = new Date().getTime();");
        lines.AppendLine("$.getJSON('/events/" + vmType.FullName + "?ts='+ts, function(data) {");

        lines.Append(assignSelf);
        lines.Append(plotSelf);

        lines.AppendLine("if(data.eval != null) eval(data.eval);");

        lines.AppendLine("setTimeout(function(){" + eventFunction + "(self)}, 100);");

        lines.AppendLine("});");

        lines.AppendLine("}");
      }


      lines.AppendLine("define(function (require) {");

      foreach (var propName in assignPropNames)
      {
        lines.AppendLine(string.Format("var {0} = ko.observable();", propName));
      }

      lines.AppendLine("return {");

      foreach (var propName in assignPropNames)
      {
        lines.AppendLine(string.Format("{0}: {0},", propName));
      }

      // supply functions for action methods
      foreach (var methodInfo in vmType.GetMethods())
      {
        if(!methodInfo.GetCustomAttributes(false).OfType<ActionMethodAttribute>().Any())
          continue;

        lines.AppendLine(methodInfo.Name + ": function () {");
        lines.AppendLine("var params = '';");
        foreach (var propName in postbackPropNames)
        {
          lines.AppendLine(string.Format("if({0}() != null) params += '{0}=' + encodeURIComponent(JSON.stringify({0}()))+'&';", propName));
        }

        lines.AppendLine("var ts = new Date().getTime();");
        lines.AppendLine("$.post('/viewmodel/" + vmType.FullName + "/" + methodInfo.Name + "?ts='+ts, params, function (data) {");

        //lines.AppendLine("debugger;");

        lines.Append(assignSelf);
        lines.Append(plotSelf);

        lines.AppendLine("}); },");
      }

      //lines.AppendLine("activate: function() {");
      //lines.AppendLine("},");

      lines.AppendLine("viewAttached: function(view) {");

      lines.AppendLine("self = this;");

//lines.AppendLine("debugger;");

      lines.AppendLine("var ts = new Date().getTime();");
      lines.AppendLine("$.getJSON('/viewmodel/" + vmType.FullName + "?ts='+ts, function(data) {");

      lines.Append(assignSelf);
      lines.Append(plotSelf);

      lines.AppendLine("});");


      if (supportsEvents)
      {
        lines.AppendLine("setTimeout(function(){" + eventFunction + "(self)}, 100);");
      }
      lines.AppendLine("}");

      lines.AppendLine("};");



      lines.AppendLine("});");
      return lines.ToString();
    }
  }
}