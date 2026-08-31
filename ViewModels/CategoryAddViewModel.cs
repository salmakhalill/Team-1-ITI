using System.ComponentModel.DataAnnotations;

namespace Team_1_ITI.ViewModels
{
    public class CategoryAddViewModel
    {
        [Required]
        public string CategoryName { get; set; }

        public string? Description { get; set; }

    }
}
