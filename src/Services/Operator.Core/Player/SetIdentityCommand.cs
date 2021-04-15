namespace TGS.Services.Operator.Player
{
    public record SetIdentityCommand(string PlayerCode, string RealName, string Identity);
}
