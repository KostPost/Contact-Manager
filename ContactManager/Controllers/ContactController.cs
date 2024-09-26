using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Controllers;

public class ContactController : Controller
{
    public IActionResult UploadFile()
    {
        return View();
    }
}