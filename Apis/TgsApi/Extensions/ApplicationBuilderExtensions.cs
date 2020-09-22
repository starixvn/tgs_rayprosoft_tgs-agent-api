using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.DataServices.DataInitializers;

namespace TGS.Apis.TgsApi.Extensions
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