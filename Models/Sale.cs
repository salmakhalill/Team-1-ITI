using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace Team_1_ITI.Models
{
    public class Sale
    {
        [Key]
        public int SaleID { get; set; }

        public DateTime SaleDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string? CustomerInfo { get; set; }

        public List<SaleItem> SaleItems { get; set; }
    }
}
