using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Services.Operations;
using Tgs.ServiceUtility;
using TGS.Services.Operator.ApplicationServices.Player;

namespace TGS.Services.Operator.ApplicationServices.Player
{
    internal class GetUserNameQueryService : IGetService<GetUserNameQuery, bool>
    {
        private readonly IPlayerOperations _playerOperations;

        public GetUserNameQueryService(IPlayerOperations playerOperations)
        {
            _playerOperations = playerOperations ?? throw new System.ArgumentNullException(nameof(playerOperations));
        }

        public async ValueTask<bool> GetAsync(GetUserNameQuery query, CancellationToken cancellationToken = default)
        {
            var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(query.BrandId, query.Username);

            return playerCode.IsNotNullOrEmpty();
        }
    }
}
