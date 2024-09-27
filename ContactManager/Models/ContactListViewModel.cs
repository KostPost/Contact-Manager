namespace ContactManager.Models;

public class ContactListViewModel
{
    public IEnumerable<Contact> Contacts { get; set; }
    public int PageNumber { get; set; }
    public int TotalContacts { get; set; }
    public int PageSize { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalContacts / PageSize);
}