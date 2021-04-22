namespace TGS.Services.Operator.Player
{
    public record CommitSetPlayerPhoneCommand(string PlayerCode, string SecurityKey, string ValidateCode);
}
