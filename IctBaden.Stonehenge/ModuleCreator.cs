using System;
using System.Linq;
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
      var propNames = vmType.GetProperties().Select(pi => pi.Name).ToArray();

      var lines = new StringBuilder();

      lines.AppendLine("//ViewModel:" + vmType.FullName);
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

      var xmlNodeList = page.SelectNodes("//button[@data-bind]");
      if (xmlNodeList != null)
      {
        foreach (XmlNode inputNode in xmlNodeList)
        {
          var dataBind = inputNode.Attributes["data-bind"].Value.Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries);
          var click = dataBind.FirstOrDefault(d => d.StartsWith("click"));
          if (click != null)
          {
            var onClick = click.Split(new[] {':'})[1].Trim();
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

      foreach (var propName in propNames)
      {
        lines.AppendLine(string.Format("{0}(data.{0});", propName));
      }

      lines.AppendLine("}); },");

      lines.AppendLine("viewAttached: function(view) {");

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

      lines.AppendLine("}");

      lines.AppendLine("};");

      lines.AppendLine("});");
      return lines.ToString();
    }
  }
}