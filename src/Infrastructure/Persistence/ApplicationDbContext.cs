using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Onion.Application.Common.Interfaces;
using Onion.Domain.Entities.Identity;

namespace Onion.Infrastructure.Persistence;

internal class ApplicationDbContext : DbContext, IDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }

    public DbSet<User> Users => Set<User>();

    public int Commit()
    {
        return SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
