
using Microsoft.AspNetCore.Mvc;

namespace Projeto_Bibliotec_MVC_tarde.Controllers;

public class LoginController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}


