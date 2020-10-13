using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Data.SqlClient;
using TGS.Apis.BackendOffice.DataService.Enums;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.DataService.Repositories;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Base.Interfaces;
using TGS.Core.Infrastructures.Security;

namespace TGS.Apis.BackendOffice.DataService.DataInitializers
{
    [ExcludeFromCodeCoverage]
    public sealed partial class DataInitializer : IDataInitializer
    {
        private const string DefaultTimeZone = "+0800";
        private const string AdminUsername = "admin.enabled";
        private const string UserPassword = "TgsP@ssw0rd";
        private const string MarketDepartment = "市場";
        private const string ProductDepartment = "產品";

        private readonly IHashProvider _hashProvider;
        private readonly IBackendOfficeRepository _repository;

        private static readonly Dictionary<string, Guid> Departments = new Dictionary<string, Guid>
        {
            { MarketDepartment , new Guid("5CBEDCF4-9BE7-4C1F-8627-CF2A0746C7DB") },
            { ProductDepartment, new Guid("5CBEDCF4-9BE7-4C1F-8627-CF2A0746C7DA") }
        };

        public DataInitializer(IBackendOfficeRepository repository)
        {
            _repository = repository;

            var config = new TgsHashConfig();
            _hashProvider = new HashProvider(config);
        }

        private readonly IRole[] _roles =
        {
            new AdminRole(), new ResellerAdminRole()
        };

        void IDataInitializer.SetInitialData()
        {
            if (_repository.Users.Any())
            {
                return;
            }

            GenerateDepartments();
            GenerateRoles();
            GenerateUsers();
            GenerateSettings();
        }

        private void GenerateDepartments()
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
            _repository.BulkInsert(departments.ToArray());
        }

        private void GenerateRoles()
        {
            var roles = CreateRoleWithMenus(_roles);
            var command = @"
INSERT INTO [dbo].[Roles]
    ([Id]
    ,[RoleType]
    ,[Name]
    ,[MenuSetting]
    ,[DisplayName]
    ,[Description]
    ,[Status]
    ,[CreatedOn]
    ,[UpdatedOn]
    ,[CreatedByUsername]
    ,[UpdatedByUsername])
VALUES
    (@id
    ,@roleType
    ,@name
    ,@menuSetting
    ,@displayName
    ,@description
    ,@status
    ,SYSDATETIMEOFFSET()
    ,SYSDATETIMEOFFSET()
    ,@createdByUsername
    ,@updatedByUsername)
";
            foreach (var role in roles)
            {
                var args = new[]
                {
                    new SqlParameter { ParameterName = "@id", Value = role.Id, DbType = DbType.Guid },
                    new SqlParameter { ParameterName = "@roleType", Value = (int)role.RoleType, DbType = DbType.Int16 },
                    new SqlParameter { ParameterName = "@name", Value = role.Name, DbType = DbType.String },
                    new SqlParameter { ParameterName = "@menuSetting", Value = role.MenuSetting, DbType = DbType.String },
                    new SqlParameter { ParameterName = "@displayName", Value = role.Name, DbType = DbType.String },

                    new SqlParameter { ParameterName = "@description", Value = role.DisplayName, DbType = DbType.String },
                    new SqlParameter { ParameterName = "@status", Value = role.Status, DbType = DbType.String },
                    new SqlParameter { ParameterName = "@createdByUsername", Value = role.CreatedByUsername, DbType = DbType.String },
                    new SqlParameter { ParameterName = "@updatedByUsername", Value = role.UpdatedByUsername, DbType = DbType.String }
                };

                _repository.ExecuteSqlCommand(command, CommandType.Text, parameters: args);
            }
        }

        private Role[] CreateRoleWithMenus(IRole[] roles)
        {
            return roles.Select(role => new Role
            {
                Id = role.Id,
                Name = role.Name,
                DisplayName = role.Name,
                Description = role.Description,
                Status = RoleStatus.Enabled,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                MenuSetting = _jsonSerialization.SerializeToString(GetMenuSettings()),
                RoleType = role.RoleType
            }).ToArray();
        }


        private void GenerateUsers()
        {
            string[] usernames =
            {
                "hank.wu", "lester.wei", "sam.chuang", "houdini.sheng"
            };

            var users = new List<User>
            {
                CreateUser("admin.disabled", UserStatus.Disabled, Roles.Admin, Departments[MarketDepartment]),
                CreateUser("admin.enabled", UserStatus.Enabled, Roles.Admin, Departments[MarketDepartment]),
                CreateUser("admin.archived", UserStatus.Archived, Roles.Admin, Departments[MarketDepartment]),
                CreateUser("admin.locked", UserStatus.Locked, Roles.Admin, Departments[ProductDepartment])
            };

            users.AddRange(usernames.Select(username => CreateUser(username, UserStatus.Enabled, Roles.Admin, Departments[ProductDepartment])));
            _repository.BulkInsert(users);
        }
    }
}