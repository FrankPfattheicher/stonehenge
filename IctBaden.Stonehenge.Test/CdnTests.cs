namespace IctBaden.Stonehenge.Test
{
    using IctBaden.Stonehenge.Creators;
    using IctBaden.Stonehenge.Services;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [DeploymentItem("CDN.cfg")]
    [TestClass]
    public class CdnTests
    {
        [TestMethod]
        public void HtmlNoMatchReturnsOriginalText()
        {
            const string page = "  <link href=\"app/icon.png\" rel=\"icon\" type=\"image/x-icon\" />   ";

            var resolved = ContentDeliveryNetworkSupport.RersolveHostsHtml(page, false);

            Assert.AreEqual(page, resolved);
        }

        [TestMethod]
        public void HtmlMatchCssReturnsResolvedText()
        {
            const string Page = "  <link type=\"text/css\" media=\"all\" href=\"css/bootstrap.css\" rel=\"stylesheet\">   ";
            var target = ContentDeliveryNetworkSupport.CdnLookup["bootstrap.css"];
            var expected = Page.Replace("css/bootstrap.css", target).Trim();

            var resolved = ContentDeliveryNetworkSupport.RersolveHostsHtml(Page, false).Trim();

            Assert.AreEqual(expected, resolved);
        }

        [TestMethod]
        public void HtmlMatchScriptReturnsResolvedText()
        {
            const string Page = "  <script type=\"text/javascript\" src=\"scripts/jquery-2.1.3.js\"></script>  ";
            var target = ContentDeliveryNetworkSupport.CdnLookup["jquery-2.1.3.js"];
            var expected = Page.Replace("scripts/jquery-2.1.3.js", target).Trim();

            var resolved = ContentDeliveryNetworkSupport.RersolveHostsHtml(Page, false).Trim();

            Assert.AreEqual(expected, resolved);
        }

        [TestMethod]
        public void JsNoMatchReturnsOriginalText()
        {
            const string Script = "  'none':         '/app/lib/flot/js/nothing',   ";

            var resolved = ContentDeliveryNetworkSupport.RersolveHostsJs(Script, false);

            Assert.AreEqual(Script, resolved);
        }

        [TestMethod]
        public void JsMatchMapReturnsResolvedText()
        {
            const string script = "    'flot':         '/app/lib/flot/js/jquery.flot',";
            var target = ContentDeliveryNetworkSupport.CdnLookup["jquery.flot.js"];
            var expected = script.Replace("/app/lib/flot/js/jquery.flot", target).Replace(".js'", "'").Trim();

            var resolved = ContentDeliveryNetworkSupport.RersolveHostsJs(script, false).Trim();

            Assert.AreEqual(expected, resolved);
        }

        [TestMethod]
        public void JsMainJsReturnsResolvedText()
        {
            var mainjs = ResourceLoader.LoadText("", "", "main.js");
            var resolved = ContentDeliveryNetworkSupport.RersolveHostsJs(mainjs, false);

            Assert.IsFalse(resolved.Contains("/app/lib/knockout/js/"));
            Assert.IsFalse(resolved.Contains("/app/lib/bootstrap/js/"));
            Assert.IsFalse(resolved.Contains("/app/lib/jquery/js/"));
            Assert.IsFalse(resolved.Contains("/app/lib/flot/js/"));
        }

    }
}