namespace Team_1_ITI.ViewModels.Products
{
    public class ProductIndexViewModel
    {
        public List<ProductViewModel> Products { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public string CurrentSearch { get; set; } = "";
        public int? CurrentCategory { get; set; }
        public string CurrentStatus { get; set; } = "";
    }
}