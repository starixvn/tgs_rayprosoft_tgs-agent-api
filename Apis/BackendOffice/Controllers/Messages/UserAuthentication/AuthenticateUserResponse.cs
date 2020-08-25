using System;

namespace TGS.Apis.TGS.Apis.BackendOffice.Controllers.Messages.UserAuthentication
{
    public class AuthenticateUserResponse : MessageResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string TimeZone { get; set; }
        public string Role { get; set; }
        public string CsrfToken { get; set; }
        public RoleMenuViewModel[] Menus { get; set; }
    }
}
