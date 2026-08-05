namespace Ecommerce.Api.Core.Models;

public class Customer
{
    public int Id { get; set; }
    public string? IdentityUserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime CreatedDate { get; set; }
}
