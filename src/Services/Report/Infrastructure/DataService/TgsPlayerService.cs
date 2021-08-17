using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Report.Application.Models.Player;

namespace TGS.Services.Report.Infrastructure.DataService
{
    public interface ITgsPlayerService
    {
        Task<Player> GetPlayerAsync(Expression<Func<Player, bool>> predicate,
            Func<IQueryable<Player>, IQueryable<Player>> preQuery = null);

        IQueryable<Player> GetPlayerQueries(Func<IQueryable<Player>, IQueryable<Player>> preQuery = null,
            bool tracking = false);

        Task<(PlayerAccountChangeModel[], int)> GetPlayerAccountChangeAsync(GetPlayerAccountChangesRequest request);
    }

    public class TgsPlayerService : BaseRepositoryOperations<ITgsRepository>, ITgsPlayerService
    {
        public readonly ITgsRepository _repository;

        public TgsPlayerService(ITgsRepository repository) : base(repository)
        {
            _repository = repository;
        }

        async Task<(PlayerAccountChangeModel[], int)> ITgsPlayerService.GetPlayerAccountChangeAsync(
            GetPlayerAccountChangesRequest request)
        {
            const string spName = "dbo.report_get_player_account_change_records";

            TransactionType? transactionType = request.AccountChangeType switch
            {
                AccountChangeType.Deposit                => TransactionType.Deposit,
                AccountChangeType.Withdrawal             => TransactionType.Withdrawal,
                AccountChangeType.Adjust                 => TransactionType.Adjust,
                AccountChangeType.Bonus                  => TransactionType.Bonus,
                AccountChangeType.Rebate                 => TransactionType.Rebate,
                AccountChangeType.Dividend               => TransactionType.Dividend,
                AccountChangeType.Promotion              => TransactionType.Promotion,
                AccountChangeType.GameProviderFundOut    => TransactionType.GameProviderFundOut,
                AccountChangeType.GameProviderFundIn     => TransactionType.GameProviderFundIn,
                AccountChangeType.CancelWithdrawal       => TransactionType.CancelWithdrawal,
                AccountChangeType.GameProviderDeposit    => TransactionType.GameProviderDeposit,
                AccountChangeType.GameProviderWithdrawal => TransactionType.GameProviderWithdrawal,
                _                                        => null
            };

            GameActionType? gameActionType = request.AccountChangeType switch
            {
                AccountChangeType.PlaceBet          => GameActionType.PlaceBet,
                AccountChangeType.WinBet            => GameActionType.WinBet,
                AccountChangeType.LoseBet           => GameActionType.LoseBet,
                AccountChangeType.FreeBet           => GameActionType.FreeBet,
                AccountChangeType.TieBet            => GameActionType.TieBet,
                AccountChangeType.CancelTransaction => GameActionType.CancelTransaction,
                AccountChangeType.LockAmount        => GameActionType.LockAmount,
                AccountChangeType.UnlockAmount      => GameActionType.UnlockAmount,
                AccountChangeType.WinJackpot        => GameActionType.WinJackpot,
                _                                   => null
            };

            var results = await _repository
                .SqlQueryMultipleAsync<PlayerAccountChangeModel>(spName, parameters:
                    new Dictionary<string, object>
                    {
                        {"_from", request.From},
                        {"_to", request.To},
                        {"_player_code", request.PlayerCode},
                        {"_transaction_type", transactionType},
                        {"_game_action_type", gameActionType},
                        {"_page_size", request.ShowAll ? int.MaxValue : request.Rows},
                        {"_page_number", request.ShowAll ? 1 : request.Page}
                    });
            return (results.ToArray(), results.Any() ? results.First().TotalCount : 0);
            ;
        }

        Task<Player> ITgsPlayerService.GetPlayerAsync(Expression<Func<Player, bool>> predicate,
            Func<IQueryable<Player>, IQueryable<Player>> preQuery) => GetEntityFirstOrDefaultAsync(predicate, preQuery);

        IQueryable<Player> ITgsPlayerService.GetPlayerQueries(
            Func<IQueryable<Player>, IQueryable<Player>> preQuery,
            bool tracking) =>
            GetQueriesAsync(preQuery, tracking);
    }
}