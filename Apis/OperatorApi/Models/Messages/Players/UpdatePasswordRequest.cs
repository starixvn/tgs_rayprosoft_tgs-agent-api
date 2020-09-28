namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public class UpdatePasswordRequest : BaseRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
