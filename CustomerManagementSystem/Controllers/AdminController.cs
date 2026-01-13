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

        public AdminController(ICustomerRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Customer customer)
        {
            if (customer == null)
                return Json(new { success = false, message = "Invalid customer data" });

            if (!ModelState.IsValid)
                return Json(ModelState);

            try
            {
                await _repo.UpdateCustomerAsync(customer);
                return Json(new { success = true, message = "Customer updated successfully" });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating the customer"
                });

            }
        }



        public async Task<IActionResult> Delete([FromBody] int id)
        {
            await _repo.DeleteCustomerAsync(id);
            return Json(new { success = true });
        }
    }
}