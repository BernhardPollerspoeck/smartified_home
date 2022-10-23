using System.ComponentModel.DataAnnotations;

namespace smart.database;

public class User
{

    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = default!;
    [Required]
    public bool Locked { get; set; }

    public bool IsAdmin { get; set; }


    public string? PasswordHash { get; set; }
    [Required]
    public List<RefreshToken> RefreshTokens { get; set; } = default!;

}
