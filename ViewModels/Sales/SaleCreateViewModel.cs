using System.ComponentModel.DataAnnotations;

namespace Team_1_ITI.ViewModels.Sales
{
    public class SaleCreateViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Customer information is required.")]
        [StringLength(100, ErrorMessage = "Customer information cannot exceed 100 characters.")]
        public string CustomerInfo { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Sale must contain at least one item.")]
        public List<SaleItemViewModel> Items { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Items != null && Items.GroupBy(i => i.ProductID).Any(g => g.Count() > 1))
            {
                yield return new ValidationResult(
                    "Duplicate products are not allowed. Please update the quantity instead.",
                    new[] { nameof(Items) });
            }
        }
    }
}