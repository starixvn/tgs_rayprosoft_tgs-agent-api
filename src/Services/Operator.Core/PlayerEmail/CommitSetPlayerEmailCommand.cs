namespace TGS.Services.Operator.PlayerEmail
{
    public record CommitSetPlayerEmailCommand(string PlayerCode, string Token, string ValidateCode);
}
