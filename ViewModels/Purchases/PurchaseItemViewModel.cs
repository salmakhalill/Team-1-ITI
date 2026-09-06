using System.ComponentModel.DataAnnotations;

namespace Team_1_ITI.ViewModels.Purchases
{
    public class PurchaseItemViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999999999",
            ErrorMessage = "Unit cost must be greater than 0.")]
        public decimal UnitCost { get; set; }
    }
}