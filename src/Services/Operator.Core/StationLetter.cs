using System;

namespace TGS.Services.Operator
{
    public record StationLetter(
        Guid Id,
        string Subject,
        string Content,
        DateTimeOffset CreateOn,
        bool IsRead);
}
