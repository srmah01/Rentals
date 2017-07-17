using Rentals.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Rentals.DAL
{
    /// <summary>
    /// Represents the Account Configuartion class
    /// </summary>
    public class AccountConfiguration : EntityTypeConfiguration<Account>
    {
        /// <summary>
        /// Default Constructor
        /// Also sets the precision of the Opening Balance column.
        /// </summary>
        public AccountConfiguration()
        {
            this.Property(a => a.OpeningBalance)
                .HasPrecision(10, 2);
        }
    }
}