namespace TGS.Apis.TgsApi.GameProviders.Jili.Services
{
    public interface IJiliPlayerService
    {
        string ToJiliPlayerId(string brandCode, string playerCode);
    }

    internal class JiliPlayerService : IJiliPlayerService
    {
        public string ToJiliPlayerId(string brandCode, string playerCode)
            => $"{brandCode}_{playerCode}";
    }
}
