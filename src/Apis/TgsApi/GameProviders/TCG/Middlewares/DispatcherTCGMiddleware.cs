using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using TGS.Apis.TgsApi.GameProviders.TCG.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Shared.Game.ApiClients.TCG;
using TGS.Shared.Game.Models.TCG.Common;
using TGS.Shared.Game.Models.TCG.Enums;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Middlewares
{
    public class DispatcherTCGMiddleware : IMiddleware
    {
        private static readonly IReadOnlyDictionary<string, string> _Routes = new Dictionary<string, string>
        {
            { "sgb", "/api/tcg/balance" },
            { "db", "/api/tcg/bet" },
            { "cr", "/api/tcg/settle" },
            { "rsv", "/api/tcg/reservemoney" },
            { "cnrsv", "/api/tcg/cancelreserve" },
            { "crsv", "/api/tcg/confirmreserve" },
            { "scs", "/api/tcg/checktransactionstatus" }
        };

        private static readonly RecyclableMemoryStreamManager _RecyclableMemoryStreamManager =
            new RecyclableMemoryStreamManager();

        private readonly IJsonSerializationProvider _json;
        private readonly JsonSerializerOptions _jsonSerializeOptions;
        private readonly ISettingOperations _settingOperations;
        private readonly ITCGUtility _utility;
        private readonly ITCGService _service;

        public DispatcherTCGMiddleware(
            IJsonSerializationProvider json,
            ISettingOperations settingOperations,
            ITCGUtility utility,
            ITCGService service)
        {
            _json = json ?? throw new ArgumentNullException(nameof(json));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _jsonSerializeOptions = new JsonSerializerOptions
            {
                WriteIndented = false,
                NumberHandling = JsonNumberHandling.Strict
            };
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var form = context.Request.Form;
            if (!form.TryGetValue("merchant_code", out var merchantCode) && string.IsNullOrEmpty(merchantCode))
            {
                throw new ModelBindingException(GameApiError.MissingRequiredParameter.Description());
            }

            if (!form.TryGetValue("params", out var paramsRequest) && string.IsNullOrEmpty(paramsRequest))
            {
                throw new ModelBindingException(GameApiError.MissingRequiredParameter.Description());
            }
            if (!form.TryGetValue("sign", out var sign) && string.IsNullOrEmpty(sign))
            {
                throw new ModelBindingException(GameApiError.MissingRequiredParameter.Description());
            }

            var decodeMerchantCode = HttpUtility.UrlDecode(merchantCode);
            var merchant = await _service.GetMerchantByMerchantCodeAsync(decodeMerchantCode).ConfigureAwait(false);
            if (merchant == null)
            {
                throw new ModelBindingException(GameApiError.MissingRequiredParameter.Description());
            }
            var decodeSign = HttpUtility.UrlDecode(sign);


            var hash256 = _utility.GetHash256(paramsRequest, merchant.SHA256Key);
            if (hash256.IgnoreCaseNotEquals(decodeSign))
            {
                throw new InvalidCredentialsException(GameApiError.InvalidSignature.Description());
            }

            var decrypt = string.Empty;
            try
            {
                decrypt = _utility.DESCDecrypt(paramsRequest, merchant.DESKey);
            }
            catch
            {
                throw new ArgumentException(GameApiError.InvalidParametersFailedToDecryptTheParameters.Description());
            }

            var request = _json.DeserializeFromString<TCGSingleWalletRequest>(decrypt);

            if (!_Routes.TryGetValue(request.Method, out var route))
            {
                await next(context);
                return;
            }

            var stream = _RecyclableMemoryStreamManager.GetStream(Encoding.UTF8.GetBytes(decrypt));
            context.Request.Body = stream;
            context.Request.ContentType = MediaTypeNames.Application.Json;
            context.Request.Path = route;

            await next(context);

        }

    }
}