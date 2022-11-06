using Microsoft.EntityFrameworkCore;

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
}
