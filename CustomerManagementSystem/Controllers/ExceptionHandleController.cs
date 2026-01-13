using Microsoft.AspNetCore.Mvc;

namespace CustomerManagementSystem.Controllers
{
    public class ExceptionHandleController : Controller
    {
        public IActionResult Error()
        {
            return View();
        }
    }
}
