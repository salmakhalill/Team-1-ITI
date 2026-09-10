using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace Team_1_ITI.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseID { get; set; }

        [ForeignKey("Supplier")]
        public int SupplierID { get; set; }

        public DateTime PurchaseDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public Supplier Supplier { get; set; }

        public List<PurchaseItem> PurchaseItems { get; set; }
    }
}
