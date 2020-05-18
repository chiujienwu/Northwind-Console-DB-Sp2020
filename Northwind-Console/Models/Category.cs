using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NorthwindConsole.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Category Name is Required")]
        [MaxLength(15)]
        public string CategoryName { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }

        public virtual List<Product> Products { get; set; }
    }
}
