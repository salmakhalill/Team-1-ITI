using System.ComponentModel.DataAnnotations;

namespace Team_1_ITI.ViewModels.Purchases
{
    public class PurchaseItemViewModel
    {
        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Min Qty is 1.")]
        public int Quantity { get; set; }

        [Range(0.01, 99999999.99, ErrorMessage = "Cost must be > 0.")]
        public decimal UnitCost { get; set; }
    }
}