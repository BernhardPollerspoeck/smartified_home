using smart.contract;
using System.ComponentModel.DataAnnotations;

namespace smart.database;

public class ElementHandler
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public EHandlerType HandlerType { get; set; } = default!;


    public string? SettingsData { get; set; } = default!;

    public bool Connected { get; set; }

    public virtual ICollection<HomeElement> HomeElements { get; set; } = default!;

}
