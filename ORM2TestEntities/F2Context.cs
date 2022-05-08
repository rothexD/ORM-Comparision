using ORM.Core;

namespace ORM2TestEntiteis;

public class F2Context : DbContext
{
    public DbSet<customers> Customers { get; set; }
    public DbSet<books> Books { get; set; }
    public DbSet<chapters> Chapters { get; set; }
    public DbSet<bills> Bills { get; set; }
    public DbSet<articles> Articles { get; set; }
    public DbSet<knight> Knights { get; set; }
    public DbSet<weapon> Weapons { get; set; }
}