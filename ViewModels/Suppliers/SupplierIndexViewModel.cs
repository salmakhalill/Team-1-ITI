using Team_1_ITI.Models;

namespace Team_1_ITI.ViewModels.Suppliers
{
    public class SupplierIndexViewModel
    {
        public List<Supplier> Suppliers { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}
