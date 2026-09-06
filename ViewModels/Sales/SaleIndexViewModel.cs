using Team_1_ITI.Models;

namespace Team_1_ITI.ViewModels.Sales
{
    public class SaleIndexViewModel
    {
        public List<Sale> Sales { get; set; } = new();

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}