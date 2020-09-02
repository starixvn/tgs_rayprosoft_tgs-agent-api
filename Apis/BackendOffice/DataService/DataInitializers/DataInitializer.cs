using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using TGS.Apis.BackendOffice.DataService.Enums;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Apis.BackendOffice.DataService.DataInitializers
{
    public interface IDataInitializer
    {
        void SetInitialData();
    }

    [ExcludeFromCodeCoverage]
    public sealed partial class DataInitializer : IDataInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private const string DefaultTimeZone = "+0800";
        private const string AdminUsername = "admin.enabled";
        private const string UserPassword = "TgsP@ssw0rd";

        public DataInitializer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        private readonly IRole[] _roles =
        {
            new AdminRole(), new ResellerAdminRole()
        };

        void IDataInitializer.SetInitialData()
        {
            using var serviceScope = _scopeFactory.CreateScope();
            var repository = serviceScope.ServiceProvider.GetRequiredService<IBackendOfficeRepository>();
            if (repository.Users.Any())
            {
                return;
            }

            GenerateRoles(repository);
            GenerateUsers(repository);
            GenerateSettings(repository);
        }

        private void GenerateRoles(IBackendOfficeRepository repo)
        {
            var roles = CreateRoleWithMenus(_roles);
            var command = @"
INSERT INTO [dbo].[Roles]
    ([Id]
    ,[RoleType]
    ,[Name]
    ,[Department]
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
    ,@department
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

                    new SqlParameter { ParameterName = "@department", Value = role.Department, DbType = DbType.String },
                    new SqlParameter { ParameterName = "@menuSetting", Value = role.MenuSetting, DbType = DbType.String },
                    new SqlParameter { ParameterName = "@displayName", Value = role.Name, DbType = DbType.String },

                    new SqlParameter { ParameterName = "@description", Value = role.DisplayName, DbType = DbType.String },
                    new SqlParameter { ParameterName = "@status", Value = role.Status, DbType = DbType.String },
                    new SqlParameter { ParameterName = "@createdByUsername", Value = role.CreatedByUsername, DbType = DbType.String },
                    new SqlParameter { ParameterName = "@updatedByUsername", Value = role.UpdatedByUsername, DbType = DbType.String }
                };
                repo.ExecuteSqlCommand(command, CommandType.Text, args);
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
                Department = "技術部門",
                Status = RoleStatus.Enabled,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                MenuSetting = _jsonSerialization.SerializeToString(GetMenuSettings(), _jsonSerializerOptions),
                RoleType = role.RoleType
            }).ToArray();
        }


        private void GenerateUsers(IBackendOfficeRepository repository)
        {
            string[] usernames =
            {
                "hank.wu", "lester.wei", "sam.chuang", "houdini.sheng"
            };

            var users = new List<User>
            {
                CreateUser("admin.disabled", UserStatus.Disabled, Roles.Admin),
                CreateUser("admin.enabled", UserStatus.Enabled, Roles.Admin),
                CreateUser("admin.archived", UserStatus.Archived, Roles.Admin),
                CreateUser("admin.locked", UserStatus.Locked, Roles.Admin)
            };

            users.AddRange(usernames.Select(username => CreateUser(username, UserStatus.Enabled, Roles.Admin)));
            repository.BulkInsert(users);
        }
    }
}