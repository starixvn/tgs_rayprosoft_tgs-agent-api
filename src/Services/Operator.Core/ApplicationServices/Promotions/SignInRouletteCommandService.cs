using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Promotions;

internal class SignInRouletteCommandService : IGetService<SignInRouletteCommand, RouletteResult>
{
    public ValueTask<RouletteResult> GetAsync(SignInRouletteCommand command, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
