using Microsoft.AspNetCore.Mvc;

namespace AppointmentApi.Controllers
{
    public class HomeController : ControllerBase
    {
        public IActionResult Index() => new RedirectResult("~/swagger");
    }
}