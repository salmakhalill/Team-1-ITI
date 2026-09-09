using Microsoft.AspNetCore.Mvc;
using Team_1_ITI.Models;
using Team_1_ITI.Services;
using Team_1_ITI.ViewModels.Suppliers;

namespace Team_1_ITI.Controllers
{
    public class SuppliersController : Controller
    {

        private readonly SupplierService service;

        public SuppliersController (SupplierService supplierService)
        {
            service = supplierService;
        }
        public IActionResult Index(int pageNumber = 1)
        {
            int pageSize = 10;

            var suppliers = service.GetPaged(
                pageNumber,
                pageSize,
                out int totalPages
                );

            SupplierIndexViewModel model = new()
            {
                Suppliers = suppliers,
                CurrentPage = pageNumber,
                TotalPages = totalPages
            };

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(SupplierAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                var supplier = new Supplier
                {
                    SupplierName = model.SupplierName,
                    ContactName = model.ContactName,
                    Phone = model.Phone,
                    Email = model.Email,
                    Address = model.Address,
                };

                service.Add(supplier);

                return RedirectToAction("Index");

            }

            return View(model);

        }

        public IActionResult Details(int? SupplierID)
        {
            if (SupplierID == null)
                return NotFound();
            var supplier = service.GetDetails(SupplierID.Value);

            if (supplier == null)
                return NotFound();
            return View(supplier);
        }

        public IActionResult Edit(int? SupplierID)
        {
            if (SupplierID == null)
                return NotFound();

            var supplier =service.GetById(SupplierID.Value);

            if (supplier == null)
                return NotFound();

            var model = new SupplierEditViewModel
            {
                SupplierID = supplier.SupplierID,
                SupplierName = supplier.SupplierName,
                ContactName = supplier.ContactName,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Address = supplier.Address,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SupplierEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var supplier =service.GetById(model.SupplierID);

            if (supplier == null)
                return NotFound();

            supplier.SupplierName = model.SupplierName;
            supplier.ContactName = model.ContactName;
            supplier.Phone = model.Phone;
            supplier.Email = model.Email;
            supplier.Address = model.Address;

            service.Update(supplier);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int? SupplierID)
        {
            if (SupplierID == null)
                return NotFound();

            var supplier = service.GetById(SupplierID.Value);

            if (supplier == null)
                return NotFound();

            return View(supplier);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int SupplierID)
        {
            var supplier = service.GetById(SupplierID);

            if (supplier == null)
                return NotFound();

            bool deleted = service.Delete(supplier);

            if (!deleted)
            {
                TempData["ErrorMessage"] =
                    "Cannot delete this supplier because it has purchases associated with it.";

                return View("Delete", supplier);
            }

            TempData["SuccessMessage"] = "Supplier deleted successfully.";

            return RedirectToAction("Index");
        }
    }

}
