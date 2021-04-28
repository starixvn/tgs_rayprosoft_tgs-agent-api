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
        Task<PlayerBankInfo> GetPlayerBankAsync(Expression<Func<PlayerBankInfo, bool>> predicate,
            Func<IQueryable<PlayerBankInfo>, IQueryable<PlayerBankInfo>> preQuery = null);

        Task CreatePlayerBankAsync(CreatePlayerBankRequest request);
        Task UpdatePlayerBankAsync(PlayerBankInfo playerBank, UpdatePlayerBankRequest request);
        Task DeletePlayerBankAsync(PlayerBankInfo playerBank);
        Task<bool> ExistsAsync(Expression<Func<PlayerBankInfo, bool>> predicate);
    }

    public class PlayerBankService : BaseRepositoryOperations<ITgsRepository>, IPlayerBankService
    {
        public PlayerBankService(ITgsRepository tgsRepository) : base(
            tgsRepository)
        {
        }

        Task<PlayerBankInfo> IPlayerBankService.GetPlayerBankAsync(Expression<Func<PlayerBankInfo, bool>> predicate,
            Func<IQueryable<PlayerBankInfo>, IQueryable<PlayerBankInfo>> preQuery) =>
            GetEntityFirstOrDefaultAsync(predicate, preQuery);

        Task IPlayerBankService.UpdatePlayerBankAsync(
            PlayerBankInfo playerBank, UpdatePlayerBankRequest request)
        {
            //playerBank.RealName = request.RealName;
            //playerBank.BankName = request.BankName;
            //playerBank.BankAccount = request.BankAccount;
            playerBank.UpdatedByUsername = request.EditorUsername;
            playerBank.UpdatedOn = DateTimeOffset.Now;

            return UpdateEntityAsync(playerBank);
        }

        Task IPlayerBankService.CreatePlayerBankAsync(CreatePlayerBankRequest request)
        {
            var bank = new PlayerBankInfo
            {
                PlayerCode = request.PlayerCode,
                //RealName = request.RealName,
                //BankAccount = request.BankAccount,
                //BankName = request.BankName,
                CreatedByUsername = request.EditorUsername,
                UpdatedByUsername = request.EditorUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            return CreateEntityAsync(bank);
        }

        Task IPlayerBankService.DeletePlayerBankAsync(PlayerBankInfo playerBank) => DeleteEntityAsync(playerBank);

        Task<bool> IPlayerBankService.ExistsAsync(Expression<Func<PlayerBankInfo, bool>> predicate) =>
            ExistsAsync(predicate);
    }
}