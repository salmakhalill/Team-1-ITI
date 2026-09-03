using System.ComponentModel.DataAnnotations;

namespace Team_1_ITI.ViewModels.Purchases
{
    public class CreatePurchaseViewModel
    {
        [Required(ErrorMessage = "Please select a supplier.")]
        public int SupplierId { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [MinLength(1, ErrorMessage = "Purchase must contain at least one item.")]
        public List<PurchaseItemViewModel> Items { get; set; }
            = new List<PurchaseItemViewModel>();
    }
}