using Microsoft.EntityFrameworkCore;
using CivicConnect.Models;

namespace CivicConnect.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ServiceRequest> ServiceRequests { get; set; }
    }
}
