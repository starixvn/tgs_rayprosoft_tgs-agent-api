using System;
using TGS.Core.DataServices.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.DataInitializers
{
    public partial class DataInitializer
    {
        private const string DefaultTimeZone = "+0800";
        private const string UserPassword = "TgsP@ssw0rd";

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
                DepartmentId = departmentId,
                Username = userName,
                Password = passwordHash,
                Salt = salt,
                Name = userName,
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