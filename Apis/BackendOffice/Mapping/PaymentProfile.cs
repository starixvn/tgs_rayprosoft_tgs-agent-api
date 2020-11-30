using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Apis.BackendOffice.DataService;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.Mapping
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile(IPaymentMappingShared paymentMappingShared)
        {
            Expression<Func<Payment, bool>>
                dtoExpression = r => paymentMappingShared.GetDesignatedUsers(r).Any();

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
    }
}