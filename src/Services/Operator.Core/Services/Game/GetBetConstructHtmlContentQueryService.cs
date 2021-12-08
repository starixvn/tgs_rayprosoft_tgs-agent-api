using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.ServiceUtility;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator.Services.Game
{
    public class GetBetConstructHtmlContentQueryService : IGetService<GetBetConstructHtmlContentQuery, BetConstructHtmlContentInfo>
    {
        private readonly IGameProvidersOperatorPluggableClient _pluggableClient;

        public GetBetConstructHtmlContentQueryService(
            IGameProvidersOperatorPluggableClient pluggableClient
            )
        {
            _pluggableClient = pluggableClient ?? throw new ArgumentNullException(nameof(pluggableClient));
        }

        public async ValueTask<BetConstructHtmlContentInfo> GetAsync(GetBetConstructHtmlContentQuery query, CancellationToken cancellationToken = default)
        {
            var isMobile = query.PlayerTokenData.PlatformType == PlatformType.Mobile;

            var bcUri = await _pluggableClient
                .GetGameUrlAsync(
                    query.PlayerTokenData.PlayerCode,
                    query.PlayerTokenData.LoginRecordId,
                    isMobile,
                    2100001,
                    query.CultureName,
                    query.IP,
                    null)
                .ConfigureAwait(false);

            var html =
                "<div id=\"bcsportsbookcontainer\"></div>" +
                $"<script id=\"bcsportsbook\" src=\"{bcUri}\"></script>";

            return new BetConstructHtmlContentInfo()
            {
                Html = html
            };
        }
    }
}
