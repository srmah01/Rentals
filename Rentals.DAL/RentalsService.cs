using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentals.Domain;
using System.Data.Entity;

namespace Rentals.DAL
{
    /// <summary>
    /// The implemention of the Rental Tracker Service
    /// </summary>
    public class RentalsService : IRentals
    {
        #region Dashboard

        /// <summary>
        /// <see cref="IRentals.GetNumberOfAccounts"/>
        /// </summary>
        public int GetNumberOfAccounts()
        {
            using (var context = new RentalsContext())
            {
                return context.Accounts.Count();
            }
        }

        /// <summary>
        /// <see cref="IRentals.GetNumberOfCategories()"/>
        /// </summary>
        public int GetNumberOfCategories()
        {
            using (var context = new RentalsContext())
            {
                return context.Categories.Count();
            }
        }

        /// <summary>
        /// <see cref="IRentals.GetNumberOfPayees()"/>
        /// </summary>
        public int GetNumberOfPayees()
        {
            using (var context = new RentalsContext())
            {
                return context.Payees.Count();
            }
        }

        /// <summary>
        /// <see cref="IRentals.GetNumberOfTransactions()"/>
        /// </summary>
        public int GetNumberOfTransactions()
        {
            using (var context = new RentalsContext())
            {
                return context.Transactions.Count();
            }
        }

        /// <summary>
        /// <see cref="IRentals.GetTotalOfAccountBalances()"/>
        /// </summary>
        public Decimal GetTotalOfAccountBalances()
        {
            using (var context = new RentalsContext())
            {
                var balance = 0.0m;

                foreach (var account in context.Accounts)
                {
                    balance += GetAccountBalance(account.Id);
                }

                return balance;
            }
        }

        #endregion

        #region Accounts

        /// <summary>
        /// <see cref="IRentals.GetAllAccounts()"/>
        /// </summary>
        public ICollection<Account> GetAllAccounts()
        {
            using (var context = new RentalsContext())
            {
                var accounts = context.Accounts.AsNoTracking().ToList();
                foreach (var account in accounts)
                {
                    account.Balance = GetAccountBalance(account.Id);
                }

                return accounts;
            }
        }

        /// <summary>
        /// <see cref="IRentals.FindAccount(int?)"/>
        /// </summary>
        public Account FindAccount(int? id)
        {
            using (var context = new RentalsContext())
            {
                var account = context.Accounts.AsNoTracking()
                                       .SingleOrDefault(a => a.Id == id);
                if (account != null)
                {
                    account.Balance = GetAccountBalance(id);
                }

                return account;
            }
        }

        /// <summary>
        /// <see cref="IRentals.FindAccountWithTransactions(int?, DateTime?, DateTime?, bool)"/>
        /// </summary>
        public Account FindAccountWithTransactions(int? id, DateTime? from = null, DateTime? to = null, bool ascending = true)
        {
            using (var context = new RentalsContext())
            {
                var account = context.Accounts
                                     .SingleOrDefault(a => a.Id == id);

                if (account != null)
                {
                    context.Accounts.Attach(account);
                    var transactions = context.Entry(account)
                                    .Collection(a => a.Transactions)
                                    .Query()
                                    .Include(t => t.Payee)
                                    .Include(t => t.Category)
                                    .OrderBy(t => t.Date)
                                    .ThenBy(t => t.Id);

                    CalculateTransactionBalances(transactions, account.OpeningBalance);

                    if (from == null)
                    {
                        from = DateTime.MinValue;
                    }

                    if (to == null)
                    {
                        to = DateTime.MaxValue;
                    }

                    if (ascending)
                    {
                        account.Transactions = transactions
                                               .Where(t => t.Date >= from && t.Date <= to)
                                               .ToList();
                    }
                    else
                    {
                        account.Transactions = transactions
                                               .Where(t => t.Date >= from && t.Date <= to)
                                               .OrderByDescending(t => t.Date)
                                               .ThenByDescending(t => t.Id)
                                               .ToList();
                    }

                    account.Balance = GetAccountBalance(id);
                }

                return account;
            }
        }

        /// <summary>
        /// <see cref="IRentals.SaveNewAccount(Account)"/>
        /// </summary>
        public void SaveNewAccount(Account account)
        {
            using (var context = new RentalsContext())
            {
                context.Accounts.Add(account);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// <see cref="IRentals.SaveUpdatedAccount(Account)"/>
        /// </summary>
        public void SaveUpdatedAccount(Account account)
        {
            using (var context = new RentalsContext())
            {
                context.Entry(account).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// <see cref="IRentals.GetAccountBalance(int?)"/>
        /// </summary>
        public Decimal GetAccountBalance(int? id)
        {
            using (var context = new RentalsContext())
            {
                var openingBalance = (from a in context.Accounts
                                      where a.Id == id
                                      select a.OpeningBalance).FirstOrDefault();

                // Generates SQL that gets only the specific column
                var amounts = (from t in context.Transactions
                               where t.AccountId == id
                               select new { Amount = t.Amount, CategoryType = t.Category.Type }).ToList();

                var income = amounts.Where(a => a.CategoryType == CategoryType.Income).ToList().Sum(a => a.Amount);

                var expense = amounts.Where(a => a.CategoryType == CategoryType.Expense).ToList().Sum(a => a.Amount);

                return (openingBalance + income - expense);
            }
        }

        /// <summary>
        /// Calculate the balance for each Transaction in a given collection of transactions.
        /// </summary>
        /// <param name="transactions">The given collection of Transactions.</param>
        /// <param name="openingBalance">The Opening Balance of the Account.</param>
        private void CalculateTransactionBalances(IQueryable<Transaction> transactions, Decimal openingBalance)
        {
            var balance = openingBalance;

            foreach (var transaction in transactions)
            {
                balance += (transaction.Amount * (transaction.Category.Type == CategoryType.Income ? 1 : -1));
                transaction.Balance = balance;
            }
        }

        #endregion

        #region Categories

        /// <summary>
        /// <see cref="IRentals.GetAllCategories()"/>
        /// </summary>
        public ICollection<Category> GetAllCategories()
        {
            using (var context = new RentalsContext())
            {
                return context.Categories.AsNoTracking().ToList();
            }
        }

        /// <summary>
        /// <see cref="IRentals.FindCategory(int?)"/>
        /// </summary>
        public Category FindCategory(int? id)
        {
            using (var context = new RentalsContext())
            {
                return context.Categories.AsNoTracking()
                                        .SingleOrDefault(c => c.Id == id);
            }
        }

        /// <summary>
        /// <see cref="IRentals.FindCategory(int?, DateTime?, DateTime?, bool)"/>
        /// </summary>
        public Category FindCategoryWithTransactions(int? id, DateTime? from = null, DateTime? to = null, bool ascending = true)
        {
            using (var context = new RentalsContext())
            {
                var category = context.Categories
                                     .SingleOrDefault(a => a.Id == id);

                if (category != null)
                {
                    context.Categories.Attach(category);

                    if (from == null)
                    {
                        from = DateTime.MinValue;
                    }

                    if (to == null)
                    {
                        to = DateTime.MaxValue;
                    }

                    if (ascending)
                    {
                        context.Entry(category)
                               .Collection(c => c.Transactions)
                               .Query()
                               .Include(t => t.Account)
                               .Include(t => t.Payee)
                               .Where(t => t.Date >= from && t.Date <= to)
                               .OrderBy(t => t.Date)
                               .ThenBy(t => t.Id)
                               .Load();
                    }
                    else
                    {
                        context.Entry(category)
                               .Collection(c => c.Transactions)
                               .Query()
                               .Include(t => t.Account)
                               .Include(t => t.Payee)
                               .Where(t => t.Date >= from && t.Date <= to)
                               .OrderByDescending(t => t.Date)
                               .ThenByDescending(t => t.Id)
                               .Load();
                    }
                }

                return category;
            }
        }

        /// <summary>
        /// <see cref="IRentals.SaveNewCategory(Category)"/>
        /// </summary>
        public void SaveNewCategory(Category category)
        {
            using (var context = new RentalsContext())
            {
                context.Categories.Add(category);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// <see cref="IRentals.SaveUpdatedCategory(Category)"/>
        /// </summary>
        public void SaveUpdatedCategory(Category category)
        {
            using (var context = new RentalsContext())
            {
                context.Entry(category).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        #endregion

        #region Payees

        /// <summary>
        /// <see cref="IRentals.GetAllPayees()"/>
        /// </summary>
        public ICollection<Payee> GetAllPayees()
        {
            using (var context = new RentalsContext())
            {
                return context.Payees
                    .AsNoTracking()
                    .Include(p => p.DefaultCategory)
                    .AsNoTracking()
                    .ToList();
            }
        }

        /// <summary>
        /// <see cref="IRentals.FindPayee(int?)"/>
        /// </summary>
        public Payee FindPayee(int? id)
        {
            using (var context = new RentalsContext())
            {
                return context.Payees.AsNoTracking()
                                     .Include(p => p.DefaultCategory)
                                     .SingleOrDefault(p => p.Id == id);
            }
        }

        /// <summary>
        /// <see cref="IRentals.FindPayeeWithTransactions(int?, DateTime?, DateTime?, bool)"/>
        /// </summary>
        public Payee FindPayeeWithTransactions(int? id, DateTime? from = null, DateTime? to = null, bool ascending = true)
        {
            using (var context = new RentalsContext())
            {
                var payee = context.Payees
                                   .Include(p => p.DefaultCategory)
                                   .SingleOrDefault(a => a.Id == id);

                if (payee != null)
                {
                    context.Payees.Attach(payee);

                    if (from == null)
                    {
                        from = DateTime.MinValue;
                    }

                    if (to == null)
                    {
                        to = DateTime.MaxValue;
                    }

                    if (ascending)
                    {
                        context.Entry(payee)
                               .Collection(c => c.Transactions)
                               .Query()
                               .Include(t => t.Account)
                               .Include(t => t.Category)
                               .Where(t => t.Date >= from && t.Date <= to)
                               .OrderBy(t => t.Date)
                               .ThenBy(t => t.Id)
                               .Load();
                    }
                    else
                    {
                        context.Entry(payee)
                               .Collection(c => c.Transactions)
                               .Query()
                               .Include(t => t.Account)
                               .Include(t => t.Category)
                               .Where(t => t.Date >= from && t.Date <= to)
                               .OrderByDescending(t => t.Date)
                               .ThenByDescending(t => t.Id)
                               .Load();
                    }
                }

                return payee;
            }
        }

        /// <summary>
        /// <see cref="IRentals.SaveNewPayee(Payee)"/>
        /// </summary>
        public void SaveNewPayee(Payee payee)
        {
            using (var context = new RentalsContext())
            {
                context.Payees.Add(payee);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// <see cref="IRentals.SaveUpdatedPayee(Payee)"/>
        /// </summary>
        public void SaveUpdatedPayee(Payee payee)
        {
            using (var context = new RentalsContext())
            {
                context.Entry(payee).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        #endregion

        #region Transactions

        /// <summary>
        /// <see cref="IRentals.GetAllTransactionsWithAccountAndPayeeAndCategory(String, String, String, DateTime?, DateTime?, bool)"/>
        /// </summary>
        public ICollection<Transaction> GetAllTransactionsWithAccountAndPayeeAndCategory(
            String account = null, String payee = null, String category = null,
            DateTime? from = null, DateTime? to = null, bool ascending = true)
        {
            using (var context = new RentalsContext())
            {
                var transactions = context.Transactions.AsNoTracking()
                                                       .Include(t => t.Account)
                                                       .Include(t => t.Payee)
                                                       .Include(t => t.Category)
                                                       .AsQueryable();

                if (from == null)
                {
                    from = DateTime.MinValue;
                }

                if (to == null)
                {
                    to = DateTime.MaxValue;
                }

                if (!String.IsNullOrEmpty(account) || !String.IsNullOrEmpty(payee) || !String.IsNullOrEmpty(category))
                {
                    // Make search an OR operation - any of the matching search terms is included in the results
                    transactions = transactions.Where(t =>
                        (!String.IsNullOrEmpty(account) && t.Account.Name.ToLower().Contains(account.ToLower())) ||
                        (!String.IsNullOrEmpty(payee) && t.Payee.Name.ToLower().Contains(payee.ToLower())) ||
                        (!String.IsNullOrEmpty(category) && t.Category.Name.ToLower().Contains(category.ToLower()))
                        );
                }

                if (ascending)
                {
                    transactions = transactions.AsQueryable()
                                    .Where(t => t.Date >= from && t.Date <= to)
                                    .OrderBy(t => t.Date)
                                    .ThenBy(t => t.Id);
                }
                else
                {
                    transactions = transactions.AsQueryable()
                                    .Where(t => t.Date >= from && t.Date <= to)
                                    .OrderByDescending(t => t.Date)
                                    .ThenByDescending(t => t.Id);
                }


                return transactions.ToList();
            }
        }

        /// <summary>
        /// <see cref="IRentals.FindTransaction(int?)"/>
        /// </summary>
        public Transaction FindTransaction(int? id)
        {
            using (var context = new RentalsContext())
            {
                return context.Transactions.AsNoTracking()
                                           .SingleOrDefault(p => p.Id == id);
            }
        }

        /// <summary>
        /// <see cref="IRentals.FindTransactionWithAccountAndPayeeAndCategory(int?)"/>
        /// </summary>
        public Transaction FindTransactionWithAccountAndPayeeAndCategory(int? id)
        {
            using (var context = new RentalsContext())
            {
                return context.Transactions.AsNoTracking()
                                           .Include(t => t.Account)
                                           .Include(t => t.Payee)
                                           .Include(t => t.Category)
                                           .SingleOrDefault(p => p.Id == id);
            }
        }

        /// <summary>
        /// <see cref="IRentals.SaveNewTransaction(Transaction)"/>
        /// </summary>
        public void SaveNewTransaction(Transaction transaction)
        {
            using (var context = new RentalsContext())
            {
                context.Transactions.Add(transaction);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// <see cref="IRentals.SaveUpdatedTransaction(Transaction)"/>
        /// </summary>
        public void SaveUpdatedTransaction(Transaction transaction)
        {
            using (var context = new RentalsContext())
            {
                context.Entry(transaction).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// <see cref="IRentals.RemoveTransaction(Transaction)"/>
        /// </summary>
        public void RemoveTransaction(int id)
        {
            using (var context = new RentalsContext())
            {
                Transaction transaction = context.Transactions.Find(id);
                context.Transactions.Remove(transaction);
                context.SaveChanges();
            }
        }

        #endregion
    }
}
