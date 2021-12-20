namespace TGS.Services.Operator.Services.Player
{
    public record CommitPlayerInitPhoneCommand(
        string SecurityKey,
        string ValidateCode,
        string IpAddress,
        string? HostName);
}