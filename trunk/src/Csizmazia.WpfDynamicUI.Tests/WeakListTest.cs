using System;
using Csizmazia.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csizmazia.WpfDynamicUI.Tests
{
    [TestClass]
    public class WeakListTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void Test_GcCollect()
        {
            var weakList = new WeakList<object>();
            weakList.Add(new Object());


            GC.Collect();
            GC.WaitForPendingFinalizers();

            weakList.Clear(true);

            Assert.IsTrue(weakList.Count == 0, "Item must be collected by the GC collector");
        }

        [TestMethod]
        public void Test_GcCollectWholeList()
        {
            var weakList = new WeakList<object>();
            for (int i = 0; i < 10000; i++)
                weakList.Add(new Object());


            GC.Collect();
            GC.WaitForPendingFinalizers();

            weakList.Clear(true);

            Assert.IsTrue(weakList.Count == 0, "Items must be collected by the GC collector");
        }

        [TestMethod]
        public void Test_GcCannotCollect()
        {
            var weakList = new WeakList<object>();

            //holding strong reference
            var item = new Object();

            TestContext.Properties.Add("item", item);

            weakList.Add(item);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            weakList.Clear(true);

            Assert.AreEqual(1, weakList.Count);

            TestContext.Properties.Remove("item");
        }

        [TestMethod]
        public void Test_GCCannotCollectWholeList()
        {
            var items = new Object[10000];
            for (int i = 0; i < 10000; i++)
            {
                items.SetValue(new Object(), i);
            }

            TestContext.Properties.Add("items", items);
            var weakList = new WeakList<object>(items);


            GC.Collect();
            GC.WaitForPendingFinalizers();

            weakList.Clear(true);

            Assert.AreEqual(10000, weakList.Count);

            TestContext.Properties.Remove("items");
        }
    }
}