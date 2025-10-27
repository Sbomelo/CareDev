using Microsoft.AspNetCore.Mvc;

namespace CareDev.Controllers
{
    public class NurseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

