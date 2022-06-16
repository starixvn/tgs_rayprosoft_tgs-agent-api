using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Core.Models;
using TGS.Services.Operations.Validators;

namespace TGS.Services.Operator.ApplicationServices.Player;

internal class GetLogoutQueryService : IGetService<GetLogoutQuery, LogoutInfo>
{
    private readonly IPlayerLoginRecordRepository _loginRecordRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IStatusValidator _statusValidator;

    public GetLogoutQueryService(
        IStatusValidator statusValidator,
        IPlayerRepository playerRepository,
        IPlayerLoginRecordRepository loginRecordRepository)
    {
        _statusValidator = statusValidator ?? throw new ArgumentNullException(nameof(statusValidator));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _loginRecordRepository = loginRecordRepository ?? throw new ArgumentNullException(nameof(loginRecordRepository));
    }

    public async ValueTask<LogoutInfo> GetAsync(GetLogoutQuery query, CancellationToken cancellationToken = default)
    {
        await _statusValidator.ThrowIfBrandStatusIsNotOneOfAsync(query.BrandId, ValidStatus.Default);

        var player = await _playerRepository.GetPlayerInfoAsync(query.PlayerCode)
            .ConfigureAwait(false);

        if (player == null)
        {
            throw new PlayerNotFoundException("");
        }

        await _loginRecordRepository
            .DeauthorizePlayerAsync(query.LoginRecordId)
            .ConfigureAwait(false);

        return new LogoutInfo
        {
            UpdateTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
    }
}
