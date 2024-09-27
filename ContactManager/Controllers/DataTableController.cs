using ContactManager.Data;
using ContactManager.Models;
using ContactManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Controllers
{
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

            var contacts = _context.Contacts.AsQueryable();

            contacts = sortOrder switch
            {
                "name_desc" => contacts.OrderByDescending(c => c.Name),
                "DateOfBirth" => contacts.OrderBy(c => c.DateOfBirth),
                "date_desc" => contacts.OrderByDescending(c => c.DateOfBirth),
                "Married" => contacts.OrderBy(c => c.Married),
                "married_desc" => contacts.OrderByDescending(c => c.Married),
                "Phone" => contacts.OrderBy(c => c.Phone),
                "phone_desc" => contacts.OrderByDescending(c => c.Phone),
                "Salary" => contacts.OrderBy(c => c.Salary),
                "salary_desc" => contacts.OrderByDescending(c => c.Salary),
                _ => contacts.OrderBy(c => c.Name),
            };

            int pageSize = 11;
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
}
