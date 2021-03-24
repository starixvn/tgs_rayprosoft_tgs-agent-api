using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.BackOffice.Application.Models.PlayerBank;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IPlayerBankService
    {
        Task<PlayerBank> GetPlayerBankAsync(Expression<Func<PlayerBank, bool>> predicate,
            Func<IQueryable<PlayerBank>, IQueryable<PlayerBank>> preQuery = null);

        Task CreatePlayerBankAsync(CreatePlayerBankRequest request);
        Task UpdatePlayerBankAsync(PlayerBank playerBank, UpdatePlayerBankRequest request);
        Task DeletePlayerBankAsync(PlayerBank playerBank);
        Task<bool> ExistsAsync(Expression<Func<PlayerBank, bool>> predicate);
    }

    public class PlayerBankService : BaseRepositoryOperations<ITgsRepository>, IPlayerBankService
    {
        public PlayerBankService(ITgsRepository tgsRepository) : base(
            tgsRepository)
        {
        }

        Task<PlayerBank> IPlayerBankService.GetPlayerBankAsync(Expression<Func<PlayerBank, bool>> predicate,
            Func<IQueryable<PlayerBank>, IQueryable<PlayerBank>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        Task IPlayerBankService.UpdatePlayerBankAsync(
            PlayerBank playerBank, UpdatePlayerBankRequest request)
        {
            playerBank.RealName = request.RealName;
            playerBank.BankName = request.BankName;
            playerBank.BankAccount = request.BankAccount;
            playerBank.UpdatedByUsername = request.EditorUsername;
            playerBank.UpdatedOn = DateTimeOffset.Now;

            return UpdateEntityAsync(playerBank);
        }

        Task IPlayerBankService.CreatePlayerBankAsync(CreatePlayerBankRequest request)
        {
            var bank = new PlayerBank
            {
                PlayerCode = request.PlayerCode,
                RealName = request.RealName,
                BankAccount = request.BankAccount,
                BankName = request.BankName,
                CreatedByUsername = request.EditorUsername,
                UpdatedByUsername = request.EditorUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            return CreateEntityAsync(bank);
        }

        Task IPlayerBankService.DeletePlayerBankAsync(PlayerBank playerBank) => DeleteEntityAsync(playerBank);

        Task<bool> IPlayerBankService.ExistsAsync(Expression<Func<PlayerBank, bool>> predicate) =>
            ExistsAsync(predicate);
    }
}