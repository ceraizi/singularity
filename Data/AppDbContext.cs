using Microsoft.EntityFrameworkCore;
using Singularity.Models;

namespace Singularity.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Book> Books { get; set; }
}