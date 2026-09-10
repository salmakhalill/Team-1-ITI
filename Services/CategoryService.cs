using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_1_ITI.Data;
using Team_1_ITI.Models;
using Team_1_ITI.Services.AI;

namespace Team_1_ITI.Services
{
    public class CategoryService
    {
        private readonly InventoryManagementDbContext db;

        public CategoryService(InventoryManagementDbContext db)
        {
            this.db = db;
        }


        public List<Category> GetAll()
        {
            return db.Categories.ToList();
        }


        public List<Category>GetPaged(
            int pageNumber,
            int pageSize,
            out int totalPages
            )
        {
            int totalCategories = db.Categories.Count();

            totalPages = (int)Math.Ceiling((Double)totalCategories / pageSize);

            if (pageNumber < 1)
                pageNumber = 1;

            if (pageNumber > totalPages && totalPages > 0)
                pageNumber = totalPages;

            return db.Categories
                .OrderBy(C => C.CategoryID)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public Category? GetById(int id)
        {
            return db.Categories
                .FirstOrDefault(c => c.CategoryID == id);
        }

        public bool IsNameExists(string categoryName, int categoryId = 0)
        {
            return db.Categories.Any(c => c.CategoryName == categoryName && c.CategoryID != categoryId);
        }

        public void Add(Category category)
        {
            db.Categories.Add(category);
            db.SaveChanges();
        }


        public Category? GetDetails(int id)
        {
            return db.Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.CategoryID == id);
        }


        public void Update(Category category)
        {
            db.SaveChanges();
        }


        public bool Delete(Category category)
        {
            bool hasProducts = db.Products
                .Any(p => p.CategoryID == category.CategoryID);

            if (hasProducts)
            {
                return false;
            }

            db.Categories.Remove(category);
            db.SaveChanges();

            return true;
        }
    }
}
