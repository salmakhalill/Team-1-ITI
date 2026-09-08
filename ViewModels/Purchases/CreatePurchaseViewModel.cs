using System.ComponentModel.DataAnnotations;

namespace Team_1_ITI.ViewModels.Purchases
{
    public class CreatePurchaseViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Please select a supplier.")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [MinLength(1, ErrorMessage = "Purchase must contain at least one item.")]
        public List<PurchaseItemViewModel> Items { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Items != null && Items.GroupBy(i => i.ProductId).Any(g => g.Count() > 1))
            {
                yield return new ValidationResult(
                    "Duplicate products are not allowed. Please update the quantity instead.",
                    new[] { nameof(Items) });
            }
        }
    }
}