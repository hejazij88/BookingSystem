namespace BookingSystem.Domain.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Revoked { get; set; }
    public bool IsRevoked => Revoked != null;
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }

    public bool IsActive => Revoked == null && !IsExpired;

    // relation
    public string ApplicationUserId { get; set; } = null!;
    public ApplicationUser ApplicationUser { get; set; } = null!;
}