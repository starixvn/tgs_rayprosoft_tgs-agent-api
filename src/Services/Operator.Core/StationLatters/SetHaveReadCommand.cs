using System;

namespace TGS.Services.Operator.StationLetters
{
    public record SetHaveReadCommand(string PlayerCode, Guid LetterId);
}
