using System.ComponentModel.DataAnnotations;

namespace smart.database;

public class HomeElement
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public EElementType ElementType { get; set; } = default!;

    public bool ConnectionValidated { get; set; }

    public string? ConnectionInfo { get; set; } = default!;

    public string? SettingsData { get; set; } = default!;

    public string? StateData { get; set; }
    public DateTime? StateTimestamp { get; set; }

    public virtual ElementHandler ElementHandler { get; set; } = default!;

}
