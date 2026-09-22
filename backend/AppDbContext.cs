using Example.Model;
using Microsoft.EntityFrameworkCore;

namespace Example;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    // public DbSet<Session> Sessions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
