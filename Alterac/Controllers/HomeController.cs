using Microsoft.AspNetCore.Mvc;

namespace Nagrand.Controllers;
public class HomeController : Controller
{

    public IActionResult Index()
    {
        return View();
    }
}
