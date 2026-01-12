using CustomerManagementSystem.Infrastructure;
using CustomerManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICustomerRepository _repo;

        public AdminController(ICustomerRepository repo) => _repo = repo;


        [HttpPost]
        public async Task<IActionResult> Update(Customer customer)
        {
            await _repo.UpdateCustomerAsync(customer);
            return Ok();
        }


        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteCustomerAsync(id);
            return RedirectToAction("Index", "Customers");
        }
    }
}