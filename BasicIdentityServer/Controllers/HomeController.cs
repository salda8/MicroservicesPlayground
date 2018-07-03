using Microsoft.AspNetCore.Mvc;

namespace BasicIdentityServer.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}