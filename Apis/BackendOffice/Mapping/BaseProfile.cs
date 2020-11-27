using AutoMapper;

namespace TGS.Apis.BackendOffice.Mapping
{
    public class BaseProfile : Profile
    {
        public static MapperConfiguration Config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<BaseProfile>();
            cfg.AddProfile<ResellerProfile>();
        });

        public BaseProfile()
        {
            CreateMap<string, string>().ConvertUsing<NullStringConverter>();
            // Domain to Resource
            //CreateMap<Music, MusicResource>();
            //CreateMap<Artist, ArtistResource>();

            //// Resource to Domain
            //CreateMap<MusicResource, Music>();
            //CreateMap<SaveMusicResource, Music>();
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