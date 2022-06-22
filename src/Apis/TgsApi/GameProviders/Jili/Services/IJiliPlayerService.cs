namespace TGS.Apis.TgsApi.GameProviders.Jili.Services;

public interface IJiliPlayerService
{
    string ToJiliPlayerId(string brandCode, string playerCode);
}