using System;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.DataInitializers
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
            RoleType roleType = RoleType.Admin)
        {
            var (salt, passwordHash) = _passwordHandler.GenerateCredential(UserPassword);
            userId ??= GuidExtensions.NewSequentialGuid();

            return new()
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