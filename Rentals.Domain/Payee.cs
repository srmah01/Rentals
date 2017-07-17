using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rentals.Domain
{
    /// <summary>
    /// Class describing the Payee entity
    /// </summary>
    public class Payee
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Payee()
        {
            Transactions = new HashSet<Transaction>();
        }

        /// <summary>
        /// Gets and Sets the Payee ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets and Sets the Payee Name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Gets and Sets the Default Category for this Payee
        /// This is a Foreign Key in to to Category table
        /// </summary>
        [Required(ErrorMessage = "Default Category field is required.")]
        public int DefaultCategoryId { get; set; }

        /// <summary>
        /// Gets and Sets the Default Category for this Payee
        /// </summary>
        [Display(Name = "Default Category")]
        public Category DefaultCategory { get; set; }

        /// <summary>
        /// Gets and Sets the Memo notes for this Payee
        /// </summary>
        [MaxLength(200)]
        public string Memo { get; set; }

        /// <summary>
        /// Gets and Sets a collection of Transactions for this Payee
        /// </summary>
        public virtual ICollection<Transaction> Transactions { get; private set; }
    }
}
