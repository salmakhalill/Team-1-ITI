using Team_1_ITI.Models;

namespace Team_1_ITI.ViewModels.Categories
{
    public class CategoryIndexViewModel
    {
        public List<Category> Categories { get; set; } = new();

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}