using System;

namespace TGS.Services.Operator.StationLatters
{
    public record SetHaveReadCommand(string PlayerCode, Guid LatterId);
}
