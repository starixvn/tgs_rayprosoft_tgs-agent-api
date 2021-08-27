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
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.WMCasino.Enums;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Middlewares
{
    public class WMCasinoDispatcherMiddleware : IMiddleware
    {
        private static readonly IReadOnlyDictionary<string, string> _Routes = new Dictionary<string, string>
        {
            { "CallBalance", "/api/wm/balance" },
            { "PointInout", "/api/wm/transfer" },
            { "TimeoutBetReturn", "/api/wm/canceltransfer" },
            { "SendMemberReport", "/api/wm/betdetail" }
        };

        private static readonly RecyclableMemoryStreamManager _RecyclableMemoryStreamManager =
            new RecyclableMemoryStreamManager();

        private readonly IJsonSerializationProvider _json;
        private readonly JsonSerializerOptions _jsonSerializeOptions;
        private readonly ISettingOperations _settingOperations;

        public WMCasinoDispatcherMiddleware(
            IJsonSerializationProvider json,
            ISettingOperations settingOperations)
        {
            _json = json;
            _settingOperations = settingOperations;

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

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<WMCasinoConfiguration>(SettingConsts
                    .GameProviderConfigurations.WMCasino);

            items = ParseWalletActionType(items);
            items = ParseBetDetail(items);
            items = ParseBetResults(configuration.MaxResultCount, items);

            var json = _json.SerializeToString(items, _jsonSerializeOptions);

            var stream = _RecyclableMemoryStreamManager.GetStream(Encoding.UTF8.GetBytes(json));
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

        private Dictionary<string, object> ParseBetDetail(Dictionary<string, object> items)
        {
            const string betDetailKey = "betdetail";

            if (!items.TryGetValue(betDetailKey, out var value))
            {
                return items;
            }

            var data = value?.ToString();
            if (string.IsNullOrWhiteSpace(data))
            {
                return items;
            }

            var details = _json.DeserializeFromString<Dictionary<string, decimal>>(data);
            items[betDetailKey] = details;

            return items;
        }

        private static Dictionary<string, object> ParseBetResults(int maxResultCount, Dictionary<string, object> items)
        {
            const string resultKey = "result";

            if (!items.TryGetValue("totalorder", out var totalOrder) ||
                !int.TryParse(totalOrder.ToString(), out var totalResultCount) ||
                totalResultCount <= 0)
            {
                return items;
            }

            if (!items.TryGetValue("page", out var page) ||
                !int.TryParse(page.ToString(), out var pageNumber) ||
                pageNumber <= 0)
            {
                return items;
            }

            var maxCountOfPages = maxResultCount * pageNumber;
            var resultCount = totalResultCount > maxCountOfPages
                ? maxResultCount
                : totalResultCount - maxResultCount * (pageNumber - 1);

            var details = Enumerable.Range(0, resultCount)
                .Select(idx =>
                {
                    var keyPrefix = $"{resultKey}[{idx}]";
                    var collection = items.Where(p => p.Key.IgnoreCaseContains(keyPrefix))
                        .Select(p => new
                        {
                            p.Key,
                            Name = p.Key.Replace(keyPrefix, string.Empty)
                                .Replace("[", string.Empty).Replace("]", string.Empty),
                            p.Value
                        });

                    return new
                    {
                        Index = idx,
                        Detail = collection
                    };
                }).ToArray();

            items[resultKey] = details
                .Select(d => d.Detail
                    .ToDictionary(p => p.Name, p => p.Value))
                .ToArray();

            foreach (var key in details.SelectMany(d => d.Detail).Select(d => d.Key))
            {
                items.Remove(key);
            }

            return items;
        }
    }
}