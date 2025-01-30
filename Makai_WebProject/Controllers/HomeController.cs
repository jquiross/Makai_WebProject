using System.Diagnostics;
using Makai_WebProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace Makai_WebProject.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(UsuarioModel entidad)
        {
            return View();
        }
    }
}
