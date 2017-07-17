using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rentals.Domain.Tests
{
    /// <summary>
    /// Test class for the Account entity
    /// </summary>
    [TestClass]
    public class AccountTest
    {
        [TestMethod]
        public void CanCreateAccount()
        {
            var expected = new Account();

            Assert.IsNotNull(expected);
        }

        [TestMethod]
        public void CanSetAccountProperties()
        {
            var account = new Account();

            var name = "Account1";
            var amount = 10.00m;
            account.Name = name;
            account.OpeningBalance = amount;
            account.Balance = amount;

            Assert.AreEqual(name, account.Name);
            Assert.AreEqual(amount, account.OpeningBalance);
            Assert.AreEqual(amount, account.Balance);
            Assert.AreEqual(0, account.Transactions.Count);
        }
    }
}
