namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record ResetPasswordRequest(string Token, string ValidateCode, string NewPassword);
}
