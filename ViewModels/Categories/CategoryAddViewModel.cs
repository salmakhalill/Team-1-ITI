using System.ComponentModel.DataAnnotations;

namespace Team_1_ITI.ViewModels.Categories
{
    public class CategoryAddViewModel
    {
        [Required]
        public string CategoryName { get; set; }

        public string? Description { get; set; }

    }
}
