using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace Team_1_ITI.Models
{
    [Index(nameof(SupplierName), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class Supplier
    {
        [Key]
        public int SupplierID { get; set; }

        [Required]
        [MaxLength(100)]
        public string SupplierName { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ContactName { get; set; }

        [Required]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Address { get; set; }

        public List<Purchase> Purchases { get; set; } 
    }
}
