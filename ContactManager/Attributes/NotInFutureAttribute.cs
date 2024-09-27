using System.ComponentModel.DataAnnotations;

namespace ContactManager.Models;

public class NotInFutureAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime date)
        {
            if (date > DateTime.Today)
            {
                return new ValidationResult("Date of birth cannot be in the future.");
            }
        }

        return ValidationResult.Success;
    }
}