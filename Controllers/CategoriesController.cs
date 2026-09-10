using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_1_ITI.Models;
using Team_1_ITI.Services;
using Team_1_ITI.ViewModels.Categories;

namespace Team_1_ITI.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly CategoryService service;

        public CategoriesController (CategoryService categoryService)
        {
            service = categoryService;
        }

        public IActionResult Index(int pageNumber =1)
        {
            int pageSize = 10;

            var categories = service.GetPaged(
                pageNumber,
                pageSize,
                out int totalPages
                );

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

        [AcceptVerbs("GET", "POST")]
        public IActionResult CheckCategoryName(string CategoryName, int CategoryID = 0)
        {
            bool isExists = service.IsNameExists(CategoryName, CategoryID);

            if (isExists)
            {
                return Json(false); 
            }

            return Json(true); 
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

                service.Add(category);

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

            var category = service.GetDetails(categoryid.Value);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        public IActionResult Edit(int? CategoryID)
        {

            if (CategoryID == null)
                return NotFound();

            var category = service.GetById(CategoryID.Value);

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
            var category = service.GetById(model.CategoryID);
            if (category == null)
                return NotFound();

            category.CategoryName = model.CategoryName;
            category.Description = model.Description;

            service.Update(category);

            return RedirectToAction("Index");
        }


        public IActionResult Delete(int ? CategoryID)
        {
            if (CategoryID == null)
                return NotFound();

            var category =service.GetById(CategoryID.Value);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int CategoryID)
        {
            var category = service.GetById(CategoryID);

            if (category == null)
                return NotFound();

            bool deleted = service.Delete(category);

            if (!deleted)
            {
                TempData["ErrorMessage"] =
                    "Cannot delete this category because it has products associated with it.";

                return View("Delete", category);
            }

            TempData["SuccessMessage"] = "Category deleted successfully.";

            return RedirectToAction("Index");
        }
    }
    
}
