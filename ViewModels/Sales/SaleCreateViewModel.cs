using System.ComponentModel.DataAnnotations;

namespace Team_1_ITI.ViewModels.Sales
{
    public class SaleCreateViewModel
    {
        public string? CustomerInfo { get; set; }

        [Required]
        public List<SaleItemViewModel> Items { get; set; } = new();
    }
}