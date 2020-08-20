using System;
using MessagePack;
using TGS.Infrastructures.Log;

namespace TGS.Infrastructures.Nats
{
    public interface IMessagePackProvider
    {
        T Deserialize<T>(byte[] arrayToDeserialize);

        byte[] Serialize<T>(T objectToSerialize);
    }

    public class MessagePackProvider : IMessagePackProvider
    {
        private readonly IOsLog _osLog;
        private readonly MessagePackSerializerOptions _messagePackSerializerOptions = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
        public MessagePackProvider(IOsLog osLog)
        {
            _osLog = osLog;
        }

        T IMessagePackProvider.Deserialize<T>(byte[] arrayToDeserialize)
        {
            try
            {
                return MessagePackSerializer.Deserialize<T>(arrayToDeserialize, _messagePackSerializerOptions);
            }
            catch (Exception ex)
            {
                _osLog.WriteEventLogError($"Failed to deserialize. {ex}");
                throw;
            }
        }

        byte[] IMessagePackProvider.Serialize<T>(T objectToSerialize)
        {
            try
            {
                return MessagePackSerializer.Serialize(objectToSerialize, _messagePackSerializerOptions);
            }
            catch (Exception ex)
            {
                _osLog.WriteEventLogError($"Failed to serialize. {ex}");
                throw;
            }
        }
    }
}