using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operator.GameProviders;
using TGS.Services.Operator.Games;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Game
{
    internal class GetGamesQueryService : IQueryService<GetGamesQuery, GameInfo>
    {
        private readonly IQueryService<GetAllGamesByGameTypeQuery, GameInfo> _queryService;

        public GetGamesQueryService(
            IQueryService<GetAllGamesByGameTypeQuery, GameInfo> queryService)
        {
            _queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
        }

        public IAsyncEnumerable<GameInfo> QueryAsync(
            GetGamesQuery query,
            CancellationToken cancellationToken = default)
        {
            var allQuery = new GetAllGamesByGameTypeQuery(
                query.BrandId,
                query.PlayerTokenData,
                query.PlatformType,
                query.Types);


            var allGames = _queryService.QueryAsync(allQuery, cancellationToken);

            return allGames.Where(
                gameInfo => gameInfo.GameProviderCode.Equals(
                    query.SubGameProviderCode,
                    StringComparison.InvariantCultureIgnoreCase));
        }
    }
}