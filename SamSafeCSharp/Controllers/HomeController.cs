using Microsoft.AspNetCore.Mvc;

namespace SafeCSharp
{
    [Route("safecsharp/[controller]")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}