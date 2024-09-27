using ContactManager.Data;
using ContactManager.Models;
using ContactManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Controllers;

public class DataTableController : Controller
{
    private readonly ContactManagerDbContext _context;
    private readonly IContactService _contactService;

    public DataTableController(IContactService contactService, ContactManagerDbContext context)
    {
        _contactService = contactService;
        _context = context;
    }
    
    public async Task<IActionResult> DataTable(string sortOrder, int pageNumber = 1)
    {
        ViewBag.NameSortParam = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewBag.DateSortParam = sortOrder == "DateOfBirth" ? "date_desc" : "DateOfBirth";
        ViewBag.MarriedSortParam = sortOrder == "Married" ? "married_desc" : "Married";
        ViewBag.PhoneSortParam = sortOrder == "Phone" ? "phone_desc" : "Phone";
        ViewBag.SalarySortParam = sortOrder == "Salary" ? "salary_desc" : "Salary";

        var contacts = from c in _context.Contacts select c;

        switch (sortOrder)
        {
            case "name_desc":
                contacts = contacts.OrderByDescending(c => c.Name);
                break;
            case "DateOfBirth":
                contacts = contacts.OrderBy(c => c.DateOfBirth);
                break;
            case "date_desc":
                contacts = contacts.OrderByDescending(c => c.DateOfBirth);
                break;
            case "Married":
                contacts = contacts.OrderBy(c => c.Married);
                break;
            case "married_desc":
                contacts = contacts.OrderByDescending(c => c.Married);
                break;
            case "Phone":
                contacts = contacts.OrderBy(c => c.Phone);
                break;
            case "phone_desc":
                contacts = contacts.OrderByDescending(c => c.Phone);
                break;
            case "Salary":
                contacts = contacts.OrderBy(c => c.Salary);
                break;
            case "salary_desc":
                contacts = contacts.OrderByDescending(c => c.Salary);
                break;
            default:
                contacts = contacts.OrderBy(c => c.Name);
                break;
        }
        
        

        int pageSize = 11; // Number of contacts per page
        var totalContacts = await contacts.CountAsync();
        var contactsToShow = await contacts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        var viewModel = new ContactListViewModel
        {
            Contacts = contactsToShow,
            PageNumber = pageNumber,
            TotalContacts = totalContacts,
            PageSize = pageSize
        };

        return View(viewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact != null)
        {
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Contact deleted successfully.";
        }
        else
        {
            TempData["SuccessMessage"] = "Contact not found.";
        }

        return RedirectToAction("DataTable");
    }
}
