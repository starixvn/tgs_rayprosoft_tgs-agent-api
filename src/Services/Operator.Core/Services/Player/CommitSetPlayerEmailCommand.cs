namespace TGS.Services.Operator.Services.Player
{
    public record CommitSetPlayerEmailCommand(string PlayerCode, string SecurityKey, string ValidateCode);
}
