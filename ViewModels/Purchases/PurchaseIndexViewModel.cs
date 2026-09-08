namespace Team_1_ITI.ViewModels.Purchases
{
    public class PurchaseIndexViewModel
    {
        public List<PurchaseListViewModel> Purchases { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}