using IctBaden.Stonehenge.Creators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IctBaden.StonehengeTest
{
  [DeploymentItem("CDN.cfg")]
  [TestClass]
  public class CdnTests
  {
    [TestMethod]
    public void HtmlNoMatchReturnsOriginalText()
    {
      const string page = "  <link href=\"app/icon.png\" rel=\"icon\" type=\"image/x-icon\" />   ";

      var resolved = ContentDeliveryNetworkSupport.RersolveHostsHtml(page);

      Assert.AreEqual(page, resolved);
    }

    [TestMethod]
    public void HtmlMatchCssReturnsResolvedText()
    {
      const string page = "  <link type=\"text/css\" media=\"all\" href=\"css/bootstrap.css\" rel=\"stylesheet\">   ";
      var target = ContentDeliveryNetworkSupport.CdnLookup["bootstrap.css"];
      var expected = page.Replace("css/bootstrap.css", target).Trim();

      var resolved = ContentDeliveryNetworkSupport.RersolveHostsHtml(page).Trim();

      Assert.AreEqual(expected, resolved);
    }

    [TestMethod]
    public void HtmlMatchScriptReturnsResolvedText()
    {
      const string page = "  <script type=\"text/javascript\" src=\"scripts/jquery-1.10.2.js\"></script>  ";
      var target = ContentDeliveryNetworkSupport.CdnLookup["jquery-1.10.2.js"];
      var expected = page.Replace("scripts/jquery-1.10.2.js", target).Trim();

      var resolved = ContentDeliveryNetworkSupport.RersolveHostsHtml(page).Trim();

      Assert.AreEqual(expected, resolved);
    }

    [TestMethod]
    public void JsNoMatchReturnsOriginalText()
    {
      const string script = "  'none':         '/app/lib/flot/js/nothing',   ";

      var resolved = ContentDeliveryNetworkSupport.RersolveHostsJs(script);

      Assert.AreEqual(script, resolved);
    }

    [TestMethod]
    public void JsMatchMapReturnsResolvedText()
    {
      const string script = "  'flot':         '/app/lib/flot/js/jquery.flot',   ";
      var target = ContentDeliveryNetworkSupport.CdnLookup["jquery.flot.js"];
      var expected = script.Replace("/app/lib/flot/js/jquery.flot", target).Replace(".js'", "'").Trim();
      
      var resolved = ContentDeliveryNetworkSupport.RersolveHostsJs(script).Trim();
      
      Assert.AreEqual(expected, resolved);
    }

  }
}