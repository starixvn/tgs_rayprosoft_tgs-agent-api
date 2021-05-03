using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.GlobalApi.Models.Messages;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Transactions;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;

namespace TGS.Apis.GlobalApi.Handlers
{
    public sealed class
        CreatePlayerAndLoginHandler : IRequestHandler<CreatePlayerAndLoginRequest, CreatePlayerAndLoginResponse>
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IPasswordHandler _passwordHandler;
        private readonly Func<ITgsRepository> _repoFactory;
        private readonly ITokenProvider _tokenProvider;
        private readonly ITransactionManager _transactionManager;

        public CreatePlayerAndLoginHandler(
            Func<ITgsRepository> repoFactory,
            IBrandOperations brandOperations,
            IPasswordHandler passwordHandler,
            ITransactionManager transactionManager,
            ITokenProvider tokenProvider)
        {
            _repoFactory = repoFactory;
            _brandOperations = brandOperations;
            _passwordHandler = passwordHandler;
            _transactionManager = transactionManager;
            _tokenProvider = tokenProvider;
        }

        async Task<CreatePlayerAndLoginResponse>
            IRequestHandler<CreatePlayerAndLoginRequest, CreatePlayerAndLoginResponse>.Handle(
                CreatePlayerAndLoginRequest request, CancellationToken cancellationToken)
        {
            var brandId = await _brandOperations.GetBrandIdByCodeAsync(request.BrandCode);
            var betLimit = await _brandOperations.GetBrandDefaultBetLimitAsync(brandId, request.CurrencyCode);
            var username = request.Username;
            var currencyCode = request.CurrencyCode;

            var (playerCode, loginRecordId) = await _transactionManager.ExecuteTransactionAsync(_repoFactory,
                IsolationLevel.ReadCommitted,
                Guid.NewGuid().ToString(),
                async repo =>
                {
                    var player = await repo.Players.AsNoTracking().FirstOrDefaultAsync(p =>
                        p.BrandId == brandId && p.Username.ToLower() == username.ToLower(), cancellationToken);

                    if (player != null && player.DefaultWalletCurrencyCode.IgnoreCaseNotEquals(currencyCode))
                    {
                        throw new CurrencyNotMatchingException(currencyCode, player.DefaultWalletCurrencyCode);
                    }

                    if (player == null)
                    {
                        var initWalletAmount = request.InitialWalletAmount <= 0 ? 5000 : request.InitialWalletAmount;
                        player = CreatePlayer(brandId, request.CurrencyCode, username, betLimit,
                            initWalletAmount);

                        await repo.Players.AddAsync(player, cancellationToken);
                    }

                    var createdOn = DateTimeOffset.Now;
                    var loginRecord = new PlayerLoginRecord
                    {
                        Id = Guid.NewGuid(),
                        PlayerCode = player.PlayerCode,
                        BrandId = brandId,
                        Status = LoginStatus.Enabled,
                        PlatformType = PlatformType.Desktop,
                        IpAddress = string.Empty,
                        CreatedOn = createdOn,
                        ExpiredOn = createdOn.AddSeconds(Consts.TokenExpireSeconds.PlayerLogin)
                    };

                    await repo.PlayerLoginRecords.AddAsync(loginRecord, cancellationToken);

                    await repo.SaveChangesAsync(cancellationToken);

                    return (player.PlayerCode, loginRecord.Id);
                });

            return new()
            {
                PlayerCode = playerCode,
                LoginRecordId = loginRecordId,
                EncryptedToken = await _tokenProvider.EncryptAsync(loginRecordId)
            };
        }

        private Player CreatePlayer(Guid brandId, string currencyCode, string username, int betLimit,
            decimal initWalletAmount)
        {
            const string pwd = "TgsP@ssw0rd";
            var now = DateTimeOffset.Now;

            var walletId = GuidExtensions.NewSequentialGuid();
            var (salt, password) = _passwordHandler.GenerateCredential(pwd);
            var (withdrawSalt, withdrawPassword) = _passwordHandler.GenerateCredential(pwd);
            var playerCode = PlayerCodeGenerator.NewId();

            return new()
            {
                PlayerCode = playerCode,
                Username = username,
                Type = PlayerType.TestPlayer,
                BrandId = brandId,
                Salt = salt,
                Password = password,
                WithdrawSalt = withdrawSalt,
                WithdrawPassword = withdrawPassword,
                BetLimitId = betLimit,
                Status = PlayerStatus.Enabled,
                DefaultWalletCurrencyCode = currencyCode,
                DefaultWalletId = walletId,
                RegisteredOn = now,
                LastActiveOn = now,
                UpdatedOn = now,
                UpdatedByUsername = username,
                Wallets = new[]
                {
                    new Wallet
                    {
                        Id = walletId,
                        PlayerCode = playerCode,
                        CurrencyCode = currencyCode,
                        ExternalCurrencyCode = currencyCode,
                        Balance = initWalletAmount,
                        CreatedOn = now,
                        UpdatedOn = now
                    }
                }
            };
        }
    }
}