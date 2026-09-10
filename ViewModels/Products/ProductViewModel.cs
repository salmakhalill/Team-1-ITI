using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Team_1_ITI.ViewModels.Products
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage = "SKU is required.")]
        [StringLength(50)]
        [Remote(action: "CheckSKU", controller: "Product", AdditionalFields = nameof(ProductID), ErrorMessage = "This SKU is already registered for another product.")]
        [Display(Name = "SKU / Code")]
        public string SKU { get; set; } = null!;

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        [Display(Name = "Initial Stock")]
        public int StockQuantity { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Low Stock Alert Level")]
        public int LowStockThreshold { get; set; }
    }
}