namespace TGS.Services.Operator.PlayerIdentity
{
    public record SetIdentityCommand(string PlayerCode, string RealName, string Identity);
}
