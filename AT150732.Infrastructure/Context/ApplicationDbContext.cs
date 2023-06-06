namespace AT150732.Infrastructure.Context;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Filename=F:\\Doan\\AT15H\\AT150732.WebAPI\\AT15H.db");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Contact>().HasKey(e => e.Id);
        builder.Entity<Employee>().HasKey(e => e.Id);
        builder.Entity<Employee>().HasOne(e => e.Contact);
        base.OnModelCreating(builder);
    }
}
