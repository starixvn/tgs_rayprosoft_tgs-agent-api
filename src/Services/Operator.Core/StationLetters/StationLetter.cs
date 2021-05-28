using System;
using TGS.Services.Operator;

namespace TGS.Services.Operator.StationLetters
{
    public record StationLetter(
        Guid Id,
        string Subject,
        string Content,
        DateTimeOffset CreateOn,
        bool IsRead);
}
