using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Models.GameProviderFundTransferTransaction;

namespace TGS.GameProviders.Repositories
{
    public interface IGameProviderFundTransferTransactionRepository
    {
        ValueTask<Guid?> AddAsync(AddGameProviderFundTransferTransactionRequest request);
        ValueTask<Guid?> UpdateAsync(UpdateGameProviderFundTransferTransactionRequest request);
        ValueTask<GetGameProviderFundTransferTransactionResponse> GetAsync(Guid gameProviderFundTransferTransactionId);
        GetGameProviderFundTransferTransactionResponse[] GetByGameProviderTransactionId(Guid gameProviderId, string gameProviderTransactionId);
    }
}
