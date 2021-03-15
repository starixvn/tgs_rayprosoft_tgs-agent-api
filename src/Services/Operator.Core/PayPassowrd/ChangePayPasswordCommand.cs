namespace TGS.Services.Operator.PayPassowrd
{
    public record ChangePayPasswordCommand(
        string PlayerCode,
        string NewPassword,
        string ValidateCode,
        string Token);
}
