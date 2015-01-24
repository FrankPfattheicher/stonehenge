namespace IctBaden.Stonehenge2.Test.Resources
{
    using System.Collections.Generic;
    using System.Reflection;

    using IctBaden.Stonehenge2.Resources;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ResourceLoaderTests
    {
        private ResourceLoader loader;

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
            var resource = loader.Load("unknown.txt");
            Assert.IsNull(resource);
        }

        [TestMethod]
        public void Load_resource_icon_png()
        {
            var resource = loader.Load("icon.png");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "image/png");
            Assert.IsTrue(resource.IsBinary);
            Assert.AreEqual(180, resource.Data.Length);
        }

        [TestMethod]
        public void Load_resource_index_html()
        {
            var resource = loader.Load("index.html");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "text/html");
            Assert.IsFalse(resource.IsBinary);
            Assert.IsTrue(resource.Text.StartsWith("<!DOCTYPE html>"));
        }

        [TestMethod]
        public void Load_resource_image_png()
        {
            var resource = loader.Load("image.jpg");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "image/jpeg");
            Assert.IsTrue(resource.IsBinary);
            Assert.AreEqual(1009, resource.Data.Length);
        }

        [TestMethod]
        public void Load_resource_test_html()
        {
            var resource = loader.Load("test.htm");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "text/html");
            Assert.IsFalse(resource.IsBinary);
            Assert.IsTrue(resource.Text.StartsWith("<!DOCTYPE html>"));
        }
    }
}
