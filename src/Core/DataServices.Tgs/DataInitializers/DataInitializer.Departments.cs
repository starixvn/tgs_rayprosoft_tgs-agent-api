using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        private const string MarketDepartment = "市場";
        private const string ProductDepartment = "產品";

        private static readonly Dictionary<string, Guid> AdminDepartments = new()
        {
            {MarketDepartment, new("5CBEDCF4-9BE7-4C1F-8627-CF2A0746C7DB")},
            {ProductDepartment, new("5CBEDCF4-9BE7-4C1F-8627-CF2A0746C7DA")}
        };

        private static Department[] Departments;

        private async Task GenerateDepartmentsAsync()
        {
            Departments = _Resellers.SelectMany(x =>
            {
                var resellerDepartments = new Department
                {
                    Id = GuidExtensions.NewSequentialGuid(),
                    Name = $"{x.Name}-{MarketDepartment}",
                    Description = MarketDepartment,
                    ResellerId = x.Id,
                    CreatedByUsername = _AdminUsername,
                    UpdatedByUsername = _AdminUsername,
                    CreatedOn = DateTimeOffset.Now,
                    UpdatedOn = DateTimeOffset.Now
                };
                var licenseeDepartments = x.Licensees.Select(l => new Department
                {
                    Id = GuidExtensions.NewSequentialGuid(),
                    Name = $"{l.Name}-{MarketDepartment}",
                    Description = MarketDepartment,
                    ResellerId = x.Id,
                    LicenseeId = l.Id,
                    CreatedByUsername = _AdminUsername,
                    UpdatedByUsername = _AdminUsername,
                    CreatedOn = DateTimeOffset.Now,
                    UpdatedOn = DateTimeOffset.Now
                });

                var brandDepartments = x.Licensees.SelectMany(l =>
                {
                    return l.Brands.Select(b => new Department
                    {
                        Id = GuidExtensions.NewSequentialGuid(),
                        Name = $"{b.Name}-{MarketDepartment}",
                        Description = MarketDepartment,
                        ResellerId = x.Id,
                        LicenseeId = l.Id,
                        BrandId = b.Id,
                        CreatedByUsername = _AdminUsername,
                        UpdatedByUsername = _AdminUsername,
                        CreatedOn = DateTimeOffset.Now,
                        UpdatedOn = DateTimeOffset.Now
                    });
                });
                return licenseeDepartments.Union(brandDepartments).Append(resellerDepartments);
            }).ToArray();

            var departments = AdminDepartments.Select(p => new Department
            {
                Id = p.Value,
                Name = p.Key,
                Description = p.Key,
                CreatedByUsername = _AdminUsername,
                UpdatedByUsername = _AdminUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            }).ToArray();

            await _repository.Departments.AddRangeAsync(Departments.Union(departments));
            await _repository.SaveChangesAsync();
        }
    }
}