using Microsoft.AspNetCore.Mvc;

namespace Makai_APIProject.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
