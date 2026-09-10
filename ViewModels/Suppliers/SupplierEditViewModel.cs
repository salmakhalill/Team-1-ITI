using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Team_1_ITI.ViewModels.Suppliers
{
    public class SupplierEditViewModel
    {
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        [StringLength(100, ErrorMessage = "Supplier name cannot exceed 100 characters.")]
        [Remote(action: "CheckSupplierName", controller: "Suppliers", AdditionalFields = nameof(SupplierID), ErrorMessage = "This supplier name already exists.")]
        public string SupplierName { get; set; }

        [Required(ErrorMessage = "Contact name is required.")]
        [StringLength(100, ErrorMessage = "Contact name cannot exceed 100 characters.")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Remote(action: "CheckEmail", controller: "Suppliers", AdditionalFields = nameof(SupplierID), ErrorMessage = "This email is already registered for another supplier.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }
    }
}