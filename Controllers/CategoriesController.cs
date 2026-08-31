using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_1_ITI.Models;
using Team_1_ITI.Services;
using Team_1_ITI.ViewModels;

namespace Team_1_ITI.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly InventoryManagementDbContext db;

        public CategoriesController (InventoryManagementDbContext context)
        {
            db= context;
        }

        public IActionResult Index(int pageNumber =1)
        {
            int pageSize = 10;

            int totalCategories = db.Categories.Count();

            int totalPages = (int)Math.Ceiling((Double)totalCategories/pageSize);

            if (pageNumber < 1)
                pageSize = 1;

            if (pageNumber > totalPages && totalPages>0)
                pageNumber = totalPages;

            List<Category> categories = db.Categories
                .OrderBy(C => C.CategoryID)
                .Skip((pageNumber -1)*pageSize) 
                .Take(pageSize)
                .ToList();

            CategoryIndexViewModel model = new()
            {
                Categories= categories,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
            };


            return View(model);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    CategoryName = model.CategoryName,
                    Description = model.Description
                };

                db.Categories.Add(category);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }


        public IActionResult Details(int? categoryid)
        {
            if (categoryid == null)
            {
                return NotFound();
            }

            var category = db.Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.CategoryID == categoryid);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        public IActionResult Edit(int? CategoryID)
        {
            var category = db.Categories
                .FirstOrDefault(c => c.CategoryID == CategoryID);
            if (category == null)
                return NotFound();

            var model = new CategoryEditViewModel
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(CategoryEditViewModel model)
        {
            var category = db.Categories
                .FirstOrDefault(c => c.CategoryID == model.CategoryID);
            if (category == null)
                return NotFound();

            category.CategoryName = model.CategoryName;
            category.Description = model.Description;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
    
}
