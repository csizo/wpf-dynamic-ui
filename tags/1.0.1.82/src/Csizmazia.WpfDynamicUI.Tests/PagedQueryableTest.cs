using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Csizmazia.Collections;
using Csizmazia.WpfDynamicUI.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csizmazia.WpfDynamicUI.Tests
{
    /// <summary>
    ///This is a test class for PagedQueryableTest and is intended
    ///to contain all PagedQueryableTest Unit Tests
    ///</summary>
    [TestClass]
    public class PagedQueryableTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion

        private IOrderedQueryable<GenericParameterHelper> GetQuery()
        {
            int capacity = new Random().Next(1, 1000);
            var list = new List<GenericParameterHelper>(capacity);
            for (int i = 1; i < list.Capacity; i++)
            {
                list.Add(new GenericParameterHelper(i));
            }
            return list.AsQueryable().OrderBy(o => o);
        }

        private Expression<Func<GenericParameterHelper, bool>> GetFilter()
        {
            int limit = new Random().Next(10, 1000);
            return c => c.Data < limit;
        }

        /// <summary>
        ///A test for PagedQueryable`1 Constructor
        ///</summary>
        [TestMethod]
        public void PagedQueryableConstructorTest()
        {
            IOrderedQueryable<GenericParameterHelper> query = GetQuery();
            Expression<Func<GenericParameterHelper, bool>> condition = GetFilter();
            int pageSize = 12;
            bool autoLoad = true;
            var target = new PagedQueryable<GenericParameterHelper>(query, condition, pageSize, autoLoad);

            Assert.AreEqual(12, target.PageSize);
            Assert.AreEqual(1, target.CurrentPage);
            Assert.AreEqual(12, target.Items.Count);
        }


        /// <summary>
        ///A test for PagedQueryable`1 Constructor
        ///</summary>
        [TestMethod]
        public void PagedQueryableConstructorTest1()
        {
            IOrderedQueryable<GenericParameterHelper> query = GetQuery();
            int pageSize = 10;
            bool autoLoad = true;
            var target = new PagedQueryable<GenericParameterHelper>(query, pageSize, autoLoad);


            int expectedItemsCount = Math.Min(10, GetQuery().Count());

            Assert.AreEqual(10, target.PageSize);
            Assert.AreEqual(1, target.CurrentPage);
            Assert.AreEqual(expectedItemsCount, target.Items.Count);
        }


        /// <summary>
        ///A test for PagedQueryable`1 Constructor
        ///</summary>
        [TestMethod]
        public void PagedQueryableConstructorTest2()
        {
            IOrderedQueryable<GenericParameterHelper> query = GetQuery();
            bool autoLoad = true;
            var target = new PagedQueryable<GenericParameterHelper>(query, autoLoad);

            int expectedItemsCount = Math.Min(20, GetQuery().Count());

            Assert.AreEqual(20, target.PageSize);
            Assert.AreEqual(1, target.CurrentPage);
            Assert.AreEqual(expectedItemsCount, target.Items.Count);
        }


        /// <summary>
        ///A test for PagedQueryable`1 Constructor
        ///</summary>
        [TestMethod]
        public void PagedQueryableConstructorTest3()
        {
            IOrderedQueryable<GenericParameterHelper> query = GetQuery();
            Expression<Func<GenericParameterHelper, bool>> condition = GetFilter();
            bool autoLoad = true;
            var target = new PagedQueryable<GenericParameterHelper>(query, condition, autoLoad);

            int expectedItemsCount = Math.Min(20, GetQuery().Count());
            Assert.AreEqual(20, target.PageSize);
            Assert.AreEqual(1, target.CurrentPage);
            Assert.AreEqual(expectedItemsCount, target.Items.Count);
        }


        /// <summary>
        ///A test for BeginInit
        ///</summary>
        [TestMethod]
        public void BeginInitTest()
        {
            IOrderedQueryable<GenericParameterHelper> query = GetQuery();
            bool autoLoad = true;
            var target = new PagedQueryable<GenericParameterHelper>(query, autoLoad);

            target.BeginInit();

            target.Reset();
        }


        /// <summary>
        ///A test for BeginInit
        ///</summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EndInitTest()
        {
            IOrderedQueryable<GenericParameterHelper> query = GetQuery();
            bool autoLoad = true;
            var target = new PagedQueryable<GenericParameterHelper>(query, autoLoad);

            target.EndInit();
        }


        /// <summary>
        ///A test for MoveFirst
        ///</summary>
        [TestMethod]
        public void MoveFirstTest()
        {
            IOrderedQueryable<GenericParameterHelper> query = GetQuery();
            bool autoLoad = true;
            var target = new PagedQueryable<GenericParameterHelper>(query, autoLoad);

            target.MoveFirst();


            Assert.AreEqual(1, target.CurrentPage);
        }


        /// <summary>
        ///A test for MoveLast
        ///</summary>
        [TestMethod]
        public void MoveLastTest()
        {
            IOrderedQueryable<GenericParameterHelper> query = GetQuery();
            bool autoLoad = true;
            var target = new PagedQueryable<GenericParameterHelper>(query, autoLoad);

            target.MoveLast();

            Assert.AreEqual(target.PageCount, target.CurrentPage);
        }


        /// <summary>
        ///A test for MoveNext
        ///</summary>
        [TestMethod]
        public void MoveNextTest()
        {
            IOrderedQueryable<GenericParameterHelper> query = GetQuery();
            bool autoLoad = true;
            var target = new PagedQueryable<GenericParameterHelper>(query, autoLoad);

            int second = Math.Min(2, target.PageCount);

            target.MoveNext();

            Assert.AreEqual(second, target.CurrentPage);
        }


        /// <summary>
        ///A test for MovePrevious
        ///</summary>
        [TestMethod]
        public void MovePreviousTest()
        {
            IOrderedQueryable<GenericParameterHelper> query = GetQuery();
            bool autoLoad = true;
            var target = new PagedQueryable<GenericParameterHelper>(query, autoLoad);


            target.MovePrevious();

            Assert.AreEqual(1, target.CurrentPage);
        }


        /// <summary>
        ///A test for Reset
        ///</summary>
        [TestMethod]
        public void RefreshTest()
        {
            IOrderedQueryable<GenericParameterHelper> query = GetQuery();
            bool autoLoad = true;
            var target = new PagedQueryable<GenericParameterHelper>(query, autoLoad);

            target.Reset();

            Assert.AreEqual(1, target.CurrentPage);
        }

        /// <summary>
        ///A test for Reset
        ///</summary>
        [TestMethod]
        public void SortColumnTest()
        {
            IOrderedQueryable<GenericParameterHelper> query = GetQuery();
            bool autoLoad = true;
            var target = new PagedQueryable<GenericParameterHelper>(query, autoLoad);

            target.BeginInit();
            target.SortColumn = "Data";
            target.EndInit();

            Assert.AreEqual(1, target.CurrentPage);
        }

        /// <summary>
        ///A test for Reset
        ///</summary>
        [TestMethod]
        public void SortColumnTestDescending()
        {
            IOrderedQueryable<GenericParameterHelper> query = GetQuery();
            bool autoLoad = true;
            var target = new PagedQueryable<GenericParameterHelper>(query, autoLoad);

            target.BeginInit();
            target.SortColumn = "Data";
            target.SortDirection = SortDirection.Descending;
            target.EndInit();

            Assert.AreEqual(1, target.CurrentPage);
        }

        /// <summary>
        ///A test for Items
        ///</summary>
        [TestMethod]
        public void ItemsTest()
        {
            IOrderedQueryable<GenericParameterHelper> query = GetQuery();
            bool autoLoad = true;
            int pageSize = 19;
            var target = new PagedQueryable<GenericParameterHelper>(query, pageSize, autoLoad);

            TweakedObservableCollection<GenericParameterHelper> actual;
            actual = target.Items;

            int expected = Math.Min(19, query.Count());

            Assert.AreEqual(expected, actual.Count);
        }
    }
}