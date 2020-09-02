using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TGS.Apis.BackendOffice.DataService.EntityTypeConfigurations;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Core.DataServices.Base.Interfaces;

namespace TGS.Apis.BackendOffice.DataService
{
    public class BackendOfficeRepository : DbContext, IBackendOfficeRepository
    {
        public BackendOfficeRepository(DbContextOptions<BackendOfficeRepository> options) : base(options)
        {
        }

        DbSet<User> IBackendOfficeRepository.Users { get; set; }
        DbSet<Role> IBackendOfficeRepository.Roles { get; set; }
        DbSet<Setting> IBackendOfficeRepository.Settings { get; set; }

        Task<IDbContextTransaction> IBaseRepository.BeginTransactionAsync(IsolationLevel level)
        {
            return Database.BeginTransactionAsync(level);
        }

        Task<bool> IBaseRepository.CanConnectionAsync()
        {
            return Database.CanConnectAsync();
        }

        IDbConnection IBaseRepository.GetDbConnection()
        {
            return Database.GetDbConnection();
        }

        void IBaseRepository.SetTimeoutToSeconds(int seconds)
        {
            Database.SetCommandTimeout(seconds);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new RoleMap());
            modelBuilder.ApplyConfiguration(new SettingMap());
        }

        void IBackendOfficeRepository.BulkInsert<T>(IList<T> entities, BulkConfig bulkConfig, Action<decimal> progress) where T : class
        {
            this.BulkInsertOrUpdate(entities, bulkConfig, progress);
        }

        Task IBaseRepository.SaveChangesAsync()
        {
            return SaveChangesAsync();
        }
    }
}
