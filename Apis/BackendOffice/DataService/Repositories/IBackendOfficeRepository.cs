using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Core.DataServices.Base.Interfaces;

namespace TGS.Apis.BackendOffice.DataService.Repositories
{
    public interface IBackendOfficeRepository : IBaseRepository
    {
        DbSet<User> Users { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<Setting> Settings { get; set; }
        DbSet<Department> Departments { get; set; }
        DbSet<PaymentVendor> PaymentVendors { get; set; }
        DbSet<PaymentMethod> PaymentMethods { get; set; }
        DbSet<PaymentChannel> PaymentChannels { get; set; }
        DbSet<Payment> Payments { get; set; }
        DbSet<UserPaymentAccessControl> UserPaymentAccessControls { get; set; }
    }
}