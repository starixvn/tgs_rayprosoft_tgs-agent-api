using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Core.Models.Services.Games;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.AsiaGaming.Enums;
using TGS.Shared.Game.Models.AsiaGaming.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.AsiaGaming.Middlewares
{
    public sealed class HandleAsiaGamingExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        private readonly XmlSerializer _xml;
        private readonly IGamePlayActions _action;
        private readonly IAsiaGamingActionModelMapper _map;
        
        public HandleAsiaGamingExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log,
            IGamePlayActions action,
            IAsiaGamingActionModelMapper map)
            : base(mapper, Consts.GameProvider.AsiaGaming, log, nameof(Consts.GameProvider.AsiaGaming).ToLower())
        {
            _xml = new XmlSerializer(typeof(TransferResponse));
            _action = action;
            _map = map;
        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception)
        {
            var error = (GameApiError)errorCode;

            var response = await GetTransferResponseAsync(context, error);
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = MediaTypeNames.Text.Xml;

            var xml = ParseToXml(response);
            await context.Response.WriteAsync(xml, Encoding.UTF8);
        }

        #region Private Method

        private async Task<object> GetTransferResponseAsync(HttpContext context, GameApiError error)
        {
            if (error is GameApiError.ERROR or GameApiError.INVALID_SESSION)
            {
                return _map.Response(error);
            }
            
            var playerCode = context.Items.Get(Consts.ContextItems.PlayerCode, string.Empty);
            if (playerCode.IsNullOrEmpty())
            {
                return _map.Response(error);
            }

            var result = await _action.GetBalanceAsync(Consts.GameProvider.AsiaGaming, playerCode);
            return _map.Response(error, result);
        }

        private string ParseToXml(object response)
        {
            var emptyNamespaces = new XmlSerializerNamespaces();
            emptyNamespaces.Add("", "");
            using var sw = new Utf8StringWriter();
            using var writer = XmlWriter.Create(sw);
            _xml.Serialize(writer, response, emptyNamespaces);
            return sw.ToString();
        }
        
        private class Utf8StringWriter : StringWriter
        {
            // Use UTF8 encoding but write no BOM to the wire
            public override Encoding Encoding
            {
                get { return new UTF8Encoding(false); } // in real code I'll cache this encoding.
            }
        }

        #endregion
    }
}