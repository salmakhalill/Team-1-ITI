using Microsoft.EntityFrameworkCore;
using Team_1_ITI.Data;
using Team_1_ITI.Models;

namespace Team_1_ITI.Services
{
    public class SupplierService
    {
        private readonly InventoryManagementDbContext db;

        public SupplierService(InventoryManagementDbContext db)
        {
            this.db = db;
        }


        public List<Supplier> GetAll()
        {
            return db.Suppliers.ToList();
        }


        public List<Supplier> GetPaged(
            int pageNumber,
            int pageSize,
            out int totalPages
            )
        {
            int totalSuppliers = db.Suppliers.Count();

            totalPages=(int)Math.Ceiling((double)totalSuppliers/pageSize);

            if (pageNumber < 1)
                pageNumber = 1;


            if (pageNumber > totalPages && totalPages > 0)
                pageNumber = totalPages;

            return db.Suppliers
                .OrderBy(s => s.SupplierID)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();


        }



        public Supplier? GetById(int id) 
        {
            return db.Suppliers
                .FirstOrDefault(s=>s.SupplierID == id);
        }


        public Supplier? GetDetails(int id)
        {
            return db.Suppliers
                .Include(s => s.Purchases)
                    .ThenInclude(p => p.PurchaseItems)
                        .ThenInclude(pi => pi.Product)
                .FirstOrDefault(s => s.SupplierID == id);
        }

        public bool IsNameExists(string supplierName, int excludeId = 0)
        {
            return db.Suppliers.Any(s => s.SupplierName == supplierName && s.SupplierID != excludeId);
        }

        public bool IsEmailExists(string email, int excludeId = 0)
        {
            return db.Suppliers.Any(s => s.Email == email && s.SupplierID != excludeId);
        }

        public void Add(Supplier supplier) 
        {
            db.Suppliers .Add(supplier);
            db.SaveChanges();
        }


        public void Update(Supplier supplier) 
        {

            db.SaveChanges();
        }


        public bool Delete(Supplier supplier)
        {
            bool hasPurchases = db.Purchases
                .Any(p => p.SupplierID == supplier.SupplierID);

            if (hasPurchases)
            {
                return false;
            }

            db.Suppliers.Remove(supplier);
            db.SaveChanges();

            return true;
        }
    }
}
