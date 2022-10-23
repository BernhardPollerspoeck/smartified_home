using System.ComponentModel.DataAnnotations;

namespace smart.database;

public class ImageInfo
{
    [Key]
    public EElementType ElementType { get; set; } = default!;

    [Required]
    public string Name { get; set; } = default!;
    [Required]
    public string Tag { get; set; } = default!;

}