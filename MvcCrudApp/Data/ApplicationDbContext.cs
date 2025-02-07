using Microsoft.EntityFrameworkCore;
using MvcCrudApp.Models;

namespace MvcCrudApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
    }
}
