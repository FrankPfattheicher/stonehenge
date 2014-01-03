using IctBaden.Stonehenge.Creators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IctBaden.StonehengeTest
{
  [DeploymentItem("CDN.cfg")]
  [TestClass]
  public class CdnTests
  {
    [TestMethod]
    public void NoMatchReturnsOriginalText()
    {
      const string page = "  <link href=\"app/icon.png\" rel=\"icon\" type=\"image/x-icon\" />   ";

      var resolved = ContentDeliveryNetworkSupport.RersolveHosts(page);

      Assert.AreEqual(page, resolved);
    }

    [TestMethod]
    public void MatchCssReturnsOriginalText()
    {
      const string page = "  <link media=\"all\" href=\"css/bootstrap.css\" rel=\"stylesheet\">   ";
      var target = ContentDeliveryNetworkSupport.CdnLookup["bootstrap.css"];
      var expected = page.Replace("css/bootstrap.css", target).Trim();

      var resolved = ContentDeliveryNetworkSupport.RersolveHosts(page).Trim();

      Assert.AreEqual(expected, resolved);
    }

    [TestMethod]
    public void MatchScriptReturnsOriginalText()
    {
      const string page = "  <script type=\"text/javascript\" src=\"scripts/jquery-1.10.2.js\"></script>  ";
      var target = ContentDeliveryNetworkSupport.CdnLookup["jquery-1.10.2.js"];
      var expected = page.Replace("scripts/jquery-1.10.2.js", target).Trim();

      var resolved = ContentDeliveryNetworkSupport.RersolveHosts(page).Trim();

      Assert.AreEqual(expected, resolved);
    }

  }
}