namespace IctBaden.Stonehenge.Test
{
    using System;

    using Stonehenge;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ViewModelTests
    {
        private class TestModel1
        {
            public string Text { get; set; }
        }

        private class TestModel2
        {
            public string Test { get; set; }
        }

        private class TestViewModel : ActiveViewModel
        {
            public string First { get; set; }
            public string Test { get; set; }
        }

        [TestMethod]
        public void AllPropertiesCollected()
        {
            var vm = new TestViewModel();
            Assert.AreEqual(3, vm.GetProperties().Count);

            vm.SetModel(new TestModel1());
            Assert.AreEqual(4, vm.GetProperties().Count);
        }

        [TestMethod]
        public void ExceptionOnDuplicateProperties()
        {
            var vm = new TestViewModel();

            try
            {
                vm.SetModel(new TestModel2());
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Test", ex.ParamName);
                return;
            }

            Assert.Fail("Should throw ArgumentException");
        }
         
    }
}