using AutoMapper;
using TGS.Apis.BackendOffice.Controllers.Messages.Reseller;
using TGS.Core.DataServices.Models;

namespace TGS.Apis.BackendOffice.Mapping
{
    public class MappingProfile : Profile
    {
        public static MapperConfiguration Config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        public MappingProfile()
        {
            CreateMap<string, string>().ConvertUsing<NullStringConverter>();
            // Domain to Resource
            //CreateMap<Music, MusicResource>();
            //CreateMap<Artist, ArtistResource>();

            //// Resource to Domain
            //CreateMap<MusicResource, Music>();
            //CreateMap<SaveMusicResource, Music>();
            CreateMap<CreateResellerRequest, Reseller>();
            CreateMap<UpdateResellerRequest, Reseller>();
            CreateMap<Reseller, ResellerModel>();
            CreateMap<Currency, ResellerCurrencyModel>();
            CreateMap<Language, ResellerLanguageModel>();
        }

        public class NullStringConverter : ITypeConverter<string, string>
        {
            string ITypeConverter<string, string>.Convert(string source, string destination, ResolutionContext context)
            {
                return source ?? string.Empty;
            }

            public string Convert(string source)
            {
                return source ?? string.Empty;
            }
        }
    }
}