using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Rentals.Domain.Tests
{
    /// <summary>
    /// Test class for the Transaction entity
    /// </summary>
    [TestClass]
    public class TransactionTest
    {
        [TestMethod]
        public void CanCreateTransaction()
        {
            var expected = new Transaction();

            Assert.IsNotNull(expected);
        }

        [TestMethod]
        public void CanSetTransactionProperties()
        {
            var transaction = new Transaction();

            var account = new Account { Id = 1, Name = "Account", OpeningBalance = 1.00m };
            var category = new Category { Id = 1, Name = "Category", Type = CategoryType.Income };
            var payee = new Payee { Id = 1, Name = "Payee", DefaultCategory = category };
            var memo = "This is some memo text";
            var amount = 1.00m;
            transaction.AccountId = account.Id;
            transaction.Account = account;
            transaction.PayeeId = payee.Id;
            transaction.Payee = payee;
            transaction.CategoryId = category.Id;
            transaction.Category = category;
            transaction.Date = DateTime.Today;
            transaction.Amount = amount;
            transaction.Balance = amount;
            transaction.Memo = memo;

            Assert.AreEqual(account.Id, transaction.AccountId);
            Assert.AreEqual(account, transaction.Account);
            Assert.AreEqual(payee.Id, transaction.PayeeId);
            Assert.AreEqual(payee, transaction.Payee);
            Assert.AreEqual(category.Id, transaction.CategoryId);
            Assert.AreEqual(category, transaction.Category);
            Assert.AreEqual(DateTime.Today, transaction.Date);
            Assert.AreEqual(amount, transaction.Amount);
            Assert.AreEqual(amount, transaction.Balance);
            Assert.AreEqual(memo, transaction.Memo);
        }

    }
}
