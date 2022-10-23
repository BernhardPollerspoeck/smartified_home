using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace smart.database;

[Owned]
public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Token { get; set; } = default!;

    [Required]
    public DateTime Expires { get; set; }

    [Required]
    public DateTime Created { get; set; }

    [Required]
    public string CreatedByIp { get; set; } = default!;

    public DateTime? Revoked { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? ReasonRevoked { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked => Revoked != null;
    public bool IsActive => !IsRevoked && !IsExpired;


    public static RefreshToken Invalid => new() { Revoked = DateTime.UtcNow - TimeSpan.FromDays(365) };

}