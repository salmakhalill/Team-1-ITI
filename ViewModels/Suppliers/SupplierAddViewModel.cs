using System.ComponentModel.DataAnnotations;

namespace Team_1_ITI.ViewModels.Suppliers
{
    public class SupplierAddViewModel
    {
        [Required(ErrorMessage = "Supplier name is required.")]
        [StringLength(100, ErrorMessage = "Supplier name cannot exceed 100 characters.")]
        public string SupplierName { get; set; }

        [Required(ErrorMessage = "Contact name is required.")]
        [StringLength(100, ErrorMessage = "Contact name cannot exceed 100 characters.")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }
    }
}