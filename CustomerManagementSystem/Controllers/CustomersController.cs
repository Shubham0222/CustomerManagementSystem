using CustomerManagementSystem.Infrastructure;
using CustomerManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class CustomersController : Controller
{
    private readonly ICustomerRepository _repo;

    public CustomersController(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index(int page = 1, string search = "")
    {
        int pageSize = 10;

        var model = await _repo.GetCustomersAsync(page, pageSize, search);

        model.Search = search;

        return View(model);
    }


    [HttpGet]
    public IActionResult Create()
    {
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
            TempData["SuccessMessage"] = reason;
            return View(customer);
        }

        TempData["SuccessMessage"] = reason;
        return RedirectToAction(nameof(Index));

    }
}
