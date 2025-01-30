using Microsoft.AspNetCore.Mvc;

namespace Makai_WebProject.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult IniciarSesion()
        {
            return View();
        }

        public IActionResult Principal()
        {
            return View();
        }
    }
}
