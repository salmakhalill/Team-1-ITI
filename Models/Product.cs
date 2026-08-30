using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace Team_1_ITI.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        public string SKU { get; set; }

        public string ProductName { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }

        public decimal UnitPrice { get; set; }

        public int StockQuantity { get; set; }

        public int LowStockThreshold { get; set; }

        public Category Category { get; set; }

        public List<PurchaseItem> PurchaseItems { get; set; }

        public List<SaleItem> SaleItems { get; set; }
    }
}
