using System.Linq;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Apis.BackendOffice.Controllers.Messages.Reseller;
using TGS.Core.DataServices.Models;

namespace TGS.Apis.BackendOffice.Mapping
{
    public class ResellerProfile : BaseProfile
    {
        public ResellerProfile()
        {
            CreateMap<CreateResellerRequest, Reseller>();
            CreateMap<UpdateResellerRequest, Reseller>();
            CreateMap<Reseller, ResellerModel>()
                .ForMember(model => model.ResellerCurrencies,
                    opt => opt.MapFrom(src => src.Currencies.Select(x => x.Currency)));
            CreateMap<Currency, ResellerCurrencyModel>();
            CreateMap<Language, ResellerLanguageModel>();
            CreateMap<Payment, PaymentModel>();
        }
    }
}