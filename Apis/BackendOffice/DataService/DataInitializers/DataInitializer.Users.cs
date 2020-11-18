using System;
using TGS.Apis.BackendOffice.DataService.Enums;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Apis.BackendOffice.DataService.DataInitializers
{
    public partial class DataInitializer
    {
        private User CreateUser(
            string userName,
            UserStatus status,
            Guid roleId,
            Guid departmentId,
            Guid? userId = null,
            Guid? resellerId = null,
            RoleType roleType = RoleType.Admin)
        {
            var (salt, passwordHash) = _passwordHandler.GenerateCredential(UserPassword);
            userId ??= GuidExtensions.NewSequentialGuid();

            return new User
            {
                Id = userId.Value,
                RoleId = roleId,
                RoleType = roleType,
                DepartmentId = departmentId,
                Username = userName,
                Password = passwordHash,
                Salt = salt,
                Name = userName,
                ResellerId = resellerId,
                Status = status,
                TimeZone = DefaultTimeZone,
                CreatedByUsername = AdminUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedByUsername = AdminUsername,
                UpdatedOn = DateTimeOffset.Now
            };
        }
    }
}