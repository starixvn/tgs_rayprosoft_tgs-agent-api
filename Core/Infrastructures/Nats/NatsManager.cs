using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NATS.Client;
using NATS.Client.Rx;
using NATS.Client.Rx.Ops;
using TGS.Core.Infrastructures.Log;

namespace TGS.Core.Infrastructures.Nats
{
    public interface INatsManager : IDisposable
    {
        Task PublishAsync<T>(string topic, T content);

        Task PublishAsync<T>(T content);

        IAsyncSubscription SubscribeAsync<T>(string topic, string queueName, Action<NatsMessage<T>> action);

        IAsyncSubscription SubscribeAsync<T>(string topic, Action<NatsMessage<T>> action);

        INATSObservable<Msg> SubscribeWithObserveAsync(string topic);

        T Deserialize<T>(byte[] data);
        string GetConnectStatus();
    }

    public class NatsManager : INatsManager
    {
        private readonly IMessagePackProvider _messagePackProvider;
        private readonly IOptions<NatsSetting> _natsSetting;
        private readonly IServiceLog<NatsManager> _serviceLog;
        private Lazy<IConnection> _connectionFactory;
        private ConnState _state;

        public NatsManager(
            IMessagePackProvider messagePackProvider,
            IServiceLog<NatsManager> serviceLog,
            IOptions<NatsSetting> natsSetting)
        {
            _serviceLog = serviceLog;
            _messagePackProvider = messagePackProvider;
            _natsSetting = natsSetting;
            CreateConnection();
        }

        string INatsManager.GetConnectStatus()
        {
            return _connectionFactory.Value.State.ToString();
        }

        Task INatsManager.PublishAsync<T>(string topic, T content)
        {
            return PublishAsync(topic, content);
        }

        Task INatsManager.PublishAsync<T>(T content)
        {
            return PublishAsync(typeof(T).Name, content);
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

        void IDisposable.Dispose()
        {
            if (!_connectionFactory.IsValueCreated)
            {
                return;
            }

            _connectionFactory.Value?.Drain();
            _connectionFactory.Value?.Dispose();
        }

        INATSObservable<Msg> INatsManager.SubscribeWithObserveAsync(string topic)
        {
            return _connectionFactory.Value?.Observe(topic).Where(m => m.Data?.Any() == true);
        }

        T INatsManager.Deserialize<T>(byte[] data)
        {
            return _messagePackProvider.Deserialize<T>(data);
        }

        private void CreateConnection()
        {
            _connectionFactory = new Lazy<IConnection>(Connect);
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
                _serviceLog.LogInformation($"Succeeded to create a NATS connection to {opts.Url}.");
            }
            catch (Exception ex)
            {
                _serviceLog.LogError($"Failed to connect to NATS. {ex}");
                CreateConnection();
            }

            return conn;
        }


        private Task PublishAsync<T>(string topic, T content)
        {
            return Task.Run(() =>
            {
                if (_connectionFactory.IsValueCreated && _state != ConnState.CONNECTED)
                {
                    return;
                }

                var data = _messagePackProvider.Serialize(content);

                _connectionFactory.Value?.Publish(topic, data);
                try
                {
                    _connectionFactory.Value?.Flush();
                }
                catch (Exception ex)
                {
                    _serviceLog.LogError($"Flush failed. Subject:{topic} Error:{ex}");
                }
            });
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
            _serviceLog.LogWarning($"Reconnected to NATS. {e.Conn.Opts.Url}");
        }

        private void DisconnectedEventHandler(object sender, ConnEventArgs e)
        {
            _state = ConnState.DISCONNECTED;
            _serviceLog.LogError($"Disconnected to NATS. {e.Conn.Opts.Url}");
        }

        private void ClosedEventHandler(object sender, ConnEventArgs e)
        {
            _state = ConnState.DISCONNECTED;
            _serviceLog.LogWarning($"NATS connection closed. {e.Conn.Opts.Url}");
        }

        private void AsyncErrorEventHandler(object sender, ErrEventArgs e)
        {
            _serviceLog.LogError($"Error occurred. Subject:{e.Subscription.Subject} Error:{e.Error}");
        }
    }
}