using System;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace TGS.Infrastructures.Nats
{
    public interface IMessagePackProvider
    {
        T Deserialize<T>(byte[] arrayToDeserialize);

        byte[] Serialize<T>(T objectToSerialize);
    }

    public class MessagePackProvider : IMessagePackProvider
    {
        private readonly ILogger _logger;
        private readonly MessagePackSerializerOptions _messagePackSerializerOptions = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
        public MessagePackProvider(ILogger logger)
        {
            _logger = logger;
        }

        T IMessagePackProvider.Deserialize<T>(byte[] arrayToDeserialize)
        {
            try
            {
                return MessagePackSerializer.Deserialize<T>(arrayToDeserialize, _messagePackSerializerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to deserialize. {ex}");
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
                _logger.LogError($"Failed to serialize. {ex}");
                throw;
            }
        }
    }
}