using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Settings;

namespace TGS.Services.Promotion
{
    public class PromotionTypeFactory : IPromotionTypeFactory
    {
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly ITgsRepository _repository;

        public PromotionTypeFactory(ITgsRepository repository, IJsonSerializationProvider jsonSerializationProvider)
        {
            _repository = repository;
            _jsonSerializationProvider = jsonSerializationProvider;
        }

        async ValueTask<bool> IPromotionTypeFactory.IsDepositTypeAsync(string promotionType)
        {
            var promotionsJson = await _repository.Settings.AsQueryable()
                .FirstOrDefaultAsync(x => x.Key == SettingConsts.PromotionType);

            var promotionTypeGroups =
                _jsonSerializationProvider.DeserializeFromString<PromotionTypeGroup[]>(promotionsJson.Value);

            var promotionGroupType =
                promotionTypeGroups.FirstOrDefault(x => x.Types.Select(t => t.Name).Contains(promotionType));

            return promotionGroupType?.Name == Consts.PromotionGroupType.Deposit;
        }
    }
}