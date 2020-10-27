using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using TGS.Apis.BackendOffice.DataService.DataInitializers;
using TGS.Apis.BackendOffice.DataService.EntityTypeConfigurations;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Core.DataServices.Base.Interfaces;

namespace TGS.Apis.BackendOffice.DataService.Repositories
{
    [ExcludeFromCodeCoverage]
    public class BackendOfficeRepository : DbContext, IBackendOfficeRepository
    {
        public BackendOfficeRepository(DbContextOptions<BackendOfficeRepository> options) : base(options)
        {
        }

        DbSet<User> IBackendOfficeRepository.Users { get; set; }
        DbSet<Role> IBackendOfficeRepository.Roles { get; set; }
        DbSet<Setting> IBackendOfficeRepository.Settings { get; set; }
        DbSet<Department> IBackendOfficeRepository.Departments { get; set; }
        DbSet<PaymentVendor> IBackendOfficeRepository.PaymentVendors { get; set; }
        DbSet<PaymentMethod> IBackendOfficeRepository.PaymentMethods { get; set; }
        DbSet<PaymentChannel> IBackendOfficeRepository.PaymentChannels { get; set; }
        DbSet<Payment> IBackendOfficeRepository.Payments { get; set; }
        DbSet<UserPaymentAccessControl> IBackendOfficeRepository.UserPaymentAccessControls { get; set; }

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

        Task IBaseRepository.BulkInsertAsync<T>(IList<T> entities, BulkConfig bulkConfig, Action<decimal> progress)
            where T : class
        {
            return this.BulkInsertAsync(entities, bulkConfig, progress);
        }

        Task IBaseRepository.BulkInsertOrUpdateAsync<T>(IList<T> entities, BulkConfig bulkConfig,
            Action<decimal> progress) where T : class
        {
            return this.BulkInsertOrUpdateAsync(entities, bulkConfig, progress);
        }

        EntityEntry<T> IBaseRepository.Entry<T>(T entity)
        {
            return base.Entry(entity);
        }

        DbSet<T> IBaseRepository.Set<T>()
        {
            return Set<T>();
        }

        Task IBaseRepository.SaveChangesAsync()
        {
            return SaveChangesAsync();
        }

        async Task IBaseRepository.ExecuteMigrationAndSeedingAsync(bool addSeeding)
        {
            var isMigrate = !Database.GetService<IRelationalDatabaseCreator>().Exists() ||
                            Database.GetPendingMigrations().Any();

            if (isMigrate) await Database.MigrateAsync();

            if (addSeeding)
            {
                IDataInitializer data = new DataInitializer(this);
                await data.SetInitialDataAsync();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new RoleMap());
            modelBuilder.ApplyConfiguration(new SettingMap());
            modelBuilder.ApplyConfiguration(new DepartmentMap());
            modelBuilder.ApplyConfiguration(new PaymentVendorMap());
            modelBuilder.ApplyConfiguration(new PaymentMethodMap());
            modelBuilder.ApplyConfiguration(new PaymentChannelMap());
            modelBuilder.ApplyConfiguration(new PaymentMap());

            modelBuilder.Entity<UserPaymentAccessControl>();
        }
    }
}