using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.GlobalApi.Extensions;
using TGS.Apis.GlobalApi.Models.Data;
using TGS.Apis.GlobalApi.Models.Messages;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;
using TGS.Services.Operations;

namespace TGS.Apis.GlobalApi.Handlers
{
    public sealed class GetBrandPlayersForMockSiteHandler
        : IRequestHandler<GetBrandPlayersRequest, PagedResponse<BrandPlayersResult>>
    {
        private const int NewPlayersCount = 2;
        private readonly IBrandOperations _brandOperations;
        private readonly IPasswordHandler _passwordHandler;
        private readonly Func<ITgsRepository> _repoFactory;
        private readonly ITgsRepository _repository;
        private readonly ITransactionManager _transactionManager;

        public GetBrandPlayersForMockSiteHandler(
            ITgsRepository repository,
            Func<ITgsRepository> repoFactory,
            IBrandOperations brandOperations,
            IPasswordHandler passwordHandler,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _repoFactory = repoFactory;
            _brandOperations = brandOperations;
            _passwordHandler = passwordHandler;
            _transactionManager = transactionManager;
        }

        async Task<PagedResponse<BrandPlayersResult>>
            IRequestHandler<GetBrandPlayersRequest, PagedResponse<BrandPlayersResult>>.Handle(
                GetBrandPlayersRequest request, CancellationToken cancellationToken)
        {
            // if ShowAll = true, return the first page with default row counts
            var newRequest = request.ShowAll ? request with { ShowAll = false, Page = 0, Rows = 10 } : request;

            var brandCode = newRequest.BrandCode;
            var currencyCode = newRequest.CurrencyCode;
            var playerType = newRequest.PlayerType;
            var query = _repository.Players.AsNoTracking()
                .Where(p => p.Brand.Code == brandCode && p.DefaultWalletCurrencyCode == currencyCode &&
                    p.Type == playerType);

            var count = await query.CountAsync(cancellationToken);
            if (count == 0)
            {
                await AddNewPlayersAsync(brandCode, currencyCode, playerType);
            }

            return await query.Select(p => new BrandPlayersResult
            {
                Username = p.Username,
                PlayerCode = p.PlayerCode,
                Status = p.Status
            }).CreatePagedResponseAsync<PagedResponse<BrandPlayersResult>, BrandPlayersResult>(newRequest);
        }

        private async Task AddNewPlayersAsync(string brandCode, string currencyCode, PlayerType playerType)
        {
            var brandId = await _brandOperations.GetBrandIdByCodeAsync(brandCode);
            var betLimit = await _brandOperations.GetBrandDefaultBetLimitAsync(brandId, currencyCode);

            var prefix = playerType == PlayerType.RealPlayer ? "RP" : "TP";
            var players = Enumerable.Range(0, NewPlayersCount).Select(idx =>
                CreatePlayer(brandId, currencyCode, playerType, betLimit, prefix, idx + 1)).ToArray();

            await _transactionManager.ExecuteTransactionAsync(_repoFactory, IsolationLevel.ReadCommitted,
                Guid.NewGuid().ToString(),
                async repo =>
                {
                    await repo.BulkInsertAsync(players);
                    await repo.BulkInsertAsync(players.SelectMany(p => p.Wallets).ToArray());

                    return Task.CompletedTask;
                });
        }

        private Player CreatePlayer(Guid brandId, string currencyCode, PlayerType playerType, int betLimit,
            string prefix, int num)
        {
            const string pwd = "TgsP@ssw0rd";
            var now = DateTimeOffset.Now;

            var walletId = GuidExtensions.NewSequentialGuid();
            var (salt, password) = _passwordHandler.GenerateCredential(pwd);
            var username = $"{prefix}{num:00}_{currencyCode.ToLower()}";
            var playerCode = PlayerCodeGenerator.NewId();

            return new()
            {
                PlayerCode = playerCode,
                Username = username,
                Type = playerType,
                BrandId = brandId,
                Salt = salt,
                Password = password,
                BetLimitId = betLimit,
                InvitationCode = InvitationCodeGenerator.NewId(),
                Status = PlayerStatus.Enabled,
                DefaultWalletCurrencyCode = currencyCode,
                DefaultWalletId = walletId,
                RegisteredOn = now,
                LastActiveOn = now,
                UpdatedOn = now,
                UpdatedByUsername = username,
                HasAgent = false,
                Wallets = new[]
                {
                    new Wallet
                    {
                        Id = walletId,
                        PlayerCode = playerCode,
                        CurrencyCode = currencyCode,
                        ExternalCurrencyCode = currencyCode,
                        Balance = 0,
                        CreatedOn = now,
                        UpdatedOn = now
                    }
                }
            };
        }
    }
}