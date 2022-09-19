using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.MessageQueue;

namespace TGS.Apis.BackendOffice.MessageQueue;

public class BatchAdjustBalanceHandler : IMessageHandler
{
    public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
