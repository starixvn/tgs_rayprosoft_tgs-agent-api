namespace TGS.Services.Operator.Services.Player
{
    public record CommitSetPlayerPhoneCommand(string PlayerCode, string SecurityKey, string ValidateCode);
}