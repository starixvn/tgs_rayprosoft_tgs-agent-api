using System;
using TGS.Core.Models.Services;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Users.Authentication
{
    public class UserInfo
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string TimeZone { get; set; }
        public string Role { get; set; }
        public Guid RoleId { get; set; }
        public RoleType RoleType { get; set; }
        public MenuSetting[] Menus { get; set; }
    }
}