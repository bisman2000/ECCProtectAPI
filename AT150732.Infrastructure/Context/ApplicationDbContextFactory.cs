

using Microsoft.EntityFrameworkCore.Design;

namespace AT150732.Infrastructure.Context
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlite("Filename=F:\\Doan\\AT15H\\AT150732.WebAPI\\AT15H.db");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
