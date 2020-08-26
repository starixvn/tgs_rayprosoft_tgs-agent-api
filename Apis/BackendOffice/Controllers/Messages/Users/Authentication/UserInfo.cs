using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Users.Authentication
{
    public class UserInfo
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string TimeZone { get; set; }
        public string Role { get; set; }
        public RoleMenuViewModel[] Menus { get; set; }
    }
}
