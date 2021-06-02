namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Factories
{
    public interface IBetServiceFactory
    {
        IBetService Get(int productType);
    }
}