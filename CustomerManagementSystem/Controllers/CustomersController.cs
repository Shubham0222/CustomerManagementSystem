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

    public async Task<IActionResult> Index()
    {
        var data = await _repo.GetCustomersAsync(1, 10, "");
        return View(data);
    }

    // BOTH USER + ADMIN CAN CREATE
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

        await _repo.AddCustomerAsync(customer);
        return RedirectToAction("Index");
    }
}
