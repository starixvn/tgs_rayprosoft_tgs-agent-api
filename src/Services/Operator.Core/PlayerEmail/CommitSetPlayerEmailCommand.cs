namespace TGS.Services.Operator.PlayerEmail
{
    public record CommitSetPlayerEmailCommand(string PlayerCode, string SecurityKey, string ValidateCode);
}
