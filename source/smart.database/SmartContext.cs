using Microsoft.EntityFrameworkCore;
using smart.contract;
using System.ComponentModel.DataAnnotations;

namespace smart.database;
public class SmartContext : DbContext
{

    public SmartContext()
    {
    }
    public SmartContext(DbContextOptions<SmartContext> contextOptions) : base(contextOptions)
    {
    }

    public DbSet<User> Users { get; set; } = default!;

    public DbSet<ElementHandler> ElementHandlers { get; set; } = default!;
    public DbSet<HomeElement> Elements { get; set; } = default!;

    public DbSet<LogItem> Log { get; set; } = default!;

    public DbSet<PlanDefinition> Plans { get; set; } = default!;
    public DbSet<Image> Images { get; set; } = default!;
    public DbSet<PlanElement> PlanElements { get; set; } = default!;
}

public class PlanDefinition
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = default!;

    [Required]
    public virtual Image Image { get; set; } = default!;
    public virtual ICollection<PlanElement> Elements { get; set; } = default!;

}

public class Image
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = default!;

    public EImageType Type { get; set; } = default!;

    [Required]
    public byte[] Data { get; set; } = default!;

}

public class PlanElement
{
    [Key]
    public int Id { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    public int Size { get; set; }

    [Required]
    public virtual HomeElement Element { get; set; } = default!;

    [Required]
    public virtual PlanDefinition Plan { get; set; } = default!;

    [Required]
    public virtual Image Image { get; set; } = default!;

}