using Microsoft.AspNetCore.Mvc;

public class RadarController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
