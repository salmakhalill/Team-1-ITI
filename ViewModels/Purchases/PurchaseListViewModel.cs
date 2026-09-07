namespace Team_1_ITI.ViewModels.Purchases
{
    public class PurchaseListViewModel
    {
        public int PurchaseID { get; set; }
        public string SupplierName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}