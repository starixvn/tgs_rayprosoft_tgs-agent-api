namespace TGS.Services.Operator.Services.PayPassowrd
{
    public record ChangePayPasswordCommand(
        string PlayerCode,
        string NewPassword,
        string ValidateCode,
        string SecurityKey);
}
