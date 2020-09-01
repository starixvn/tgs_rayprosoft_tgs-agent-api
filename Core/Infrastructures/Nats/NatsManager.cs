using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client;

namespace TGS.Infrastructures.Nats
{
    public interface INatsManager : IDisposable
    {
        Task PublishAsync<T>(string topic, T content);

        Task PublishAsync<T>(T content);

        IAsyncSubscription SubscribeAsync<T>(string topic, string queueName, Action<NatsMessage<T>> action);

        IAsyncSubscription SubscribeAsync<T>(string topic, Action<NatsMessage<T>> action);

        string GetConnectStatus();
    }

    public class NatsManager : INatsManager
    {
        private readonly ILogger _logger;
        private Lazy<IConnection> _connectionFactory;
        private readonly IMessagePackProvider _messagePackProvider;
        private ConnState _state;
        private readonly IOptions<NatsSetting> _natsSetting;

        public NatsManager(
            IMessagePackProvider messagePackProvider,
            ILogger<NatsManager> logger,
            IOptions<NatsSetting> natsSetting)
        {
            _logger = logger;
            _messagePackProvider = messagePackProvider;
            _natsSetting = natsSetting;
            CreateConnection();
        }

        private void CreateConnection()
        {
            _connectionFactory = new Lazy<IConnection>(Connect);
        }

        string INatsManager.GetConnectStatus()
        {
            return _connectionFactory.Value.State.ToString();
        }

        private IConnection Connect()
        {
            IConnection conn = null;
            try
            {
                var opts = ConnectionFactory.GetDefaultOptions();
                opts.Url = _natsSetting.Value.Urls;
                opts.Timeout = _natsSetting.Value.ConnectTimeoutMs;
                opts.MaxReconnect = _natsSetting.Value.ConnectTimeoutMs;
                opts.PingInterval = _natsSetting.Value.PingInterval;
                opts.SubscriptionBatchSize = _natsSetting.Value.SubscriptionBatchSize;
                opts.ReconnectedEventHandler += ReconnectedEventHandler;
                opts.DisconnectedEventHandler += DisconnectedEventHandler;
                opts.ClosedEventHandler += ClosedEventHandler;
                opts.AsyncErrorEventHandler += AsyncErrorEventHandler;

                var connectionFactory = new ConnectionFactory();
                conn = connectionFactory.CreateConnection(opts);
                _state = ConnState.CONNECTED;
                _logger.LogInformation($"Succeeded to create a NATS connection to {opts.Url}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to connect to NATS. {ex}");
                CreateConnection();
            }
            return conn;
        }

        Task INatsManager.PublishAsync<T>(string topic, T content)
            => PublishAsync(topic, content);

        Task INatsManager.PublishAsync<T>(T content)
            => PublishAsync(typeof(T).Name, content);


        private Task PublishAsync<T>(string topic, T content)
        {
            return Task.Run(() =>
            {
                if (_connectionFactory.IsValueCreated && _state != ConnState.CONNECTED) return;
                var data = _messagePackProvider.Serialize(content);
                
                _connectionFactory.Value?.Publish(topic, data);
                try
                {
                    _connectionFactory.Value?.Flush();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Flush failed. Subject:{topic} Error:{ex}");
                }
            });
        }

        IAsyncSubscription INatsManager.SubscribeAsync<T>(string topic, string queueName, Action<NatsMessage<T>> action)
        {
            return _connectionFactory.Value?.SubscribeAsync(
                topic,
                queueName,
                EventHandlerFactory(action));
        }

        IAsyncSubscription INatsManager.SubscribeAsync<T>(string topic, Action<NatsMessage<T>> action)
        {
            return _connectionFactory.Value?.SubscribeAsync(
                topic,
                EventHandlerFactory(action));
        }

        private EventHandler<MsgHandlerEventArgs> EventHandlerFactory<T>(Action<NatsMessage<T>> action)
        {
            return (sender, args) =>
            {
                var natsMessage = NatMessageFactory<T>(args);
                action?.Invoke(natsMessage);
            };
        }

        private NatsMessage<T> NatMessageFactory<T>(MsgHandlerEventArgs args)
        {
            return new NatsMessage<T>(
                args.Message.Subject,
                args.Message.Reply,
                args.Message.Data,
                _messagePackProvider.Deserialize<T>(args.Message.Data),
                args.Message.ArrivalSubscription
            );
        }

        private void ReconnectedEventHandler(object sender, ConnEventArgs e)
        {
            _state = ConnState.CONNECTED;
            _logger.LogWarning($"Reconnected to NATS. {e.Conn.Opts.Url}");
        }

        private void DisconnectedEventHandler(object sender, ConnEventArgs e)
        {
            _state = ConnState.DISCONNECTED;
            _logger.LogError($"Disconnected to NATS. {e.Conn.Opts.Url}");
        }

        private void ClosedEventHandler(object sender, ConnEventArgs e)
        {
            _state = ConnState.DISCONNECTED;
            _logger.LogWarning($"NATS connection closed. {e.Conn.Opts.Url}");
        }

        private void AsyncErrorEventHandler(object sender, ErrEventArgs e)
        {
            _logger.LogError($"Error occurred. Subject:{e.Subscription.Subject} Error:{e.Error}");
        }

        void IDisposable.Dispose()
        {
            if (!_connectionFactory.IsValueCreated)
                return;
            _connectionFactory.Value?.Drain();
            _connectionFactory.Value?.Dispose();
        }
    }
}