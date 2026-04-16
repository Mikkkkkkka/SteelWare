using Microsoft.EntityFrameworkCore;
using SteelWare.Domain;
using SteelWare.Infrastructure.DataAccess.Configurations;

namespace SteelWare.Infrastructure.DataAccess;

public class SteelWareDbContext(DbContextOptions<SteelWareDbContext> options) : DbContext(options)
{
    public DbSet<SteelRoll> SteelRolls => Set<SteelRoll>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SteelRollConfiguration());
    }
}
