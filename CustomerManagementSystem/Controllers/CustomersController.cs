using CustomerManagementSystem.Infrastructure;
using CustomerManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CustomerManagementSystem.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _repo;

        public CustomersController(ICustomerRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Countries = await _repo.GetCountriesAsync();
            return View();
        }


        //public async Task<IActionResult> CustomerTablePartial(IFormCollection foFormCollection, int pageNumber = 1, int pageSize = 5)
        //{

        //    string? searchTerm = foFormCollection["searchValue"];

        //    if (ModelState.IsValid)
        //    {
        //        var data = await _repo.GetCustomersAsync(pageNumber, pageSize, searchTerm ?? "");
        //        ViewBag.Countries = await _repo.GetCountriesAsync();
        //        return PartialView("_customerList", data);
        //    }
        //    return View();

        //}

        public async Task<IActionResult> CustomerTablePartial(
        IFormCollection foFormCollection,
        int pageNumber = 1,
        int pageSize = 5,
        string sortColumn = "CreatedAt",
        string sortDirection = "DESC")
        {
            string? searchTerm = foFormCollection["searchValue"];

            var data = await _repo.GetCustomersAsync(
                pageNumber,
                pageSize,
                string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm,
                sortColumn,
                sortDirection
            );

            ViewBag.Countries = await _repo.GetCountriesAsync();
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortDirection = sortDirection;

            return PartialView("_customerList", data);
        }






        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Countries = await _repo.GetCountriesAsync();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            var (isSuccess, reason) = await _repo.AddCustomerAsync(customer);

            if (!isSuccess)
            {
                ViewBag.Countries = await _repo.GetCountriesAsync();
                TempData["SuccessMessage"] = reason;
                return View(customer);
            }

            TempData["SuccessMessage"] = reason;
            return RedirectToAction(nameof(Index));

        }
    }
}