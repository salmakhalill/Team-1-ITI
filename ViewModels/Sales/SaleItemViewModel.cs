using System.ComponentModel.DataAnnotations;

namespace Team_1_ITI.ViewModels.Sales
{
    public class SaleItemViewModel
    {
        [Required]
        public int ProductID { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}