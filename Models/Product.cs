using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace Team_1_ITI.Models
{
    [Index(nameof(SKU), IsUnique = true)]
    [Index(nameof(ProductName), IsUnique = true)]
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        [MaxLength(50)]
        public string SKU { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int StockQuantity { get; set; }

        public int LowStockThreshold { get; set; }

        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public List<PurchaseItem> PurchaseItems { get; set; }

        [ValidateNever]
        public List<SaleItem> SaleItems { get; set; }
    }
}
