using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Controllers;

public class ContactController : Controller
{
    public IActionResult UploadFile()
    {
        return View();
    }
    
    private readonly ContactManagerDbContext _context;

    public ContactController(ContactManagerDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,DateOfBirth,Married,Phone,Salary")] Contact contact)
    {
        if (ModelState.IsValid)
        {
            _context.Add(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        return View(contact);
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Contacts.ToListAsync());
    }
}
