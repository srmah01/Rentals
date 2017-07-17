using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentals.Domain
{
    /// <summary>
    /// Class describing the Transaction entity
    /// </summary>
    public class Transaction : IValidatableObject
    {
        /// <summary>
        /// Gets and Sets the Payee ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets and Sets the Account ID for this Transaction
        /// This is a Foreign Key in to to Account table
        /// </summary>
        [Required(ErrorMessage = "The Account field is required.")]
        public int AccountId { get; set; }

        /// <summary>
        /// Gets and Sets the Account for this Transaction
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// Gets and Sets the Payee ID for this Transaction
        /// This is a Foreign Key in to to Payee table
        /// </summary>
        [Required(ErrorMessage = "The Payee field is required.")]
        public int PayeeId { get; set; }

        /// <summary>
        /// Gets and Sets the Payee for this Transaction
        /// </summary>
        public Payee Payee { get; set; }

        /// <summary>
        /// Gets and Sets the Category ID for this Transaction
        /// This is a Foreign Key in to to Category table
        /// </summary>
        [Required(ErrorMessage = "The Category field is required.")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets and Sets the Category for this Transaction
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// Gets and Sets the Date for this Transaction
        /// </summary>
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets and Sets the Amount for this Transaction
        /// </summary>
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString ="{0:c}")]
        public Decimal Amount { get; set; }

        /// <summary>
        /// Gets and Sets the Taxable flag for this Transaction
        /// </summary>
        [Required]
        public bool Taxable { get; set; }

        /// <summary>
        /// Gets and Sets the Balance amount for the Account after this Transaction
        /// </summary>
        [NotMapped]
        public Decimal Balance { get; set; }

        /// <summary>
        /// Gets and Sets the Reference for this Transaction
        /// </summary>
        [MaxLength(30)]
        public string Reference { get; set; }

        /// <summary>
        /// Gets and Sets the the Memo notes for this Transaction
        /// </summary>
        [MaxLength(200)]
        public string Memo { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Transaction()
        {
            Taxable = true;
        }

        /// <summary>
        /// Validates the Date and the Amount
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (Date <= DateTime.MinValue)
            {
                yield return new ValidationResult
                  ("Date must be specified.", new[] { "Date" });
            }

            if (Amount == 0.00m)
            {
                yield return new ValidationResult(
                  "Amount must be non-zero", new[] { "Amount" });
            }
        }
    }
}
