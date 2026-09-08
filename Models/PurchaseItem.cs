using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace Team_1_ITI.Models
{
    public class PurchaseItem
    {
        [Key]
        public int PurchaseItemID { get; set; }

        [ForeignKey("Purchase")]
        public int PurchaseID { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public Purchase Purchase { get; set; }

        public Product Product { get; set; }
    }
}
