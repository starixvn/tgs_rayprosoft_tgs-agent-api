using MessagePack;

namespace TGS.Core.Infrastructures.Log.Events
{
    [MessagePackObject(true)]
    public abstract class LogBusEvent
    {
    }
}
