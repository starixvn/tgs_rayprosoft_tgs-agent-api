using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Shared.Game.Models.WMCasino.Enums;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Middlewares
{
    public class WMCasinoDispatcherMiddleware : IMiddleware
    {
        private static readonly string _ResultKey = "result";

        private static readonly IReadOnlyDictionary<string, string> _Routes = new Dictionary<string, string>
        {
            {"CallBalance", "/api/wm/balance"},
            {"PointInout", "/api/wm/transfer"},
            {"TimeoutBetReturn", "api/wm/canceltransfer"},
            {"SendMemberReport", "api/bets/postbetdetails"}
        };

        private static readonly RecyclableMemoryStreamManager _RecyclableMemoryStreamManager =
            new RecyclableMemoryStreamManager();

        private readonly IJsonSerializationProvider _json;
        private readonly JsonSerializerOptions _jsonSerializeOptions;

        public WMCasinoDispatcherMiddleware(IJsonSerializationProvider json)
        {
            _json = json;

            _jsonSerializeOptions = new JsonSerializerOptions
            {
                WriteIndented = false,
                NumberHandling = JsonNumberHandling.Strict
            };
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var form = context.Request.Form;
            if (!form.TryGetValue("cmd", out var cmd) && string.IsNullOrEmpty(cmd))
            {
                throw new ArgumentException("Missing cmd parameter");
            }

            if (!_Routes.TryGetValue(cmd, out var route))
            {
                await next(context);
                return;
            }

            var items = form.Keys.ToDictionary(k => k, v => (object)form[v].ToString());
            var data = await context.Request.ReadAsStringAsync();
            var decoded = WebUtility.UrlDecode(data);

            // add raw data for logging
            items["raw"] = decoded;
            items["path"] = context.Request.Path;

            items = ParseWalletActionType(items);
            items = ParseBetDetail(items);

            var json = _json.SerializeToString(items, _jsonSerializeOptions);

            await using var stream = _RecyclableMemoryStreamManager.GetStream(Encoding.UTF8.GetBytes(json));
            context.Request.Body = stream;
            context.Request.ContentType = MediaTypeNames.Application.Json;
            context.Request.Path = route;

            await next(context);
        }

        private static Dictionary<string, object> ParseWalletActionType(Dictionary<string, object> items)
        {
            if (!items.TryGetValue("code", out var value))
            {
                return items;
            }

            if (!Enum.TryParse<WalletActionType>(value.ToString(), out var actionType))
            {
                throw new ArgumentException("Invalid code value");
            }

            items["walletactiontype"] = actionType;

            return items;
        }

        private static Dictionary<string, object> ParseBetDetail(Dictionary<string, object> items)
        {
            const string betDetailKey = "betdetail";

            var matchedKeys = items.Keys.Where(s => s.StartsWith(betDetailKey)).ToArray();

            if (matchedKeys.Length == 0)
            {
                return items;
            }

            var betDetail = new Dictionary<string, object>();

            foreach (var key in matchedKeys)
            {
                var value = items[key];
                var betType = key
                    .Replace(betDetailKey, string.Empty)
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty);

                betDetail[betType] = value;

                items.Remove(key);
            }

            items[betDetailKey] = betDetail;

            return items;
        }
    }
}