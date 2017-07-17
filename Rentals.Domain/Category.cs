using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rentals.Domain
{
    /// <summary>
    /// Class describing the Category entity
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Category()
        {
            Transactions = new HashSet<Transaction>();
        }

        /// <summary>
        /// Gets and Sets the Category ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets and Sets the Category Name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Gets and Sets the Category Type
        /// </summary>
        [Range(1,2)]
        public CategoryType Type { get; set; }

        /// <summary>
        /// Gets and Sets a collection of Transactions for this Category
        /// </summary>
        public virtual ICollection<Transaction> Transactions { get; private set; }
    }
}
