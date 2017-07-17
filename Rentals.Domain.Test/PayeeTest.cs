using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rentals.Domain.Tests
{
    /// <summary>
    /// Test class for the Payee entity
    /// </summary>
    [TestClass]
    public class PayeeTest
    {
        [TestMethod]
        public void CanCreatePayee()
        {
            var expected = new Payee();

            Assert.IsNotNull(expected);
        }

        [TestMethod]
        public void CanSetPayeeProperties()
        {
            var payee = new Payee();

            var name = "Payee1";
            var category = new Category { Id = 1, Name = "Category", Type = CategoryType.Income };
            var memo = "This is some memo text";
            payee.Name = name;
            payee.DefaultCategoryId = category.Id;
            payee.DefaultCategory = category;
            payee.Memo = memo;

            Assert.AreEqual(name, payee.Name);
            Assert.AreEqual(category.Id, payee.DefaultCategoryId);
            Assert.AreEqual(category, payee.DefaultCategory);
            Assert.AreEqual(memo, payee.Memo);
            Assert.AreEqual(0, payee.Transactions.Count);
        }

    }
}
