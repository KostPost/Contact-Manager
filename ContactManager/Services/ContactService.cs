using System.Text.RegularExpressions;
using ContactManager.Models;


namespace ContactManager.Services
{
    public interface IContactService
    {
        bool ValidateContact(Contact contact, out List<string> errors);
    }
    
    public class ContactService : IContactService
    {
        public bool ValidateContact(Contact contact, out List<string> errors)
        {
            errors = new List<string>();

            if (string.IsNullOrWhiteSpace(contact.Name))
            {
                errors.Add("Name is required.");
            }
            else if (contact.Name.Length > 100)
            {
                errors.Add("Name cannot be longer than 100 characters.");
            }

            if (contact.DateOfBirth == default)
            {
                errors.Add("Date of Birth is required.");
            }
            else if (contact.DateOfBirth > DateTime.Today)
            {
                errors.Add("Date of Birth cannot be in the future.");
            }

            var phonePattern = @"^\+?[0-9]{10,15}$";
            if (string.IsNullOrWhiteSpace(contact.Phone))
            {
                errors.Add("Phone number is required.");
            }
            else if (!Regex.IsMatch(contact.Phone, phonePattern))
            {
                errors.Add("Phone number is not valid. It should be between 10 to 15 digits.");
            }

            if (contact.Salary < 0)
            {
                errors.Add("Salary must be a positive number.");
            }

            return errors.Count == 0;
        }
    }
}
