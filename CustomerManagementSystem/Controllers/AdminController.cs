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
        public async Task<IActionResult> Update([FromBody] Customer customer)
        {
            await _repo.UpdateCustomerAsync(customer);
            return Json(new { success = true });
        }



        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteCustomerAsync(id);
            return Json(new { success = true });
        }
    }
}