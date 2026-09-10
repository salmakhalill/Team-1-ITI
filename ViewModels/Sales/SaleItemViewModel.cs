using System.ComponentModel.DataAnnotations;

namespace Team_1_ITI.ViewModels.Sales
{
    public class SaleItemViewModel
    {
        [Required(ErrorMessage = "Please select a product.")]
        public int ProductID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}