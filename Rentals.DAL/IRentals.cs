using Rentals.Domain;
using System;
using System.Collections.Generic;

namespace Rentals.DAL
{
    /// <summary>
    /// Describes the interface to the Rental Tracker service.
    /// </summary>
    public interface IRentals
    {
        #region Dashboard
 
        /// <summary>
        /// Gets the number of Account entities.
        /// </summary>
        /// <returns>The number of Accounts</returns>
        int GetNumberOfAccounts();

        /// <summary>
        /// Gets the number of Category entities.
        /// </summary>
        /// <returns>The number of Categories.</returns>
        int GetNumberOfCategories();

        /// <summary>
        /// Gets the number of Payee entities.
        /// </summary>
        /// <returns>The number of Payees.</returns>
        int GetNumberOfPayees();

        /// <summary>
        /// Gets the number of Transaction entities.
        /// </summary>
        /// <returns>The number of Transactions.</returns>
        int GetNumberOfTransactions();

        /// <summary>
        /// Gets the total of all Account Balances.
        /// </summary>
        /// <returns>The total of balance of all Accounts.</returns>
        Decimal GetTotalOfAccountBalances();

        #endregion

        #region Accounts

        /// <summary>
        /// Get a collection of all of the Account entities.
        /// </summary>
        /// <returns>The collection of Accounts.</returns>
        ICollection<Account> GetAllAccounts();

        /// <summary>
        /// Get the balance of the specified Account.
        /// </summary>
        /// <param name="id">The id of the specified Account</param>
        /// <returns>The Balance of the Account</returns>
        Decimal GetAccountBalance(int? id);

        /// <summary>
        /// Get an Account by it's id.
        /// </summary>
        /// <param name="id">The id of the specified Account.</param>
        /// <returns>The Account entity.</returns>
        Account FindAccount(int? id);

        /// <summary>
        /// Get an Account, by it's id.
        /// Include with the Account a sorted collection of Transactions that meet the specified criteria.
        /// </summary>
        /// <param name="id">The id of the specified Account.</param>
        /// <param name="from">Date of earliest Transaction to be included.</param>
        /// <param name="to">Date of latest Transaction to be included.</param>
        /// <param name="ascending">Order to sort the collection. True means earliest Transactions first.</param>
        /// <returns>The Account entity.</returns>
        Account FindAccountWithTransactions(int? id, DateTime? from = null, DateTime? to =null, bool ascending = true);

        /// <summary>
        /// Save a new Account to the database.
        /// </summary>
        /// <param name="account">The Account to be saved.</param>
        void SaveNewAccount(Account account);

        /// <summary>
        /// Save an updated Account to the database.
        /// </summary>
        /// <param name="account">The updated Account to be saved.</param>
        void SaveUpdatedAccount(Account account);

        #endregion

        #region Categories

        /// <summary>
        /// Get a collection of all of the Category entities.
        /// </summary>
        /// <returns>The collection of Categories.</returns>
        ICollection<Category> GetAllCategories();

        /// <summary>
        /// Get an Category by it's id.
        /// </summary>
        /// <param name="id">The id of the specified Category.</param>
        /// <returns>The Category entity.</returns>
        Category FindCategory(int? id);

        /// <summary>
        /// Get an Category, by it's id.
        /// Include with the Category a sorted collection of Transactions that meet the specified criteria.
        /// </summary>
        /// <param name="id">The id of the specified Category.</param>
        /// <param name="from">Date of earliest Transaction to be included.</param>
        /// <param name="to">Date of latest Transaction to be included.</param>
        /// <param name="ascending">Order to sort the collection. True means earliest Transactions first.</param>
        /// <returns>The Category entity.</returns>
        Category FindCategoryWithTransactions(int? id, DateTime? from = null, DateTime? to = null, bool ascending = true);

        /// <summary>
        /// Save a new Category to the database.
        /// </summary>
        /// <param name="category">The Category to be saved.</param>
        void SaveNewCategory(Category category);

        /// <summary>
        /// Save an updated Category to the database.
        /// </summary>
        /// <param name="category">The updated Category to be saved.</param>
        void SaveUpdatedCategory(Category category);

        #endregion

        #region Payees

        /// <summary>
        /// Get a collection of all of the Payee entities.
        /// </summary>
        /// <returns>The collection of Payees.</returns>
        ICollection<Payee> GetAllPayees();

        /// <summary>
        /// Get an Payee by it's id.
        /// </summary>
        /// <param name="id">The id of the specified Payee.</param>
        /// <returns>The Payee entity.</returns>
        Payee FindPayee(int? id);

        /// <summary>
        /// Get an Payee, by it's id.
        /// Include with the Payee a sorted collection of Transactions that meet the specified criteria
        /// </summary>
        /// <param name="id">The id of the specified Payee.</param>
        /// <param name="from">Date of earliest Transaction to be included.</param>
        /// <param name="to">Date of latest Transaction to be included.</param>
        /// <param name="ascending">Order to sort the collection. True means earliest Transactions first.</param>
        /// <returns>The Payee entity.</returns>
        Payee FindPayeeWithTransactions(int? id, DateTime? from = null, DateTime? to = null, bool ascending = true);

        /// <summary>
        /// Save a new Payee in the database.
        /// </summary>
        /// <param name="payee">The Payee to be saved.</param>
        void SaveNewPayee(Payee payee);

        /// <summary>
        /// Save an updated Payee in the database.
        /// </summary>
        /// <param name="payee">The updated Payee to be saved.</param>
        void SaveUpdatedPayee(Payee payee);

        #endregion

        #region Transactions

        /// <summary>
        /// Get a sorted collection of Transactions, that meet the specified criteria.
        /// </summary>
        /// <param name="account">Include any Transaction that contains account text in any part of the Account Name</param>
        /// <param name="payee">Include any Transaction that contains payee text in any part of the Payee Name</param>
        /// <param name="category">Include any Transaction that contains category text in any part of the Category Name</param>
        /// <param name="from">Date of earliest Transaction to be included.</param>
        /// <param name="to">Date of latest Transaction to be included.</param>
        /// <param name="ascending">Order to sort the collection. True means earliest Transactions first.</param>
        /// <returns>The collection of Transaction entities.</returns>
        ICollection<Transaction> GetAllTransactionsWithAccountAndPayeeAndCategory(
            String account = null, String payee = null, String category = null,
            DateTime? from = null, DateTime? to = null, bool ascending = true);

        /// <summary>
        /// Get an Transaction by it's id.
        /// </summary>
        /// <param name="id">The id of the specified Transaction.</param>
        /// <returns>The Transaction entity.</returns>
        Transaction FindTransaction(int? id);

        /// <summary>
        /// Get an Transaction by it's id, and include the Account, Payee and Category details.
        /// </summary>
        /// <param name="id">The id of the specified Transaction.</param>
        /// <returns>The Transaction entity.</returns>
        Transaction FindTransactionWithAccountAndPayeeAndCategory(int? id);

        /// <summary>
        /// Save a new Transaction to the database.
        /// </summary>
        /// <param name="transaction">The Transaction to be saved.</param>
        void SaveNewTransaction(Transaction transaction);

        /// <summary>
        /// Save an updated Transaction to the database.
        /// </summary>
        /// <param name="transaction">The updated Transaction to be saved</param>
        void SaveUpdatedTransaction(Transaction transaction);

        /// <summary>
        /// Remove a Transaction from the database, by it's id.
        /// </summary>
        /// <param name="account">he id of the specified Transaction.</param>
        void RemoveTransaction(int id);

        #endregion
    }
}
