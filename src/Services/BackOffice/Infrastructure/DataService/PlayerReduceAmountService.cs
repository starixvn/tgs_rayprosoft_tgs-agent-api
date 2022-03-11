using System;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IPlayerReduceAmountService
    {
        Task CreateAsync(Guid brandId, string playerCode, PlayerReduceAmountType transactionType,
            string transferTarget, decimal amount, string remark, decimal bucketAmount, string username);
    }

    public class PlayerReduceAmountService : BaseRepositoryOperations<ITgsRepository>, IPlayerReduceAmountService
    {
        public PlayerReduceAmountService(ITgsRepository tgsRepository) : base(
            tgsRepository)
        {
        }

        public Task CreateAsync(Guid brandId, string playerCode, PlayerReduceAmountType transactionType,
            string transferTarget, decimal amount, string remark, decimal bucketAmount, string username)
        {
            var vipLevel = new PlayerReduceAmount
            {
                BrandId = brandId,
                PlayerCode = playerCode,
                PlayerReduceAmountType = transactionType,
                TransferTarget = transferTarget,
                Amount = amount,
                Remark = remark,
                CreatedByUsername = username,
                UpdatedByUsername = username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            return CreateEntityAsync(vipLevel);
        }
    }
}