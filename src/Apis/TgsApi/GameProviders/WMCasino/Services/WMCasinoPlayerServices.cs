namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoPlayerServices
    {
        string GetPlayerCode(string wmPlayerId, string prefix, string suffix);

        string ToWMCasinoPlayerId(string playerCode, string prefix, string suffix);
    }

    public class WMCasinoPlayerServices : IWMCasinoPlayerServices
    {
        public string GetPlayerCode(string wmPlayerId, string prefix, string suffix)
            => wmPlayerId.Replace($"{prefix}_", string.Empty).Replace($"_{suffix}", string.Empty);

        public string ToWMCasinoPlayerId(string playerCode, string prefix, string suffix)
            => $"{prefix}_{playerCode}_{suffix}";
    }
}