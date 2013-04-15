using System.Linq;
using System.Text;

namespace IctBaden.Stonehenge
{
  public class ModuleCreator
  {
    public static string CreateFromViewModel(object viewModel)
    {
      var vmType = viewModel.GetType();
      var propNames = vmType.GetProperties().Select(pi => pi.Name).ToArray();

      var lines = new StringBuilder();

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

      lines.AppendLine("activate: function() {");
      lines.AppendLine("$.getJSON('/viewmodel/" + vmType.FullName + "', function(data) {");

      foreach (var propName in propNames)
      {
        lines.AppendLine(string.Format("{0}(data.{0});", propName));
      }

      lines.AppendLine("}); } }; });");
      return lines.ToString();
    }
  }
}