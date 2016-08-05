namespace IctBaden.Stonehenge2.Test.Resources
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using Core;
    using Stonehenge2.Resources;
    using NUnit.Framework;

    [TestFixture]
    public class LoaderTests
    {
        private Loader loader;
        private readonly AppSession session = new AppSession();

        private FileLoaderTests fileTest;
        private ResourceLoader resLoader;
        private FileLoader fileLoader;

        [SetUp]
        public void Init()
        {
            var assemblies = new List<Assembly>
                                 {
                                     Assembly.GetAssembly(typeof(ResourceLoader)),
                                     Assembly.GetExecutingAssembly()
                                 };
            resLoader = new ResourceLoader(assemblies);

            fileLoader = new FileLoader(Path.GetTempPath());

            loader = new Loader(new List<IStonehengeResourceProvider>{ fileLoader, resLoader });

            fileTest = new FileLoaderTests();
            fileTest.Init();
        }

        [TearDown]
        public void Cleanup()
        {
            fileTest?.Cleanup();
        }

        // ReSharper disable InconsistentNaming

        [Test]
        public void Load_from_file_icon_png()
        {
            fileTest.CreateBinaryFile("icon.png");
            var resource = loader.Get(session, "icon.png", new Dictionary<string, string>());
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "image/png");
            Assert.IsTrue(resource.IsBinary);
            Assert.AreEqual(16, resource.Data.Length);
            Assert.IsTrue(resource.Source.StartsWith("file://"));
        }

        [Test]
        public void Load_from_resource_icon_png()
        {
            var resource = loader.Get(session, "image.jpg", new Dictionary<string, string>());
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "image/jpeg");
            Assert.IsTrue(resource.IsBinary);
            Assert.AreEqual(1009, resource.Data.Length);
            Assert.IsTrue(resource.Source.StartsWith("res://"));
        }

        [Test]
        public void Load_from_file_over_resource_icon_png()
        {
            fileTest.CreateTextFile("index.html");
            var resource = loader.Get(session, "index.html", new Dictionary<string, string>());
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "text/html");
            Assert.IsFalse(resource.IsBinary);
            Assert.IsTrue(resource.Text.StartsWith("<!DOCTYPE html>"));
            Assert.IsTrue(resource.Source.StartsWith("file://"));
        }


    }
}