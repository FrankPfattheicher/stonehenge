using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace IctBaden.Stonehenge
{
  public class ModuleCreator
  {
    public static string CreateFromViewModel(string html, object viewModel)
    {
      var page = new XmlDocument();
      page.LoadXml(html);

      var vmType = viewModel.GetType();
      var propNames = vmType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).Select(pi => pi.Name).ToArray();

      var assigns1 = new StringBuilder();
      var assigns2 = new StringBuilder();

      foreach (var propName in propNames)
      {
        assigns1.AppendLine(string.Format("if(data.{0})", propName));
        assigns2.AppendLine(string.Format("if(data.{0})", propName));
        assigns1.AppendLine(string.Format("{0}(data.{0});", propName));
        assigns2.AppendLine(string.Format("self.{0}(data.{0});", propName));
      }


      var lines = new StringBuilder();
      lines.AppendLine("//ViewModel:" + vmType.FullName);

      lines.AppendLine("function pollEvents(self) {");

      lines.AppendLine("$.getJSON('/events/" + vmType.FullName + "', function(data) {");

      lines.Append(assigns2);

      // plots
      var xmlNodeList = page.SelectNodes("//div[@class]");
      if (xmlNodeList != null)
      {
        foreach (XmlNode inputNode in xmlNodeList)
        {
          var divClass = inputNode.Attributes["class"].Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
          var plot = divClass.FirstOrDefault(d => d == "plot");
          if ((plot != null) && (inputNode.Attributes["id"] != null))
          {
            var propName = inputNode.Attributes["id"].Value;
            lines.AppendLine(string.Format("$.plot($('#{0}'), [self.{0}Data()], options);", propName));
          }
        }
      }

      lines.AppendLine("setTimeout(pollEvents(self), 100);");

      lines.AppendLine("});");

      lines.AppendLine("}");



      lines.AppendLine("define(function (require) {");

      foreach (var propName in propNames)
      {
        lines.AppendLine(string.Format("var {0} = ko.observable();", propName));
      }

      lines.AppendLine("return {");

      foreach (var propName in propNames)
      {
        lines.AppendLine(string.Format("{0}: {0},", propName));
      }

      // button clicks
      xmlNodeList = page.SelectNodes("//button[@data-bind]");
      if (xmlNodeList != null)
      {
        foreach (XmlNode inputNode in xmlNodeList)
        {
          var dataBind = inputNode.Attributes["data-bind"].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
          var click = dataBind.FirstOrDefault(d => d.StartsWith("click"));
          if (click != null)
          {
            var onClick = click.Split(new[] { ':' })[1].Trim();
            lines.AppendLine(onClick + ": function () {");
            lines.AppendLine("var params = '';");
            foreach (var propName in propNames)
            {
              lines.AppendLine(string.Format("if({0}() != null) params += '{0}=' + encodeURIComponent({0}())+'&';", propName));
            }
            lines.AppendLine(" $.post('/viewmodel/" + vmType.FullName + "/" + onClick + "', params, function (data) {");
            foreach (var propName in propNames)
            {
              lines.AppendLine(string.Format("{0}(data.{0});", propName));
            }
            lines.AppendLine("}); },");
          }
        }
      }



      lines.AppendLine("activate: function() {");
      lines.AppendLine("$.getJSON('/viewmodel/" + vmType.FullName + "', function(data) {");

      lines.AppendLine("var self = this;");
      lines.Append(assigns1);

      lines.AppendLine("}); },");


      lines.AppendLine("viewAttached: function(view) {");

//lines.AppendLine("debugger;");

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
            lines.AppendLine(string.Format("$.plot($('#{0}'), [{0}Data()], options);", propName));
          }
        }
      }

      lines.AppendLine("var self = this;");
      lines.AppendLine("pollEvents(self);");
      lines.AppendLine("}");

      lines.AppendLine("};");



      lines.AppendLine("});");
      return lines.ToString();
    }
  }
}