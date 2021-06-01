using System;

namespace TGS.Services.Operator.Services.StationLetters
{
    public record SetHaveReadCommand(string PlayerCode, Guid LetterId);
}
