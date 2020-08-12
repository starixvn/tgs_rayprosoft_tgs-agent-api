using Microsoft.EntityFrameworkCore;
using TKS.Apis.LogApi.DataService.EntityTypeConfiguration;
using TKS.Apis.LogApi.DataService.Models;

namespace TKS.Apis.LogApi.DataService
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {
        }

        public DbSet<ApiLog> ApiLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.ApplyConfiguration(new ApiLogConfiguration());
        }
    }
}
