using System.ComponentModel.DataAnnotations;

namespace smart.database;

public class LogItem
{
    public const string TYPE_COMMAND = "Command";


    [Key]
    public int Id { get; set; }

    [Required]
    public string Type { get; set; } = default!;
    [Required]
    public string ElementName { get; set; } = default!;
    [Required]
    public string ElementType { get; set; } = default!;

    [Required]
    public string HandlerName { get; set; } = default!;
    [Required]
    public string MetaInfo { get; set; } = default!;
    public bool Success { get; set; }

}
