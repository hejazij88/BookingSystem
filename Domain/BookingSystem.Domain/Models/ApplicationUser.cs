using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Domain.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public IList<string> Roles { get; set; } = new List<string>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();


}