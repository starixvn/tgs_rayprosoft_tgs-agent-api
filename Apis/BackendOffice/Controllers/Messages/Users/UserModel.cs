using System;
using TGS.Apis.BackendOffice.DataService.Enums;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Users
{
    public class UserModel
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string DepartmentName { get; set; }
        public string Role { get; set; }
        public string LastLoginIp { get; set; }
        public DateTimeOffset? LastLoginTime { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public UserStatus Status { get; set; }
    }
}
