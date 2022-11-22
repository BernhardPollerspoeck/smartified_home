using System.ComponentModel.DataAnnotations;

namespace smart.database;

public class LogItem
{
    [Key]
    public int Id { get; set; }

    public string? ElementName { get; set; }
    public string? Type { get; set; }
    public string? HandlerName { get; set; }

    [Required]
    public string MetaInfo { get; set; } = default!;

    public DateTime Timestamp { get; set; }

}
