namespace TGS.Services.Operator
{
    public record VarifyForgotPasswordCodeCommand(
        string Token,
        string ValidateCode);
}
