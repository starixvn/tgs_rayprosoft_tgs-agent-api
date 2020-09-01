using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Base.Extensions
{
    public static class EntityFrameworkCoreExtensions
    {
        public static void SqlResource(this MigrationBuilder migrationBuilder, Assembly assembly, string resourceName)
        {
            if (assembly == null)
            {
                return;
            }

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                return;
            }

            using var reader = new StreamReader(stream);
            string sql = reader.ReadToEnd();
            migrationBuilder.Sql(sql);
        }

        public static void SqlResource(this MigrationBuilder migrationBuilder, string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                return;
            }

            using var reader = new StreamReader(stream);
            string sql = reader.ReadToEnd();
            migrationBuilder.Sql(sql);
        }
    }
}
