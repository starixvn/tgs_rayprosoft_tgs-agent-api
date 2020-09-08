using System;
using TGS.Apis.BackendOffice.DataService.Enums;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Users
{
    public class UpdateUserRequest
    {
        public string Name { get; set; }
        public Guid RoleId { get; set; }
        public Guid DepartmentId { get; set; }
        public UserStatus Status { get; set; }
    }
}
