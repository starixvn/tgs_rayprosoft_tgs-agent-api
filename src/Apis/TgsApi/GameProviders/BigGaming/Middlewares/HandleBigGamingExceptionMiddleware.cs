using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Commons.Middlewares;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.BigGaming.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Log;
using TGS.Core.Models;
using TGS.Core.Models.Interfaces;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.BigGaming.Common;
using TGS.Shared.Game.Models.BigGaming.Enums;
using TGS.Shared.Game.Models.BigGaming.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Middlewares
{

    public sealed class HandleBigGamingExceptionMiddleware : BaseHandleGameProviderExceptionMiddleware
    {
        private readonly IGamePlayActions _action;
        private readonly IBigGamingModelActionMapper _map;

        private readonly IReadOnlyDictionary<string, Func<BGBaseRequest, Task<BGBaseResponse>>> _methodMapper;

        public HandleBigGamingExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log,
            IGamePlayActions action,
            IBigGamingModelActionMapper map)
            : base(mapper, Consts.GameProvider.BigGaming, log, "biggaming")
        {
            _action = action;
            _map = map;



            _methodMapper = new Dictionary<string, Func<BGBaseRequest, Task<BGBaseResponse>>>
            {
                {"open.operator.user.balance", GetResponseBalanceAsync},
                {"open.operator.order.transfer", GetResponseBetAsync},
                {"open.operator.calc.transfer",GetResponseSettleAsync},
                {"open.operator.order.status", GetResponseOrderStatusAsync},
                {"open.operator.ping" ,GetResponsePingAsync}
            };

        }

        protected override async Task CreateResponseAsync(HttpContext context, int statusCode, int errorCode,
            Exception exception)
        {
            var error = (GameApiError)errorCode;
            var message = ErrorMapper.GetMessageByException(exception);

            var requestType = context.Items.Get<Type>(Consts.ContextItems.RequestType, null);

            var value = await context.Request.ReadAsStringAsync();
            var baseRequest = JsonSerializer.Deserialize<BGBaseRequest>(value);

            var response = default(BGBaseResponse);
            if (!_methodMapper.TryGetValue(baseRequest.Method.ToLower(), out var execute))
            {
                response = new BGBaseResponse()
                {
                    Id = baseRequest.Id,
                    Error = new BGBaseErrorResponse()
                    {
                        Code = error.ToString(),
                        Message = GameApiError.Account_Does_Not_Exist.Description()
                    },
                    Jsonrpc = baseRequest.Jsonrpc
                };
            }
            else
            {
                response = await execute(baseRequest).ConfigureAwait(false);
            }

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response, response.GetType());
        }

        #region Private Method

        private Task<BGBaseResponse> GetResponseBalanceAsync(BGBaseRequest baseRequest)
        {
            return Task.FromResult((new BGBaseResponse()
            {
                Id = baseRequest.Id,
                Error = new BGBaseErrorResponse()
                {
                    Code = GameApiError.Account_Does_Not_Exist.ToString(),
                    Message = GameApiError.Account_Does_Not_Exist.Description()
                },
                Jsonrpc = baseRequest.Jsonrpc
            }));
        }

        private Task<BGBaseResponse> GetResponseBetAsync(BGBaseRequest baseRequest)
        {
            var data = JsonSerializer.Deserialize<BGBetRequest>(JsonSerializer.Serialize(baseRequest.Params));
            var result = new BGBetResponse()
            {
                UserId = data.UserId,
                Sn = data.Sn,
                AvailableAmount = 0,
                OrderResult = "0",
                TranId = data.TranId
            };

            return Task.FromResult((new BGBaseResponse()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(result)),
                Error = new BGBaseErrorResponse()
                {
                    Code = GameApiError.Handle_Exceptions_Submitted_By_Users_For_Batch_Betting.ToString(),
                    Sn = data.Sn,
                    Message = GameApiError.Handle_Exceptions_Submitted_By_Users_For_Batch_Betting.Description()
                },
                Jsonrpc = baseRequest.Jsonrpc
            }));
        }

        private Task<BGBaseResponse> GetResponseSettleAsync(BGBaseRequest baseRequest)
        {
            var data = JsonSerializer.Deserialize<BGSettleRequest>(JsonSerializer.Serialize(baseRequest.Params));
            var result = new BGSettleResponse()
            {
                UserId = data.UserId,
                Sn = data.Sn,
                AvailableAmount = 0,
                OrderResult = "0",
                TranId = data.TranId
            };

            return Task.FromResult((new BGBaseResponse()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(result)),
                Error = new BGBaseErrorResponse()
                {
                    Code = GameApiError.Exception_Of_Batch_Settlement_Submitted_By_Users.ToString(),
                    Sn = data.Sn,
                    Message = GameApiError.Exception_Of_Batch_Settlement_Submitted_By_Users.Description()
                },
                Jsonrpc = baseRequest.Jsonrpc
            }));
        }

        private Task<BGBaseResponse> GetResponseOrderStatusAsync(BGBaseRequest baseRequest)
        {
            var data = JsonSerializer.Deserialize<BGOrderStatusRequest>(JsonSerializer.Serialize(baseRequest.Params));
            var result = new BGOrderStatusResponse()
            {
                Result = null
            };

            return Task.FromResult((new BGBaseResponse()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(result)),
                Error = new BGBaseErrorResponse()
                {
                    Code = GameApiError.Bet_Slip_Does_Not_Exist.ToString(),
                    Sn = data.Sn,
                    Message = GameApiError.Bet_Slip_Does_Not_Exist.Description()
                },
                Jsonrpc = baseRequest.Jsonrpc
            }));
        }

        private Task<BGBaseResponse> GetResponsePingAsync(BGBaseRequest baseRequest)
        {
            return Task.FromResult((new BGBaseResponse()
            {
                Id = baseRequest.Id,
                Result = null,
                Error = new BGBaseErrorResponse()
                {
                    Code = GameApiError.Unknown_Exception_At_The_Application_Layer.ToString(),
                    Message = GameApiError.Unknown_Exception_At_The_Application_Layer.Description()
                },
                Jsonrpc = baseRequest.Jsonrpc
            }));
        }

        #endregion

    }

}
