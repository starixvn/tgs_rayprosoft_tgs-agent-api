namespace TGS.Services.Operator.Player
{
    public record CommitSetPlayerEmailCommand(string PlayerCode, string SecurityKey, string ValidateCode);
}
