using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Bson;
using Tgs.MessageQueue;

namespace TGS.Services.GameOperations
{
    internal class SingleWalletMessageSender : ISingleWalletMessageSender
    {
        private readonly IMessageSender _messageSender;

        public SingleWalletMessageSender(IMessageSender messageSender)
        {
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        public ValueTask PublishGameActionAsync(GameActionMessage data)
            => _messageSender.PublishAsync("tgs.singlewallet.gameaction", data.ToBson());
    }
}
