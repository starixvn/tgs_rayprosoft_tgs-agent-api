using MessagePack;

namespace TGS.Infrastructures.Log.Events
{
    [MessagePackObject((true))]
    public abstract class LogBusEvent
    {
    }
}
