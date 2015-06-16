namespace IctBaden.Stonehenge2.Test.Resources
{
    using System.Collections.Generic;
    using System.Reflection;

    using IctBaden.Stonehenge2.Core;
    using IctBaden.Stonehenge2.Resources;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ResourceLoaderTests
    {
        private ResourceLoader loader;
        private readonly AppSession session = new AppSession();

        [TestInitialize]
        public void Init()
        {
            var assemblies = new List<Assembly>
                                 {
                                     Assembly.GetAssembly(typeof(ResourceLoader)),
                                     Assembly.GetExecutingAssembly()
                                 };
            loader = new ResourceLoader(assemblies);
        }

        // ReSharper disable InconsistentNaming

        [TestMethod]
        public void Load_resource_unknown_txt()
        {
            var resource = loader.Load(session, "unknown.txt");
            Assert.IsNull(resource);
        }

        [TestMethod]
        public void Load_resource_icon_png()
        {
            var resource = loader.Load(session, "icon.png");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "image/png");
            Assert.IsTrue(resource.IsBinary);
            Assert.AreEqual(179, resource.Data.Length);
        }

        [TestMethod]
        public void Load_resource_icon32_png()
        {
            var resource = loader.Load(session, "icon32.png");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "image/png");
            Assert.IsTrue(resource.IsBinary);
            Assert.AreEqual(227, resource.Data.Length);
        }

        [TestMethod]
        public void Load_resource_image_png()
        {
            var resource = loader.Load(session, "image.jpg");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "image/jpeg");
            Assert.IsTrue(resource.IsBinary);
            Assert.AreEqual(1009, resource.Data.Length);
        }

        [TestMethod]
        public void Load_resource_test_html()
        {
            var resource = loader.Load(session, "test.html");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "text/html");
            Assert.IsFalse(resource.IsBinary);
            Assert.IsTrue(resource.Text.StartsWith("<!DOCTYPE html>"));

            resource = loader.Load(session, "TesT.HTML");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "text/html");
            Assert.IsFalse(resource.IsBinary);
            Assert.IsTrue(resource.Text.StartsWith("<!DOCTYPE html>"));
        }

        [TestMethod]
        public void Load_resource_testscript_js()
        {
            var resource = loader.Load(session, "lib/testscript.js");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "text/javascript");
            Assert.IsFalse(resource.IsBinary);
            Assert.IsTrue(resource.Text.Contains("function Test()"));
        }
    }
}
