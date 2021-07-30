using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.GameProviders.Repositories.Models.GameProviderFundTransferTransaction;

namespace TGS.GameProviders.Repositories
{
    internal sealed class GameProviderFundTransferTransactionRepository : IGameProviderFundTransferTransactionRepository
    {
        private readonly ITgsRepository _repository;

        public GameProviderFundTransferTransactionRepository(ITgsRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async ValueTask<Guid?> AddAsync(AddGameProviderFundTransferTransactionRequest request)
        {
            var gameProviderFundTransferTransactionId = GuidExtensions.NewSequentialGuid();

            var data = new GameProviderFundTransferTransaction()
            {
                GameProviderFundTransferTransactionId = gameProviderFundTransferTransactionId,
                TransactionId = request.TransactionId,
                GameProviderId = request.GameProviderId,
                WalletId = request.WalletId,
                GameProviderTransactionId = request.GameProviderTransactionId,
                PlayerCode = request.PlayerCode,
                FundSource = request.FundSource,
                FromWallet = request.FromWallet,
                ToWallet = request.ToWallet,
                Status = FundTransferTransactionStatus.WaitForResponse,
                Amount = request.Amount,
                Balance = request.Balance,
                CreatedOn = request.CreatedOn,
                UpdatedOn = request.CreatedOn,
                Description = request.Description
            };

            await _repository.GameProviderFundTransferTransactions
                .AddAsync(data)
                .ConfigureAwait(false);

            await _repository.SaveChangesAsync().ConfigureAwait(false);

            return gameProviderFundTransferTransactionId;
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="gameProviderFundTransferTransactionId">The game provider fund transfer transaction identifier.</param>
        /// <returns></returns>
        public async ValueTask<GetGameProviderFundTransferTransactionResponse> GetAsync(
            Guid gameProviderFundTransferTransactionId)
        {
            var data = await _repository.GameProviderFundTransferTransactions
                .Where(x => x.GameProviderFundTransferTransactionId == gameProviderFundTransferTransactionId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (data == null)
            {
                return new GetGameProviderFundTransferTransactionResponse();
            }

            return new GetGameProviderFundTransferTransactionResponse()
            {
                GameProviderFundTransferTransactionId = data.GameProviderFundTransferTransactionId,
                TransactionId = data.TransactionId,
                GameProviderId = data.GameProviderId,
                WalletId = data.WalletId,
                GameProviderTransactionId = data.GameProviderTransactionId,
                PlayerCode = data.PlayerCode,
                FundSource = data.FundSource,
                FromWallet = data.FromWallet,
                ToWallet = data.ToWallet,
                Status = data.Status,
                Amount = data.Amount,
                Balance = data.Balance,
                CreatedOn = data.CreatedOn,
                UpdatedOn = data.UpdatedOn,
                Description = data.Description,
            };
        }

        /// <summary>
        /// Gets the by game provider transaction identifier.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="gameProviderTransactionId">The game provider transaction identifier.</param>
        /// <returns></returns>
        public GetGameProviderFundTransferTransactionResponse[] GetByGameProviderTransactionId(Guid gameProviderId, string gameProviderTransactionId)
        {
            return (from GameProviderFundTransferTransactions in _repository.GameProviderFundTransferTransactions.AsQueryable()
                    where GameProviderFundTransferTransactions.GameProviderId == gameProviderId && GameProviderFundTransferTransactions.GameProviderTransactionId == gameProviderTransactionId
                    select new GetGameProviderFundTransferTransactionResponse()
                    {
                        GameProviderFundTransferTransactionId = GameProviderFundTransferTransactions.GameProviderFundTransferTransactionId,
                        TransactionId = GameProviderFundTransferTransactions.TransactionId,
                        GameProviderId = GameProviderFundTransferTransactions.GameProviderId,
                        WalletId = GameProviderFundTransferTransactions.WalletId,
                        GameProviderTransactionId = GameProviderFundTransferTransactions.GameProviderTransactionId,
                        PlayerCode = GameProviderFundTransferTransactions.PlayerCode,
                        FundSource = GameProviderFundTransferTransactions.FundSource,
                        FromWallet = GameProviderFundTransferTransactions.FromWallet,
                        ToWallet = GameProviderFundTransferTransactions.ToWallet,
                        Status = GameProviderFundTransferTransactions.Status,
                        Amount = GameProviderFundTransferTransactions.Amount,
                        Balance = GameProviderFundTransferTransactions.Balance,
                        CreatedOn = GameProviderFundTransferTransactions.CreatedOn,
                        UpdatedOn = GameProviderFundTransferTransactions.UpdatedOn,
                        Description = GameProviderFundTransferTransactions.Description,
                    }).ToArray();
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async ValueTask<Guid?> UpdateAsync(UpdateGameProviderFundTransferTransactionRequest request)
        {
            var data = await _repository.GameProviderFundTransferTransactions
                .Where(
                    x => x.GameProviderFundTransferTransactionId == request.GameProviderFundTransferTransactionId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (data == null)
            {
                return null;
            }

            data.TransactionId = request.TransactionId;
            data.GameProviderTransactionId = request.GameProviderTransactionId;
            data.Status = request.Status;
            data.Balance = request.Balance;
            data.UpdatedOn = request.UpdatedOn;
            data.Description = request.Description;

            _repository.GameProviderFundTransferTransactions.Update(data);

            await _repository.SaveChangesAsync().ConfigureAwait(false);

            return data.GameProviderFundTransferTransactionId;
        }
    }
}
