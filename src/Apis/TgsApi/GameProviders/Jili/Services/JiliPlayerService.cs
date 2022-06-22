namespace TGS.Apis.TgsApi.GameProviders.Jili.Services;

internal class JiliPlayerService : IJiliPlayerService
{
    public string ToJiliPlayerId(string brandCode, string playerCode)
        => $"{brandCode}_{playerCode}";
}
