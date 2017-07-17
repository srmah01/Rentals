using Rentals.Domain;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.ComponentModel.DataAnnotations;
using Rentals.DAL.Exceptions;

namespace Rentals.DAL
{
    /// <summary>
    /// Context of the Rental Tracker database
    /// </summary>
    public class RentalsContext : DbContext
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public RentalsContext() : base("Rentals")
        {
            // Turn off Lazy Loading as we want to control what data is returned and when.
            this.Configuration.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Represents the collection of all entities in the context, 
        /// or that can be queried from the database, of the Account type.
        /// </summary>
        public virtual DbSet<Account> Accounts { get; set; }

        /// <summary>
        /// Represents the collection of all entities in the context, 
        /// or that can be queried from the database, of the Payee type.
        /// </summary>
        public virtual DbSet<Payee> Payees { get; set; }

        /// <summary>
        /// Represents the collection of all entities in the context, 
        /// or that can be queried from the database, of the Category type.
        /// </summary>
        public virtual DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Represents the collection of all entities in the context, 
        /// or that can be queried from the database, of the Transaction type.
        /// </summary>
        public virtual DbSet<Transaction> Transactions { get; set; }

        /// <summary>
        /// Override to complete the configuration of the derived DbContext.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Remove cascade deletions.
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Configurations.Add(new AccountConfiguration());
            modelBuilder.Configurations.Add(new TransactionConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>The number of objects written to the underlying database.</returns>
        /// <exception cref="RentalsServiceValidationException">
        /// Throws an exception with a collection of ValidationResults.
        /// </exception>
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new RentalsServiceValidationException with an improved exception message and
                // a collection of all the validation errors
                throw new RentalsServiceValidationException(exceptionMessage, GetErrors(ex.EntityValidationErrors));
            }
        }
        /// <summary>
        /// Custom validation of the entities. Called by GetValidationErrors.
        /// </summary>
        /// <param name="entityEntry">DbEntityEntry instance to be validated.</param>
        /// <param name="items">User-defined dictionary containing additional info for custom validation. It will be passed to ValidationContext and will be exposed as Items.
        /// This parameter is optional and can be null.</param>
        /// <returns>Entity validation result</returns>
        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            var result = new DbEntityValidationResult(entityEntry, new List<DbValidationError>());
            if ((entityEntry.State == EntityState.Added) ||
                (entityEntry.State == EntityState.Modified))
            {
                if (entityEntry.Entity is Account)
                {
                    ValidateAccountEntry(entityEntry, result);
                }

                if (entityEntry.Entity is Category)
                {
                    ValidateCategoryEntry(entityEntry, result);
                }

                if (entityEntry.Entity is Payee)
                {
                    ValidatePayeeEntry(entityEntry, result);
                }
            }

            if (result.ValidationErrors.Count > 0)
            {
                return result;
            }
            else
            {
                return base.ValidateEntity(entityEntry, items);
            }
        }

        /// <summary>
        /// Validate an Account entity
        /// </summary>
        /// <param name="entityEntry">The Account entity to be validated.</param>
        /// <param name="result">The collection of results to add new errors to.</param>
        private void ValidateAccountEntry(DbEntityEntry entityEntry, DbEntityValidationResult result)
        {
            Account account = entityEntry.Entity as Account;
            // Check for uniqueness of Name
            if ((entityEntry.State == EntityState.Added && Accounts.Count(a => (a.Name == account.Name)) > 0) ||
                (entityEntry.State == EntityState.Modified && Accounts.Count(a => a.Name == account.Name && (a.Id != account.Id)) >= 1))
            {
                result.ValidationErrors.Add(
                        new DbValidationError("Name", "Account name must be unique."));
            }
        }

        /// <summary>
        /// Validate a Category entity
        /// </summary>
        /// <param name="entityEntry">The Category entity to be validated.</param>
        /// <param name="result">The collection of results to add new errors to.</param>
        private void ValidateCategoryEntry(DbEntityEntry entityEntry, DbEntityValidationResult result)
        {
            Category category = entityEntry.Entity as Category;
            // Check for uniqueness of Name
            if ((entityEntry.State == EntityState.Added && Categories.Count(c => (c.Name == category.Name)) > 0) ||
                (entityEntry.State == EntityState.Modified && Categories.Count(c => c.Name == category.Name && (c.Id != category.Id)) >= 1))
            {
                result.ValidationErrors.Add(
                        new DbValidationError("Name", "Category name must be unique."));
            }
        }

        /// <summary>
        /// Validate a Payee entity
        /// </summary>
        /// <param name="entityEntry">The Payee entity to be validated.</param>
        /// <param name="result">The collection of results to add new errors to.</param>
        private void ValidatePayeeEntry(DbEntityEntry entityEntry, DbEntityValidationResult result)
        {
            Payee payee = entityEntry.Entity as Payee;
            // Check for uniqueness of Name
            if ((entityEntry.State == EntityState.Added && Payees.Count(p => (p.Name == payee.Name)) > 0) ||
                (entityEntry.State == EntityState.Modified && Payees.Count(p => p.Name == payee.Name && (p.Id != payee.Id)) >= 1))
            {
                result.ValidationErrors.Add(
                        new DbValidationError("Name", "Payee name must be unique."));
            }
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <param name="errors">The collection of validation results.</param>
        /// <returns>An enumerable list of error results.</returns>
        private IEnumerable<ValidationResult> GetErrors(IEnumerable<DbEntityValidationResult> errors)
        {
            return errors.SelectMany(
                        x => x.ValidationErrors.Select(y =>
                              new ValidationResult(y.ErrorMessage, new[] { y.PropertyName })))
                        .ToList();
        }

    }
}
