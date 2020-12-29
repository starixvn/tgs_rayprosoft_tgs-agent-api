using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        private const string MarketDepartment = "市場";
        private const string ProductDepartment = "產品";

        private static readonly Dictionary<string, Guid> Departments = new Dictionary<string, Guid>
        {
            {MarketDepartment, new Guid("5CBEDCF4-9BE7-4C1F-8627-CF2A0746C7DB")},
            {ProductDepartment, new Guid("5CBEDCF4-9BE7-4C1F-8627-CF2A0746C7DA")}
        };

        private Task GenerateDepartmentsAsync()
        {
            var departments = Departments.Select(p => new Department
            {
                Id = p.Value,
                Name = p.Key,
                Description = p.Key,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            });

            return _repository.BulkInsertAsync(departments.ToArray());
        }
    }
}