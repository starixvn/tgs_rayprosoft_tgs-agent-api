using System;
using System.Text;
using TGS.Apis.BackendOffice.DataService.Enums;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Infrastructures.Extensions;
using TGS.Infrastructures.Security;

namespace TGS.Apis.BackendOffice.DataService.DataInitializers
{
    public partial class DataInitializer
    {
        private User CreateUser(
            string userName,
            UserStatus status,
            Guid roleId,
            Guid? userId = null,
            Guid? licenseeId = null,
            Guid? resellerId = null,
            RoleType roleType = RoleType.Admin)
        {
            var (salt, passwordHash) = GenerateCredential(UserPassword);
            userId ??= GuidExtensions.NewSequentialGuid();

            return new User
            {
                Id = userId.Value,
                RoleId = roleId,
                RoleType = roleType,
                Username = userName,
                Password = passwordHash,
                Salt = salt,
                Name = userName,
                Email = $"{userName}@rayprosoft.tw",
                ResellerId = resellerId,
                Status = status,
                TimeZone = DefaultTimeZone,
                CreatedByUsername = AdminUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedByUsername = AdminUsername,
                UpdatedOn = DateTimeOffset.Now
            };
        }

        private (byte[], byte[]) GenerateCredential(string password)
        {
            ITgsHashConfig tgsHashConfig = new TgsHashConfig();
            IHashProvider hashProvider = new HashProvider(tgsHashConfig);

            var salt = hashProvider.GenerateRandomSalt(32);
            var hashedPassword = hashProvider.Hash(
                Encoding.UTF8.GetBytes(password),
                hashProvider.Hash(salt),
                tgsHashConfig.HashAlgorithm);
            return (salt, hashedPassword);
        }
    }
}