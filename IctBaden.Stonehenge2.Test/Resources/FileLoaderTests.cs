namespace IctBaden.Stonehenge2.Test.Resources
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using IctBaden.Stonehenge2.Core;
    using IctBaden.Stonehenge2.Resources;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FileLoaderTests
    {
        private FileLoader loader;
        private readonly AppSession session = new AppSession();
        private string fullFileName;

        [TestInitialize]
        public void Init()
        {
            var path = Path.GetTempPath();
            loader = new FileLoader(path);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if ((fullFileName != null) && File.Exists(fullFileName))
            {
                File.Delete(fullFileName);
            }
        }

        // ReSharper disable InconsistentNaming

        public void CreateTextFile(string name)
        {
            fullFileName = Path.Combine(loader.RootPath, name);
            try
            {
                var file = File.CreateText(fullFileName);
                file.Write("<!DOCTYPE html>" + Environment.NewLine + "<h1>Testfile</h1>");
                file.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void CreateBinaryFile(string name)
        {
            fullFileName = Path.Combine(loader.RootPath, name);
            try
            {
                var file = File.Create(fullFileName);
                var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
                file.Write(data, 0, data.Length);
                file.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void Load_file_unknown_txt()
        {
            var resource = loader.Load(session, "unknown.txt");
            Assert.IsNull(resource);
        }

        [TestMethod]
        public void Load_file_icon_png()
        {
            CreateBinaryFile("icon.png");
            var resource = loader.Load(session, "icon.png");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "image/png");
            Assert.IsTrue(resource.IsBinary);
            Assert.AreEqual(16, resource.Data.Length);
        }

        [TestMethod]
        public void Load_file_index_html()
        {
            CreateTextFile("index.html");
            var resource = loader.Load(session, "index.html");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "text/html");
            Assert.IsFalse(resource.IsBinary);
            Assert.IsTrue(resource.Text.StartsWith("<!DOCTYPE html>"));
        }

        [TestMethod]
        public void Load_file_image_png()
        {
            CreateBinaryFile("image.jpg");
            var resource = loader.Load(session, "image.jpg");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "image/jpeg");
            Assert.IsTrue(resource.IsBinary);
            Assert.AreEqual(16, resource.Data.Length);
        }

        [TestMethod]
        public void Load_file_test_html()
        {
            CreateTextFile("test.htm");
            var resource = loader.Load(session, "test.htm");
            Assert.IsNotNull(resource);
            Assert.AreEqual(resource.ContentType, "text/html");
            Assert.IsFalse(resource.IsBinary);
            Assert.IsTrue(resource.Text.StartsWith("<!DOCTYPE html>"));
        }

    }
}