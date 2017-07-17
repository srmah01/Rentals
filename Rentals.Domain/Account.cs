using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentals.Domain
{
    /// <summary>
    /// Class describing the Account entity
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Account()
        {
           Transactions = new HashSet<Transaction>();
        }

        /// <summary>
        /// Gets and Sets the Account ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets and Sets the Account Name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public String Name { get; set; }

        /// <summary>
        /// Gets and Sets the Account Opening Balance
        /// </summary>
        [DisplayName("Opening Balance")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Decimal OpeningBalance { get; set; }

        /// <summary>
        /// Gets and Sets the Account Balance
        /// This is calculated from sum of the Transactions on this account
        /// </summary>
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Decimal Balance { get; set; }

        /// <summary>
        /// Gets and Sets a collection of Transactions for this Account
        /// </summary>
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
