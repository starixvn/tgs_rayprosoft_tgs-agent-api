using System;
using System.Linq;
using System.Linq.Expressions;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Apis.BackendOffice.Controllers.Messages.Reseller;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.Mapping
{
    public class ResellerProfile : BaseProfile
    {
        public ResellerProfile()
        {
            Expression<Func<Payment, bool>>
                dtoExpression = r => GetDesignatedUsersAsync(r).Any();

            CreateMap<CreateResellerRequest, Reseller>();
            CreateMap<UpdateResellerRequest, Reseller>();
            CreateMap<Reseller, ResellerModel>()
                .ForMember(model => model.ResellerCurrencies,
                    opt => opt.MapFrom(src => src.Currencies.Select(x => x.Currency)));
            CreateMap<Currency, ResellerCurrencyModel>();
            CreateMap<Language, ResellerLanguageModel>();
            CreateMap<Payment, PaymentFeeModel>();
            CreateMap<Payment, PaymentModel>()
                .ForMember(model => model.Number, opt => opt.MapFrom(p => p.DisplayOrder))
                .ForMember(model => model.PaymentVendorName, opt => opt.MapFrom(p => p.PaymentVendor.Name))
                .ForMember(model => model.PaymentMethodName, opt => opt.MapFrom(p => p.PaymentMethod.Name))
                .ForMember(model => model.PaymentChannelName, opt => opt.MapFrom(p => p.PaymentChannel.Name))
                .ForMember(model => model.Whitelist,
                    opt => opt.MapFrom(p =>
                        p.PaymentPlayerList.Where(x => x.ListType == PlayerListType.White)
                            .Select(x => x.Player.UpdatedByUsername)))
                .ForMember(model => model.Blacklist,
                    opt => opt.MapFrom(p =>
                        p.PaymentPlayerList.Where(x => x.ListType == PlayerListType.Black)
                            .Select(x => x.Player.UpdatedByUsername)))
                .ForMember(model => model.DesignatedUsers, opt => opt.MapFrom(dtoExpression))
                .ForMember(model => model.VipLevelIds,
                    opt => opt.MapFrom(p => p.VipLevels.Select(x => x.VipLevelId)));
        }

        private static string[] GetDesignatedUsersAsync(Payment payment)
        {
            var vipLevelIds = payment.VipLevels.Select(x => x.VipLevelId).ToArray();
            var playerCodes = payment.PaymentPlayerList.Where(x => x.ListType == PlayerListType.White)
                .Select(p => p.PlayerCode).ToArray();
            var isAll = payment.PlayerListCondition == PlayerListCondition.All;

            return payment.PaymentPlayerList
                .Where(x => playerCodes.Contains(x.PlayerCode))
                .Where(x =>
                    x.Player.AccumulatedDeposit >= payment.DepositedAmountMin &&
                    (payment.DepositedAmountUnlimited || x.Player.AccumulatedDeposit < payment.DepositedAmountMax))
                .Where(isAll && payment.IdentityAuthentication,
                    x => x.Player.IdentityAuthentication)
                .Where(isAll && payment.PhoneAuthentication,
                    x => x.Player.PhoneAuthentication)
                .Where(isAll && payment.VipLevels.Any(),
                    x => x.Player.VipId != null && vipLevelIds.Contains(x.Player.VipId.Value))
                .Select(x => x.Player.RealName).ToArray();
        }
    }
}