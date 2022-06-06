using System;

namespace TGS.Services.Operator.ApplicationServices.Player;

public record CommitInitPlayerEmailCommand(
    string SecurityKey,
    string ValidateCode,
    string IpAddress,
    string HostName,
    Guid BrandId);
