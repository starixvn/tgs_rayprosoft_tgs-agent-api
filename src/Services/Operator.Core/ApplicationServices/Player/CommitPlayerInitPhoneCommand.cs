namespace TGS.Services.Operator.ApplicationServices.Player
{
    public record CommitPlayerInitPhoneCommand(
        string SecurityKey,
        string ValidateCode,
        string IpAddress,
        string? HostName);
}
