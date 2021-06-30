using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.GameProviders.Repositories.Models.GameProviderFundTransferTransaction;

namespace TGS.GameProviders.Repositories
{
    public interface IGameProviderFundTransferTransactionRepository
    {

        ValueTask<Guid?> AddAsync(AddGameProviderFundTransferTransactionRequest request);
        ValueTask<Guid?> UpdateAsync(UpdateGameProviderFundTransferTransactionRequest request);
        ValueTask<GetGameProviderFundTransferTransactionResponse> GetAsync(Guid gameProviderFundTransferTransactionId);
    }

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
        async ValueTask<Guid?> IGameProviderFundTransferTransactionRepository.AddAsync(AddGameProviderFundTransferTransactionRequest request)
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

            await _repository.GameProviderFundTransferTransactions.AddAsync(data).ConfigureAwait(false);
            await _repository.SaveChangesAsync().ConfigureAwait(false);

            return gameProviderFundTransferTransactionId;
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        async ValueTask<Guid?> IGameProviderFundTransferTransactionRepository.UpdateAsync(UpdateGameProviderFundTransferTransactionRequest request)
        {
            var data = _repository.GameProviderFundTransferTransactions.Where(x => x.GameProviderFundTransferTransactionId == request.GameProviderFundTransferTransactionId).FirstOrDefault();
            if (data == null)
            {
                return new();
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

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="gameProviderFundTransferTransactionId">The game provider fund transfer transaction identifier.</param>
        /// <returns></returns>
        ValueTask<GetGameProviderFundTransferTransactionResponse> IGameProviderFundTransferTransactionRepository.GetAsync(
            Guid gameProviderFundTransferTransactionId)
        {
            var data = _repository.GameProviderFundTransferTransactions
                .Where(x => x.GameProviderFundTransferTransactionId == gameProviderFundTransferTransactionId)
                .FirstOrDefault();

            if (data == null)
            {
                return new();
            }

            return ValueTask.FromResult(new GetGameProviderFundTransferTransactionResponse()
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
            });
        }
    }
}
