using Microsoft.EntityFrameworkCore;
using TGS.Apis.LogApi.DataService.EntityTypeConfiguration;
using TGS.Apis.LogApi.DataService.Models;

namespace TGS.Apis.LogApi.DataService
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {
        }

        public DbSet<ApiLog> ApiLogs { get; set; }

        public DbSet<EventLog> EventLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.ApplyConfiguration(new ApiLogConfiguration());

            modelBuilder.ApplyConfiguration(new EventLogConfiguration());
        }
    }
}
