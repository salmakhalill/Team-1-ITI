using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace Team_1_ITI.Models
{
    [Index(nameof(CategoryName), IsUnique = true)]
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        public List<Product> Products { get; set; }
    }
}
