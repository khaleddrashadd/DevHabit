using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Database;

//ApplicationDbContext is the same name of class
public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(Schemas.Application);
    }
}