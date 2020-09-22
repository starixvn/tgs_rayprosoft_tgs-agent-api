using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.BackendOffice.DataService.DataInitializers;

namespace TGS.Apis.BackendOffice.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void SeedData(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dataInitializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();
            dataInitializer.SetInitialData();
        }
    }
}
