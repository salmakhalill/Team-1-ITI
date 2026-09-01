using System.ComponentModel.DataAnnotations;

namespace Team_1_ITI.ViewModels.Suppliers
{
    public class SupplierEditViewModel
    {

        public int SupplierID { get; set; }

        [Required]
        public string SupplierName { get; set; }

        public string? ContactName { get; set; }

        public string? Phone {  get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Address { get; set; }
    }
}
