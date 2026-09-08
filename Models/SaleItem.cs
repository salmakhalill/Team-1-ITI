using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace Team_1_ITI.Models
{
    public class SaleItem
    {
        [Key]
        public int SaleItemID { get; set; }

        [ForeignKey("Sale")]
        public int SaleID { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public Sale Sale { get; set; }

        public Product Product { get; set; }
    }
}
