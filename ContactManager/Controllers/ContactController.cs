using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ContactManager.Services;

namespace ContactManager.Controllers
{
    public class ContactController : Controller
    {
        private readonly ContactManagerDbContext _context;
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService, ContactManagerDbContext context)
        {
            _contactService = contactService;
            _context = context;
        }

        public IActionResult UploadFile() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,DateOfBirth,Married,Phone,Salary")] Contact contact)
        {
            var errors = new List<string>();

            if (!ModelState.IsValid || !_contactService.ValidateContact(contact, out errors))
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View("UploadFile", contact);
            }

            try
            {
                _context.Add(contact);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contact added successfully!";
                return RedirectToAction(nameof(UploadFile));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while adding the contact: " + ex.Message);
            }

            return View("UploadFile", contact);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCSV(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "No file uploaded.";
                return View("UploadFile");
            }

            var contacts = new List<Contact>();
            var errors = new List<string>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                string line;
                int lineNumber = 0;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;
                    if (lineNumber == 1) continue; // Skip header

                    var values = line.Split(',');
                    if (TryParseContact(values, lineNumber, out var contact, out var validationErrors))
                    {
                        contacts.Add(contact);
                    }
                    else
                    {
                        errors.AddRange(validationErrors.Select(error => $"Error on line {lineNumber}: {error}"));
                    }
                }
            }

            if (errors.Any())
            {
                TempData["ErrorMessage"] = "Some contacts could not be uploaded due to errors. Please check the error messages.";
                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            else
            {
                await _context.AddRangeAsync(contacts);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contacts uploaded successfully!";
            }

            return View("UploadFile");
        }

        private bool TryParseContact(string[] values, int lineNumber, out Contact contact, out List<string> validationErrors)
        {
            contact = null;
            validationErrors = new List<string>();

            try
            {
                contact = new Contact
                {
                    Name = values[0],
                    DateOfBirth = DateTime.ParseExact(values[1], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Married = bool.Parse(values[2]),
                    Phone = values[3],
                    Salary = decimal.Parse(values[4])
                };

                if (!_contactService.ValidateContact(contact, out var validationErrs))
                {
                    validationErrors.AddRange(validationErrs);
                    return false;
                }

                return true;
            }
            catch (FormatException ex)
            {
                validationErrors.Add($"Format error: {ex.Message}");
            }
            catch (IndexOutOfRangeException)
            {
                validationErrors.Add("Not enough data provided.");
            }
            catch (Exception ex)
            {
                validationErrors.Add($"Unexpected error: {ex.Message}");
            }

            return false;
        }

        public async Task<IActionResult> Index() => View(await _context.Contacts.ToListAsync());
    }
}
