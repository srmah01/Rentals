using Rentals.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Rentals.DAL
{
    /// <summary>
    /// Represents the Account Configuartion class
    /// </summary>
    public class TransactionConfiguration : EntityTypeConfiguration<Transaction>
    {
        /// <summary>
        /// Default Constructor
        /// Also sets the precision of the Amount column.
        /// </summary>
        public TransactionConfiguration()
        {
            this.Property(t => t.Amount)
                .HasPrecision(10, 2);
        }
    }
}