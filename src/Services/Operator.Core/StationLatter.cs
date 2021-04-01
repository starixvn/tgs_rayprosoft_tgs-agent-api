using System;

namespace TGS.Services.Operator
{
    public record StationLatter(Guid Id, string Subject, string Content, DateTimeOffset CreateOn);
}
