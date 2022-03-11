using System;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;

namespace TGS.Services.BackOffice.Infrastructure.DataService
{
    public interface IPlayerIncreaseAmountService
    {
        Task CreateAsync(Guid brandId, string playerCode, PlayerIncreaseAmountType transactionType,
            decimal amount, string remark, decimal bucketAmount, string username);
    }

    public class PlayerIncreaseAmountService : BaseRepositoryOperations<ITgsRepository>, IPlayerIncreaseAmountService
    {
        public PlayerIncreaseAmountService(ITgsRepository tgsRepository) : base(
            tgsRepository)
        {
        }

        public Task CreateAsync(Guid brandId, string playerCode, PlayerIncreaseAmountType transactionType,
            decimal amount, string remark, decimal bucketAmount, string username)
        {
            var vipLevel = new PlayerIncreaseAmount
            {
                BrandId = brandId,
                PlayerCode = playerCode,
                PlayerIncreaseAmountType = transactionType,
                Amount = amount,
                Remark = remark,
                BucketAmount = bucketAmount,
                CreatedByUsername = username,
                UpdatedByUsername = username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            return CreateEntityAsync(vipLevel);
        }
    }
}