using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ContactManager.Services;

namespace ContactManager.Controllers;

public class ContactController : Controller
{
    private readonly ContactManagerDbContext _context;
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService, ContactManagerDbContext context)
    {
        _contactService = contactService;
        _context = context;
    }

    public IActionResult UploadFile()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,DateOfBirth,Married,Phone,Salary")] Contact contact)
    {
        if (_contactService.ValidateContact(contact, out List<string> errors))
        {
            if (ModelState.IsValid)
            {
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
            }
        }
        else
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        return View("UploadFile", contact);
    }


    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UploadCSV(IFormFile file)
{
    if (file != null && file.Length > 0)
    {
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            var lineNumber = 0;
            var hasErrors = false; // Флаг для отслеживания ошибок

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                var values = line.Split(',');

                // Пропускаем первую строку (заголовки)
                if (lineNumber == 0)
                {
                    lineNumber++;
                    continue;
                }

                try
                {
                    var contact = new Contact
                    {
                        Name = values[0],
                        DateOfBirth = DateTime.ParseExact(values[1], "yyyy-MM-dd", CultureInfo.InvariantCulture), // Убедитесь, что формат даты совпадает с CSV
                        Married = bool.Parse(values[2]),
                        Phone = values[3],
                        Salary = decimal.Parse(values[4])
                    };

                    // Проверяем данные контакта с помощью сервиса ContactService
                    if (_contactService.ValidateContact(contact, out List<string> errors))
                    {
                        // Если контакт прошел все проверки, добавляем его в базу данных
                        _context.Add(contact);
                    }
                    else
                    {
                        // Логируем ошибки проверки и продолжаем обработку
                        hasErrors = true;
                        foreach (var error in errors)
                        {
                            ModelState.AddModelError("", $"Error on line {lineNumber}: {error}");
                        }
                    }
                }
                catch (FormatException ex)
                {
                    // Логируем ошибки формата
                    hasErrors = true;
                    ModelState.AddModelError("", $"Error parsing data on line {lineNumber}: {ex.Message}");
                }

                lineNumber++;
            }

            // Сохраняем контакты в базу, если нет ошибок
            if (!hasErrors)
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contacts uploaded successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Some contacts could not be uploaded due to errors. Please check the error messages.";
            }

            return RedirectToAction(nameof(UploadFile));
        }
    }

    TempData["ErrorMessage"] = "No file uploaded.";
    return RedirectToAction(nameof(UploadFile));
}


    public async Task<IActionResult> Index()
    {
        return View(await _context.Contacts.ToListAsync());
    }
}