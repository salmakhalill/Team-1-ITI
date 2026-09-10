using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Team_1_ITI.ViewModels.Categories
{
    public class CategoryAddViewModel
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]

        [Remote(action: "CheckCategoryName", controller: "Categories", ErrorMessage = "This category name already exists.")]
        public string CategoryName { get; set; }

        [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters.")]
        public string? Description { get; set; }
    }
}