using ContactManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Data;

public class ContactManagerDbContext : DbContext
{
    public ContactManagerDbContext(DbContextOptions<ContactManagerDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Contact> Contacts { get; set; }
}