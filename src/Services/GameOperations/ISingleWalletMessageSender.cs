using System.Threading.Tasks;

namespace TGS.Services.GameOperations
{
    public interface ISingleWalletMessageSender
    {
        ValueTask PublishGameActionAsync(GameActionMessage data);
    }
}
