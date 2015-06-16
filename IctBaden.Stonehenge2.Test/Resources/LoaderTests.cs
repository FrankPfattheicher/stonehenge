namespace IctBaden.Stonehenge2.Test.Resources
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using IctBaden.Stonehenge2.Core;
    using IctBaden.Stonehenge2.Resources;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LoaderTests
    {
        private Loader loader;
        private readonly AppSession session = new AppSession();

        private FileLoaderTests fileTest;
        private ResourceLoader resLoader;
        private FileLoader fileLoader;

        [TestInitialize]
        public void Init()
        {
            var assemblies = new List<Assembly>
                                 {
                                     Assembly.GetAssembly(typeof(ResourceLoader)),
                                     Assembly.GetExecutingAssembly()
                                 };
            resLoader = new ResourceLoader(assemblies);

            fileLoader = new FileLoader(Path.GetTempPath());

            loader = new Loader(new List<IResourceProvider>{ fileLoader, resLoader });

            fileTest = new FileLoaderTests();
            fileTest.Init();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (fileTest != null)
            {
                fileTest.Cleanup();
            }
        }
        
        // ReSharper disable InconsistentNaming

        [TestMethod]
        public void Load_from_file_icon_png()
        {
            fileTest.CreateBinaryFile("icon.png");
            var resource = loader.Load(session, "icon.png");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "image/png");
            Assert.IsTrue(resource.IsBinary);
            Assert.AreEqual(16, resource.Data.Length);
            Assert.IsTrue(resource.Source.StartsWith("file://"));
        }

        [TestMethod]
        public void Load_from_resource_icon_png()
        {
            var resource = loader.Load(session, "image.jpg");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "image/jpeg");
            Assert.IsTrue(resource.IsBinary);
            Assert.AreEqual(1009, resource.Data.Length);
            Assert.IsTrue(resource.Source.StartsWith("res://"));
        }

        [TestMethod]
        public void Load_from_file_over_resource_icon_png()
        {
            fileTest.CreateTextFile("index.html");
            var resource = loader.Load(session, "index.html");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "text/html");
            Assert.IsFalse(resource.IsBinary);
            Assert.IsTrue(resource.Text.StartsWith("<!DOCTYPE html>"));
            Assert.IsTrue(resource.Source.StartsWith("file://"));
        }


    }
}