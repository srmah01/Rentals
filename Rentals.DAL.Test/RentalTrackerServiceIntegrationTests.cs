using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Rentals.Domain;
using System.Data.Entity.Infrastructure;
using Rentals.DAL.Exceptions;
using System.Linq;

namespace Rentals.DAL.Tests
{
    /// <summary>
    /// Test class for the Rental Tracke rService Integration Tests.
    /// Uses the Effort Provider to create an in-memory database that allows the DbContext to tested unchanged.
    /// </summary>
    [TestClass]
    public class RentalsServiceIntegrationTests
    {
        #region Dashboard
        [TestMethod, TestCategory("Integration")]
        public void CanGetNumberOfAccounts()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            Assert.AreEqual(DataHelper.Accounts.Count, service.GetNumberOfAccounts());
        }

        [TestMethod, TestCategory("Integration")]
        public void CanGetNumberOfCategories()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            Assert.AreEqual(DataHelper.Categories.Count, service.GetNumberOfCategories());
        }

        [TestMethod, TestCategory("Integration")]
        public void CanGetNumberOfPayees()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            Assert.AreEqual(DataHelper.Payees.Count, service.GetNumberOfPayees());
        }

        [TestMethod, TestCategory("Integration")]
        public void CanGetNumberOfTransactions()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            Assert.AreEqual(DataHelper.Transactions.Count, service.GetNumberOfTransactions());
        }

        [TestMethod, TestCategory("Integration")]
        public void CanGetTotalOfAccountBalances()
        {
            DataHelper.NewDb();

            var service = new RentalsService();
            var total = 0.00m;
            foreach (var account in DataHelper.Accounts)
            {
                total += DataHelper.GetAccountBalance(account.Id);
            }

            Assert.AreEqual(total, service.GetTotalOfAccountBalances());
        }
        #endregion

        #region Accounts
        [TestMethod, TestCategory("Integration")]
        public void CanGetAnEmptyListOfAccounts()
        {
            DataHelper.NewDb(false);

            var service = new RentalsService();

            Assert.AreEqual(0, service.GetAllAccounts().Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void CanGetAListOfAccounts()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            Assert.AreEqual(DataHelper.Accounts.Count, service.GetAllAccounts().Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void CanGetAccountBalanceByAccountId()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            Assert.AreEqual(DataHelper.GetAccountBalance(1), service.GetAccountBalance(1));
            Assert.AreEqual(DataHelper.GetAccountBalance(2), service.GetAccountBalance(2));
            Assert.AreEqual(DataHelper.GetAccountBalance(3), service.GetAccountBalance(3));
        }

        [TestMethod, TestCategory("Integration")]
        public void GetAllAccountsIncludesBalances()
        {
            DataHelper.NewDb();

            var service = new RentalsService();
            var actual = new List<Account>(service.GetAllAccounts());

            Assert.AreEqual(DataHelper.Accounts.Count, actual.Count);
            Assert.AreEqual(DataHelper.GetAccountBalance(1), actual[0].Balance);
            Assert.AreEqual(DataHelper.GetAccountBalance(2), actual[1].Balance);
            Assert.AreEqual(DataHelper.GetAccountBalance(3), actual[2].Balance);
        }

        [TestMethod, TestCategory("Integration")]
        public void GettingBalanceOfNonExistentAccountReturnsZero()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            Assert.AreEqual(0.00m, service.GetAccountBalance(999));
        }

        [TestMethod, TestCategory("Integration")]
        public void CanFindAccountById()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindAccount(1);

            var expected = DataHelper.Accounts.SingleOrDefault(a => a.Id == 1);

            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.OpeningBalance, actual.OpeningBalance);
            Assert.AreEqual(DataHelper.GetAccountBalance(1), actual.Balance);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindAnAccountThatDoesNotExistReturnsNull()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindAccount(999);

            Assert.IsNull(actual);
        }

        [TestMethod, TestCategory("Integration")]
        public void CanFindAccountWithTransactionsById()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindAccountWithTransactions(1);

            var expected = DataHelper.Accounts.SingleOrDefault(a => a.Id == 1);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.OpeningBalance, actual.OpeningBalance);
            Assert.AreEqual(DataHelper.GetAccountBalance(1), actual.Balance);
            Assert.AreEqual(expected.Transactions.Count, actual.Transactions.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindingAnAccountWithTransactionsByIdAlsoReturnsABalanceForEachTransaction()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindAccountWithTransactions(1);

            var balance = actual.OpeningBalance;
            var transactions = actual.Transactions.ToArray();
            foreach (var transaction in transactions)
            {
                balance += transaction.Amount;
                Assert.AreEqual(balance, transaction.Balance);
            }
        }


        [TestMethod, TestCategory("Integration")]
        public void FindAnAccountWithTransactionsThatDoesNotExistReturnsNull()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindAccountWithTransactions(999);

            Assert.IsNull(actual);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindAccountWithTransactionsWithDateTodayReturnsEmptyTransactionCollection()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindAccountWithTransactions(1, DateTime.Today, DateTime.Today);

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Transactions.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindAccountWithTransactionsOnSortableAccountWithDateTodayReturnsTransactionCollection()
        {
            DataHelper.NewDb();
            var accountId = DataHelper.Accounts.Where(a => a.Name == "Sortable Account").SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.AccountId == accountId &&
                                                 t.Date == DateTime.Today);
            var actual = service.FindAccountWithTransactions(accountId, DateTime.Today, DateTime.Today);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count(), actual.Transactions.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindAccountWithTransactionsOnSortableAccountWithDateRangeReturnsTransactionCollection()
        {
            DataHelper.NewDb();
            var accountId = DataHelper.Accounts.Where(a => a.Name == "Sortable Account").SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.AccountId == accountId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today);
            var actual = service.FindAccountWithTransactions(accountId, DateTime.Today.AddMonths(-6), DateTime.Today);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count(), actual.Transactions.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindAccountWithTransactionsOnSortableAccountWithDateRangeAndOrderAscendingReturnsSortedTransactionCollection()
        {
            DataHelper.NewDb();
            var accountId = DataHelper.Accounts.Where(a => a.Name == "Sortable Account").SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.AccountId == accountId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today);
            var actual = service.FindAccountWithTransactions(accountId, DateTime.Today.AddMonths(-6), DateTime.Today, true);

            Assert.IsNotNull(actual);
            var order = actual.Transactions.First().Amount;
            foreach (var transaction in actual.Transactions)
            {
                Assert.AreEqual(order, transaction.Amount);
                order += 1.00m;
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void FindAccountWithTransactionsOnSortableAccountWithDateRangeAndOrderDescendingReturnsSortedTransactionCollection()
        {
            DataHelper.NewDb();
            var accountId = DataHelper.Accounts.Where(a => a.Name == "Sortable Account").SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.AccountId == accountId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today);
            var actual = service.FindAccountWithTransactions(accountId, DateTime.Today.AddMonths(-6), DateTime.Today, false);

            Assert.IsNotNull(actual);

            var order = actual.Transactions.First().Amount;
            foreach (var transaction in actual.Transactions)
            {
                Assert.AreEqual(order, transaction.Amount);
                order -= 1.00m;
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void CanInsertNewAccount()
        {
            DataHelper.NewDb();

            var accountToAdd = new Account()
            {
                Name = "BankAccount4",
                OpeningBalance = 100.99m
            };

            var service = new RentalsService();
            service.SaveNewAccount(accountToAdd);
            Assert.AreEqual(DataHelper.Accounts.Count + 1, service.GetAllAccounts().Count);
        }

        [TestMethod, TestCategory("Integration"),
            ExpectedException(typeof(RentalsServiceValidationException))]
        public void CannotInsertNewAccountWithSameNameAsAnExistingAccount()
        {
            DataHelper.NewDb();

            var accountToAdd = new Account()
            {
                Name = "BankAccount3",
                OpeningBalance = 100.99m
            };

            var service = new RentalsService();
            service.SaveNewAccount(accountToAdd);
            Assert.Fail("Added an account with same name as an exiting account");
        }

        [TestMethod, TestCategory("Integration")]
        public void CanUpdateAccount()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var account = service.FindAccount(1);
            string expected = "ChangedBankName";
            account.Name = expected;

            service.SaveUpdatedAccount(account);

            Assert.AreEqual(expected, service.FindAccount(1).Name);
        }

        [TestMethod, TestCategory("Integration"),
            ExpectedException(typeof(RentalsServiceValidationException))]
        public void CannotUpdateAnAccountNameToTheSameNameAsAnExistingAccount()
        {
            DataHelper.NewDb();

            var service = new RentalsService();
            var account = service.FindAccount(1);
            account.Name = "BankAccount3";

            service.SaveUpdatedAccount(account);

            Assert.Fail("Added an account with same name as an exiting account");
        }

        #endregion

        #region Categories
        [TestMethod, TestCategory("Integration")]
        public void CanInsertNewCategory()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var categoryToAdd = new Category()
            {
                Name = "Category Name",
                Type = CategoryType.Expense
            };

            service.SaveNewCategory(categoryToAdd);

            Assert.AreEqual(DataHelper.Categories.Count + 1, service.GetNumberOfCategories());
        }

        [TestMethod, TestCategory("Integration"),
         ExpectedException(typeof(RentalsServiceValidationException))]
        public void CannotInsertANewCategoryWithoutACategoryType()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var categoryToAdd = new Category()
            {
                Name = "Category Name"
            };

            service.SaveNewCategory(categoryToAdd);

            Assert.Fail("Category was added without a CatgeoryType");
        }

        [TestMethod, TestCategory("Integration"),
            ExpectedException(typeof(RentalsServiceValidationException))]
        public void CannotInsertNewCategorytWithSameNameAsAnExistingCategory()
        {
            DataHelper.NewDb();

            var categoryToAdd = new Category()
            {
                Name = "Rental Income",
                Type = CategoryType.Expense
            };

            var service = new RentalsService();
            service.SaveNewCategory(categoryToAdd);
            Assert.Fail("Added an category with same name as an exiting category");
        }

        [TestMethod, TestCategory("Integration")]
        public void CanFindCategoryById()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindCategory(1);

            var expected = DataHelper.Categories.SingleOrDefault(c => c.Id == 1);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Type, actual.Type);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindACategoryThatDoesNotExistReturnsNull()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindCategory(999);

            Assert.IsNull(actual);
        }

        [TestMethod, TestCategory("Integration")]
        public void CanFindCategoryByIdWithAllItsTransactions()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindCategoryWithTransactions(1);

            var expected = DataHelper.Categories.SingleOrDefault(c => c.Id == 1);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(DataHelper.Transactions.Count(t => t.CategoryId == 1), actual.Transactions.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindCategoryWithTransactionsWithDateTodayReturnsEmptyTransactionCollection()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindCategoryWithTransactions(1, DateTime.Today, DateTime.Today);

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Transactions.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindCategoryWithTransactionsOnSortableIncomeWithDateTodayReturnsTransactionCollection()
        {
            DataHelper.NewDb();
            var categoryId = DataHelper.Categories.Where(a => a.Name == "Sortable Income").SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.CategoryId == categoryId &&
                                                 t.Date == DateTime.Today);
            var actual = service.FindCategoryWithTransactions(categoryId, DateTime.Today, DateTime.Today);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count(), actual.Transactions.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindCategoryWithTransactionsOnSortableIncomeWithDateRangeReturnsTransactionCollection()
        {
            DataHelper.NewDb();
            var categoryId = DataHelper.Categories.Where(a => a.Name == "Sortable Income").SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.CategoryId == categoryId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today);
            var actual = service.FindCategoryWithTransactions(categoryId, DateTime.Today.AddMonths(-6), DateTime.Today);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count(), actual.Transactions.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindCategoryWithTransactionsOnSortableIncomeWithDateRangeAndOrderAscendingReturnsSortedTransactionCollection()
        {
            DataHelper.NewDb();
            var categoryId = DataHelper.Categories.Where(a => a.Name == "Sortable Income").SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.CategoryId == categoryId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today);
            var actual = service.FindCategoryWithTransactions(categoryId, DateTime.Today.AddMonths(-6), DateTime.Today, true);

            Assert.IsNotNull(actual);
            var order = actual.Transactions.First().Amount;
            foreach (var transaction in actual.Transactions)
            {
                Assert.AreEqual(order, transaction.Amount);
                order += 1.00m;
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void FindCategoryWithTransactionsOnSortableIncomeWithDateRangeAndOrderDescendingReturnsSortedTransactionCollection()
        {
            DataHelper.NewDb();
            var categoryId = DataHelper.Categories.Where(a => a.Name == "Sortable Income").SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.CategoryId == categoryId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today);
            var actual = service.FindCategoryWithTransactions(categoryId, DateTime.Today.AddMonths(-6), DateTime.Today, false);

            Assert.IsNotNull(actual);

            var order = actual.Transactions.First().Amount;
            foreach (var transaction in actual.Transactions)
            {
                Assert.AreEqual(order, transaction.Amount);
                order -= 1.00m;
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void CanUpdateCategory()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var category = service.FindCategory(1);
            string expected = "Changed Name";
            category.Name = expected;

            service.SaveUpdatedCategory(category);

            Assert.AreEqual(expected, service.FindCategory(1).Name);
        }

        [TestMethod, TestCategory("Integration"),
            ExpectedException(typeof(RentalsServiceValidationException))]
        public void CannotUpdateACategoryNameToTheSameNameAsAnExistingCategory()
        {
            DataHelper.NewDb();

            var service = new RentalsService();
            var category = service.FindCategory(1);
            category.Name = "Utilities";

            service.SaveUpdatedCategory(category);

            Assert.Fail("Added an category with same name as an exiting category");
        }

        #endregion

        #region Payees
        [TestMethod, TestCategory("Integration")]
        public void CanInsertNewPayeeWithExistingDefaultCategory()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var payeeToAdd = new Payee()
            {
                Name = "Payee Name",
                DefaultCategoryId = 1
            };

            service.SaveNewPayee(payeeToAdd);

            Assert.AreEqual(DataHelper.Payees.Count + 1, service.GetNumberOfPayees());
        }

        [TestMethod, TestCategory("Integration"),
         ExpectedException(typeof(DbUpdateException))]
        public void CannotInsertANewPayeeWithoutADefaultCategory()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var payeeToAdd = new Payee()
            {
                Name = "Payee Name",
            };

            service.SaveNewPayee(payeeToAdd);

            Assert.Fail("Payee was added without a DefaultCategory");
        }

        [TestMethod, TestCategory("Integration"),
            ExpectedException(typeof(RentalsServiceValidationException))]
        public void CannotInsertNewPayeetWithSameNameAsAnExistingPayee()
        {
            DataHelper.NewDb();

            var payeeToAdd = new Payee()
            {
                Name = "Renter A",
                DefaultCategoryId = 1
            };

            var service = new RentalsService();
            service.SaveNewPayee(payeeToAdd);
            Assert.Fail("Added an payee with same name as an exiting payee");
        }

        [TestMethod, TestCategory("Integration")]
        public void CanInsertNewPayeeWithANewDefaultCategoryAndBothWillBeInserted()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var payeeToAdd = new Payee()
            {
                Name = "Payee Name",
                DefaultCategory = new Category() { Name = "NewCategory", Type = CategoryType.Income }
            };

            service.SaveNewPayee(payeeToAdd);

            Assert.AreEqual(DataHelper.Payees.Count + 1, service.GetNumberOfPayees());
            Assert.AreEqual(DataHelper.Categories.Count + 1, service.GetNumberOfCategories());
        }

        [TestMethod, TestCategory("Integration")]
        public void CanFindPayeeById()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindPayee(1);

            var expected = DataHelper.Payees.SingleOrDefault(p => p.Id == 1);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.DefaultCategoryId, actual.DefaultCategoryId);
            Assert.AreEqual(expected.Memo, actual.Memo);
            Assert.IsNotNull(actual.DefaultCategory);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindAPayeeThatDoesNotExistReturnsNull()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindPayee(999);

            Assert.IsNull(actual);
        }

        [TestMethod, TestCategory("Integration")]
        public void CanFindPayeeByIdWithAllItsTransactions()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindPayeeWithTransactions(1);

            var expected = DataHelper.Payees.SingleOrDefault(p => p.Id == 1);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.DefaultCategoryId, actual.DefaultCategoryId);
            Assert.AreEqual(expected.Memo, actual.Memo);
            Assert.AreEqual(expected.Transactions.Count, actual.Transactions.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindPayeeWithTransactionsWithDateTodayReturnsEmptyTransactionCollection()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindPayeeWithTransactions(1, DateTime.Today, DateTime.Today);

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Transactions.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindPayeeWithTransactionsOnSortablePayeeWithDateTodayReturnsTransactionCollection()
        {
            DataHelper.NewDb();
            var payeeId = DataHelper.Payees.Where(a => a.Name == "Sortable Payee").SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.PayeeId == payeeId &&
                                                 t.Date == DateTime.Today);
            var actual = service.FindPayeeWithTransactions(payeeId, DateTime.Today, DateTime.Today);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count(), actual.Transactions.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindPayeeWithTransactionsOnSortablePayeeWithDateRangeReturnsTransactionCollection()
        {
            DataHelper.NewDb();
            var payeeId = DataHelper.Payees.Where(a => a.Name == "Sortable Payee").SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.PayeeId == payeeId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today);
            var actual = service.FindPayeeWithTransactions(payeeId, DateTime.Today.AddMonths(-6), DateTime.Today);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count(), actual.Transactions.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindPayeeWithTransactionsOnSortablePayeeWithDateRangeAndOrderAscendingReturnsSortedTransactionCollection()
        {
            DataHelper.NewDb();
            var payeeId = DataHelper.Payees.Where(a => a.Name == "Sortable Payee").SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.PayeeId == payeeId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today);
            var actual = service.FindPayeeWithTransactions(payeeId, DateTime.Today.AddMonths(-6), DateTime.Today, true);

            Assert.IsNotNull(actual);
            var order = actual.Transactions.First().Amount;
            foreach (var transaction in actual.Transactions)
            {
                Assert.AreEqual(order, transaction.Amount);
                order += 1.00m;
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void FindPayeeWithTransactionsOnSortablePayeeWithDateRangeAndOrderDescendingReturnsSortedTransactionCollection()
        {
            DataHelper.NewDb();
            var payeeId = DataHelper.Payees.Where(a => a.Name == "Sortable Payee").SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.PayeeId == payeeId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today);
            var actual = service.FindPayeeWithTransactions(payeeId, DateTime.Today.AddMonths(-6), DateTime.Today, false);

            Assert.IsNotNull(actual);

            var order = actual.Transactions.First().Amount;
            foreach (var transaction in actual.Transactions)
            {
                Assert.AreEqual(order, transaction.Amount);
                order -= 1.00m;
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void CanUpdatePayee()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var payee = service.FindPayee(1);
            string expected = "Changed Name";
            payee.Name = expected;

            service.SaveUpdatedPayee(payee);

            Assert.AreEqual(expected, service.FindPayee(1).Name);
        }

        [TestMethod, TestCategory("Integration"),
            ExpectedException(typeof(RentalsServiceValidationException))]
        public void CannotUpdateAPayeeNameToTheSameNameAsAnExistingPayee()
        {
            DataHelper.NewDb();

            var service = new RentalsService();
            var payee = service.FindPayee(1);
            payee.Name = "Renter B";

            service.SaveUpdatedPayee(payee);

            Assert.Fail("Added an payee with same name as an exiting payee");
        }
        #endregion

        #region Transactions
        [TestMethod, TestCategory("Integration")]
        public void GetAllTransactionsWithNoFilteringReturnsAllTransactionsCollection()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.GetAllTransactionsWithAccountAndPayeeAndCategory();

            Assert.IsNotNull(actual);
            Assert.AreEqual(DataHelper.Transactions.Count, actual.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void GetAllTransactionsWithDateTomorrowReturnsEmptyTransactionCollection()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.GetAllTransactionsWithAccountAndPayeeAndCategory(
                null, null, null,
                DateTime.Today.AddDays(1), DateTime.Today.AddDays(1));

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void GetAllTransactionsOnSortableAccountWithDateTodayReturnsTransactionCollection()
        {
            DataHelper.NewDb();
            var account = "Sortable Account";
            var accountId = DataHelper.Accounts.Where(a => a.Name == account).SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.AccountId == accountId &&
                                                 t.Date == DateTime.Today);
            var actual = service.GetAllTransactionsWithAccountAndPayeeAndCategory(
                account, null, null,
                DateTime.Today, DateTime.Today);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count(), actual.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void GetAllTransactionsOnSortableAccountWithDateRangeReturnsTransactionCollection()
        {
            DataHelper.NewDb();
            var account = "Sortable Account";
            var accountId = DataHelper.Accounts.Where(a => a.Name == account).SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.AccountId == accountId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today);
            var actual = service.GetAllTransactionsWithAccountAndPayeeAndCategory(
                account, null, null,
                DateTime.Today.AddMonths(-6), DateTime.Today);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count(), actual.Count);
        }

        [TestMethod, TestCategory("Integration")]
        public void GetAllTransactionsOnSortableAccountWithDateRangeAndOrderAscendingReturnsSortedTransactionCollection()
        {
            DataHelper.NewDb();
            var account = "Sortable Account";
            var accountId = DataHelper.Accounts.Where(a => a.Name == account).SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.AccountId == accountId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today);
            var actual = service.GetAllTransactionsWithAccountAndPayeeAndCategory(
                account, null, null,
                DateTime.Today.AddMonths(-6), DateTime.Today, true);

            Assert.IsNotNull(actual);
            var order = actual.First().Amount;
            foreach (var transaction in actual)
            {
                Assert.AreEqual(order, transaction.Amount);
                order += 1.00m;
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void GetAllTransactionsOnSortableAccountWithDateRangeAndOrderDescendingReturnsSortedTransactionCollection()
        {
            DataHelper.NewDb();
            var account = "Sortable Account";
            var accountId = DataHelper.Accounts.Where(a => a.Name == account).SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.AccountId == accountId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today);
            var actual = service.GetAllTransactionsWithAccountAndPayeeAndCategory(
                account,null, null,
                DateTime.Today.AddMonths(-6), DateTime.Today, false);

            Assert.IsNotNull(actual);

            var order = actual.First().Amount;
            foreach (var transaction in actual)
            {
                Assert.AreEqual(order, transaction.Amount);
                order -= 1.00m;
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void GetAllTransactionsOnSortablePayeeWithDateRangeAndOrderDescendingReturnsSortedTransactionCollection()
        {
            DataHelper.NewDb();
            var payee = "Sortable Payee";
            var payeeId = DataHelper.Payees.Where(a => a.Name == payee).SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.PayeeId == payeeId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today);
            var actual = service.GetAllTransactionsWithAccountAndPayeeAndCategory(
                null, payee, null,
                DateTime.Today.AddMonths(-6), DateTime.Today, false);

            Assert.IsNotNull(actual);

            var order = actual.First().Amount;
            foreach (var transaction in actual)
            {
                Assert.AreEqual(order, transaction.Amount);
                order -= 1.00m;
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void GetAllTransactionsOnSortableCategoryWithDateRangeAndOrderDescendingReturnsSortedTransactionCollection()
        {
            DataHelper.NewDb();
            var category = "Sortable Income";
            var categoryId = DataHelper.Categories.Where(a => a.Name == category).SingleOrDefault().Id;

            var service = new RentalsService();

            var expected = DataHelper.Transactions
                                     .Where(t => t.CategoryId == categoryId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today);
            var actual = service.GetAllTransactionsWithAccountAndPayeeAndCategory(
                null, null, category,
                DateTime.Today.AddMonths(-6), DateTime.Today, false);

            Assert.IsNotNull(actual);

            var order = actual.First().Amount;
            foreach (var transaction in actual)
            {
                Assert.AreEqual(order, transaction.Amount);
                order -= 1.00m;
            }
        }

        [TestMethod, TestCategory("Integration")]
        public void GetAllTransactionsWhenSearchedOnAccountAndPayeeAndCategoryReturnsSortedTransactionCollection()
        {
            DataHelper.NewDb();
            var account = "sort";
            var accountId = DataHelper.Accounts.Where(a => a.Name.ToLower().Contains(account)).SingleOrDefault().Id;
            var payee = "gas";
            var payeeId = DataHelper.Payees.Where(a => a.Name.ToLower().Contains(payee)).SingleOrDefault().Id;
            var category = "charges";
            var categoryId = DataHelper.Categories.Where(a => a.Name.ToLower().Contains(category)).SingleOrDefault().Id;

            var service = new RentalsService();

            var expectedAccountMatches = DataHelper.Transactions
                                     .Where(t => t.AccountId == accountId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today).Count();
            var expectedPayeeMatches = DataHelper.Transactions
                                     .Where(t => t.PayeeId == payeeId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today).Count();
            var expectedCategoryMatches = DataHelper.Transactions
                                     .Where(t => t.CategoryId == categoryId &&
                                                 t.Date >= DateTime.Today.AddMonths(-6) &&
                                                 t.Date <= DateTime.Today).Count();
            var actual = service.GetAllTransactionsWithAccountAndPayeeAndCategory(
                account, payee, category,
                DateTime.Today.AddMonths(-6), DateTime.Today, true);

            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedAccountMatches, actual.Count(t => t.AccountId == accountId));
            Assert.AreEqual(expectedPayeeMatches, actual.Count(t => t.PayeeId == payeeId));
            Assert.AreEqual(expectedCategoryMatches, actual.Count(t => t.CategoryId == categoryId));

        }


        [TestMethod, TestCategory("Integration")]
        public void CanInsertNewTransactionWithAllMandatoryDataPresent()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var transactionToAdd = new Transaction()
            {
                Date = DateTime.Today,
                Amount = 10.00m,
                AccountId = 1,
                PayeeId = 1,
                CategoryId = 1
            };

            service.SaveNewTransaction(transactionToAdd);

            Assert.AreEqual(DataHelper.Transactions.Count + 1, service.GetNumberOfTransactions());
        }

        [TestMethod, TestCategory("Integration")]
        public void CanInsertNewTransactionWithAllFieldsPresent()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var transactionToAdd = new Transaction()
            {
                Date = DateTime.Today,
                Amount = 10.00m,
                AccountId = 1,
                PayeeId = 1,
                CategoryId = 1,
                Reference = "Reference",
                Memo = "Memo"
            };

            service.SaveNewTransaction(transactionToAdd);

            Assert.AreEqual(DataHelper.Transactions.Count + 1, service.GetNumberOfTransactions());
        }


        [TestMethod, TestCategory("Integration"),
         ExpectedException(typeof(DbUpdateException))]
        public void CannotInsertANewTransactionWithoutAnAccount()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var transactionToAdd = new Transaction()
            {
                Date = DateTime.Today,
                Amount = 10.00m,
                PayeeId = 1,
                CategoryId = 1,
                Reference = "Reference",
                Memo = "Memo"
            };

            service.SaveNewTransaction(transactionToAdd);

            Assert.Fail("Transaction was added without an Account");
        }

        [TestMethod, TestCategory("Integration"),
         ExpectedException(typeof(DbUpdateException))]
        public void CannotInsertANewTransactionWithoutAPayee()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var transactionToAdd = new Transaction()
            {
                Date = DateTime.Today,
                Amount = 10.00m,
                AccountId = 1,
                CategoryId = 1,
                Reference = "Reference",
                Memo = "Memo"
            };

            service.SaveNewTransaction(transactionToAdd);

            Assert.Fail("Transaction was added without a Payee");
        }

        [TestMethod, TestCategory("Integration"),
         ExpectedException(typeof(DbUpdateException))]
        public void CannotInsertANewTransactionWithoutACategory()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var transactionToAdd = new Transaction()
            {
                Date = DateTime.Today,
                Amount = 10.00m,
                AccountId = 1,
                PayeeId = 1,
                Reference = "Reference",
                Memo = "Memo"
            };

            service.SaveNewTransaction(transactionToAdd);

            Assert.Fail("Transaction was added without a Category");
        }

        [TestMethod, TestCategory("Integration"),
         ExpectedException(typeof(RentalsServiceValidationException))]
        public void CannotInsertANewTransactionWithoutADate()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var transactionToAdd = new Transaction()
            {
                Amount = 10.00m,
                AccountId = 1,
                PayeeId = 1,
                CategoryId = 1,
                Reference = "Reference",
                Memo = "Memo"
            };

            service.SaveNewTransaction(transactionToAdd);

            Assert.Fail("Transaction was added without a Date");
        }

        [TestMethod, TestCategory("Integration"),
         ExpectedException(typeof(RentalsServiceValidationException))]
        public void CannotInsertANewTransactionWithoutAnAmount()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var transactionToAdd = new Transaction()
            {
                Date = DateTime.Today,
                AccountId = 1,
                PayeeId = 1,
                CategoryId = 1,
                Reference = "Reference",
                Memo = "Memo"
            };

            service.SaveNewTransaction(transactionToAdd);

            Assert.Fail("Transaction was added without an Amount");
        }

        [TestMethod, TestCategory("Integration")]
        public void CanInsertNewTransactionWithANewAccountCategoryAndPayeeAndAllWillBeInserted()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var transactionToAdd = new Transaction()
            {
                Date = DateTime.Today,
                Amount = 10.00m,
                Account = new Account() { Name = "New Accoount", OpeningBalance = 0.00m },
                Payee = new Payee() { Name = "New Payee", DefaultCategoryId = 1 },
                Category = new Category() { Name = "New Category", Type = CategoryType.Income },
                Reference = "Reference",
                Memo = "Memo"
            };

            service.SaveNewTransaction(transactionToAdd);

            Assert.AreEqual(DataHelper.Transactions.Count + 1, service.GetNumberOfTransactions());
            Assert.AreEqual(DataHelper.Accounts.Count + 1, service.GetNumberOfAccounts());
            Assert.AreEqual(DataHelper.Payees.Count + 1, service.GetNumberOfPayees());
            Assert.AreEqual(DataHelper.Categories.Count + 1, service.GetNumberOfCategories());
        }

        [TestMethod, TestCategory("Integration")]
        public void CanFindTransactionById()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindTransaction(1);

            var expected = DataHelper.Transactions.SingleOrDefault(t => t.Id == 1);
            Assert.AreEqual(expected.Date, actual.Date);
            Assert.AreEqual(expected.Amount, actual.Amount);
            Assert.AreEqual(expected.AccountId, actual.AccountId);
            Assert.AreEqual(expected.PayeeId, actual.PayeeId);
            Assert.AreEqual(expected.CategoryId, actual.CategoryId);
            Assert.AreEqual(expected.Reference, actual.Reference);
            Assert.AreEqual(expected.Memo, actual.Memo);
        }

        [TestMethod, TestCategory("Integration")]
        public void FindATransactionThatDoesNotExistReturnsNull()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindTransaction(999);

            Assert.IsNull(actual);
        }

        [TestMethod, TestCategory("Integration")]
        public void CanFindTransactionWithAccountAndPayeeAndCategoryById()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var actual = service.FindTransactionWithAccountAndPayeeAndCategory(1);

            var expected = DataHelper.Transactions.SingleOrDefault(t => t.Id == 1);
            Assert.AreEqual(expected.Date, actual.Date);
            Assert.AreEqual(expected.Amount, actual.Amount);
            Assert.AreEqual(expected.Account.Name, actual.Account.Name);
            Assert.AreEqual(expected.Payee.Name, actual.Payee.Name);
            Assert.AreEqual(expected.Category.Name, actual.Category.Name);
            Assert.AreEqual(expected.Reference, actual.Reference);
            Assert.AreEqual(expected.Memo, actual.Memo);
        }

        [TestMethod, TestCategory("Integration")]
        public void CanUpdateTransaction()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            var transaction = service.FindTransaction(1);
            DateTime expected = DateTime.Today;
            transaction.Date = expected;

            service.SaveUpdatedTransaction(transaction);

            Assert.AreEqual(expected, service.FindTransaction(1).Date);
        }

        [TestMethod, TestCategory("Integration"),
            ExpectedException(typeof(RentalsServiceValidationException))]
        public void CannotUpdateATransactionToHaveAnAmountOfZero()
        {
            DataHelper.NewDb();

            var service = new RentalsService();
            var transaction = service.FindTransaction(1);
            transaction.Amount = 0.00m;

            service.SaveUpdatedTransaction(transaction);

            Assert.Fail("Added an transaction with an amount of zero");
        }

        [TestMethod, TestCategory("Integration")]
        public void CanDeleteTransaction()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            service.RemoveTransaction(1);

            Assert.AreEqual(DataHelper.Transactions.Count - 1, service.GetAllTransactionsWithAccountAndPayeeAndCategory().Count);
        }

        [TestMethod, TestCategory("Integration")
            ExpectedException(typeof(ArgumentNullException))]
        public void CannotDeleteTransactionThatDoesNotExist()
        {
            DataHelper.NewDb();

            var service = new RentalsService();

            service.RemoveTransaction(999);

            Assert.Fail("Deleted a non-existent transaction");
        }

        #endregion
    }
}
