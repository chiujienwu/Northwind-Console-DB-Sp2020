using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NorthwindConsole.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Product Name is Required")]
        [MaxLength(40,ErrorMessage = "Product Name must be 40 characters or less.")]
        public string ProductName { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        // public decimal? WholesalePrice { get; set; }
        public Int16? UnitsInStock { get; set; }
        public Int16? UnitsOnOrder { get; set; }
        public Int16? ReorderLevel { get; set; }
        [DefaultValue(0)]
        public bool Discontinued { get; set; }

        public int CategoryID { get; set; }
        public int SupplierID { get; set; }

        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
