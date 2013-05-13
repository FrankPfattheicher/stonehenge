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
    public static string CreateFromViewModel(string html, object viewModel)
    {
      var xhtml = html.Replace("&nbsp;", " ");
      var page = new XmlDocument();
      page.LoadXml(xhtml);

      var vmType = viewModel.GetType();
      var supportsEvents = typeof (ActiveViewModel).IsAssignableFrom(vmType);

      var eventFunction = string.Format("poll{0}Events", vmType.Name);

      XmlNodeList xmlNodeList;

      // properties
      var assignThis = new StringBuilder();
      var assignSelf = new StringBuilder();
      var plotThis = new StringBuilder();
      var plotSelf = new StringBuilder();

      var assignPropNames = vmType.GetProperties().Select(pi => pi.Name).ToArray();

      foreach (var propName in assignPropNames)
      {
        assignThis.AppendLine(string.Format("if(data.{0})", propName));
        assignThis.AppendLine(string.Format("{0}(data.{0});", propName));

        assignSelf.AppendLine(string.Format("if(data.{0})", propName));
        assignSelf.AppendLine(string.Format("self.{0}(data.{0});", propName));
      }

      // plots
      xmlNodeList = page.SelectNodes("//div[@class]");
      if (xmlNodeList != null)
      {
        foreach (XmlNode inputNode in xmlNodeList)
        {
          var divClass = inputNode.Attributes["class"].Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
          var plot = divClass.FirstOrDefault(d => d == "plot");
          if ((plot != null) && (inputNode.Attributes["id"] != null))
          {
            var propName = inputNode.Attributes["id"].Value;

            plotThis.AppendLine(string.Format("if(data.{0}Data)", propName));
            plotThis.AppendLine(string.Format("$.plot($('#{0}'), {0}Data(), options{0});", propName));

            plotSelf.AppendLine(string.Format("if(data.{0}Data)", propName));
            plotSelf.AppendLine(string.Format("$.plot($('#{0}'), self.{0}Data(), options{0});", propName));
          }
        }
      }

      // do not send ReadOnly or OneWay bound properties back
      var postbackPropNames = new List<string>();
      foreach (var prop in vmType.GetProperties().Where(pi => pi.CanWrite))
      {
        var bindable = prop.GetCustomAttributes(typeof (BindableAttribute), true);
        if ((bindable.Length > 0) && ((BindableAttribute) bindable[0]).Direction == BindingDirection.OneWay)
          continue;
        postbackPropNames.Add(prop.Name);
      }

      var lines = new StringBuilder();

      lines.AppendLine("//ViewModel:" + vmType.FullName);

      if (supportsEvents)
      { 
        lines.AppendLine("function " + eventFunction + "(self) {");

        lines.AppendLine("var ts = new Date().getTime();");
        lines.AppendLine("$.getJSON('/events/" + vmType.FullName + "?ts='+ts, function(data) {");

        lines.Append(assignSelf);
        lines.Append(plotSelf);

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

      // button clicks
      xmlNodeList = page.SelectNodes("//button[@data-bind]");
      if (xmlNodeList != null)
      {
        foreach (XmlNode inputNode in xmlNodeList)
        {
          var click = inputNode.Attributes["data-bind"].Value
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(bind => bind.Trim())
            .FirstOrDefault(d => d.StartsWith("click"));
          if (click != null)
          {
            var onClick = click.Split(new[] { ':' })[1].Trim();
            lines.AppendLine(onClick + ": function () {");
            lines.AppendLine("var params = '';");
            foreach (var propName in postbackPropNames)
            {
              lines.AppendLine(string.Format("if({0}() != null) params += '{0}=' + encodeURIComponent({0}())+'&';", propName));
            }

            lines.AppendLine(" $.post('/viewmodel/" + vmType.FullName + "/" + onClick + "', params, function (data) {");

            lines.Append(assignThis);
            lines.Append(plotThis);

            lines.AppendLine("}); },");
          }
        }
      }

      // selection changes
      xmlNodeList = page.SelectNodes("//select[@data-bind]");
      if (xmlNodeList != null)
      {
        foreach (XmlNode selectNode in xmlNodeList)
        {
          var dataBind = selectNode.Attributes["data-bind"].Value
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(bind => bind.Trim())
            .FirstOrDefault(d => d.StartsWith("event"));
          if (dataBind != null)
          {
            var eventBind = dataBind.Replace("event:", "").Replace("{", "").Replace("}", "").Trim()
              .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(bind => bind.Trim())
              .FirstOrDefault(d => d.StartsWith("change")); ;
            if (eventBind != null)
            {
              var onchange = eventBind.Split(new[] { ':' })[1].Trim();

              lines.AppendLine(onchange + ": function () {");
              lines.AppendLine("var params = '';");
              foreach (var propName in postbackPropNames)
              {
                lines.AppendLine(string.Format("if({0}() != null) params += '{0}=' + encodeURIComponent({0}())+'&';", propName));
              }
              lines.AppendLine(" $.post('/viewmodel/" + vmType.FullName + "/" + onchange + "', params, function (data) {");

              lines.Append(assignThis);
              lines.Append(plotThis);

              lines.AppendLine("}); },");
            }
          }
        }
      }


      //lines.AppendLine("activate: function() {");
      //lines.AppendLine("},");

      lines.AppendLine("viewAttached: function(view) {");

      lines.AppendLine("var self = this;");

//lines.AppendLine("debugger;");

      lines.AppendLine("var ts = new Date().getTime();");
      lines.AppendLine("$.getJSON('/viewmodel/" + vmType.FullName + "?ts='+ts, function(data) {");

      lines.Append(assignThis);
      lines.Append(plotThis);

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