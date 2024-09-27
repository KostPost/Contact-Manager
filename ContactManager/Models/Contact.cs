
using System.ComponentModel.DataAnnotations;

namespace ContactManager.Models;

public class Contact
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    public bool Married { get; set; }

    [Required]
    [Phone]
    public string Phone { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Salary { get; set; }
}