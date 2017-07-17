using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rentals.Domain.Tests
{
    /// <summary>
    /// Test class for the Category entity
    /// </summary>
    [TestClass]
    public class CategoryTest
    {
        [TestMethod]
        public void CanCreateCategory()
        {
            var expected = new Category();

            Assert.IsNotNull(expected);
        }

        [TestMethod]
        public void CanSetCategoryProperties()
        {
            var category = new Category();

            var name = "Category1";
            var type = CategoryType.Expense;
            category.Name = name;
            category.Type = type;

            Assert.AreEqual(name, category.Name);
            Assert.AreEqual(type, category.Type);
            Assert.AreEqual(0, category.Transactions.Count);
        }

    }
}
