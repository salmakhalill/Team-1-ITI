using System.ComponentModel.DataAnnotations;

namespace Team_1_ITI.ViewModels.Sales
{
    public class SaleCreateViewModel
    {
        [Required]
        [StringLength(100)]
        public string CustomerInfo { get; set; }

        [Required]
        public List<SaleItemViewModel> Items { get; set; } = new();
    }
}