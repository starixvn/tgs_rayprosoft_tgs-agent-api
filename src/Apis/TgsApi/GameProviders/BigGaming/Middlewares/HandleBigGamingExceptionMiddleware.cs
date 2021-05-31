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

        private readonly IReadOnlyDictionary<string, Func<BigGamingRequest, GameApiError, string, Task<BigGamingResponse>>> _methodMapper;

        public HandleBigGamingExceptionMiddleware(
            Func<string, IErrorMapper> mapper,
            ITgsLog log,
            IGamePlayActions action,
            IBigGamingModelActionMapper map)
            : base(mapper, Consts.GameProvider.BigGaming, log, "biggaming")
        {
            _action = action;
            _map = map;
            _methodMapper = new Dictionary<string, Func<BigGamingRequest, GameApiError, string, Task<BigGamingResponse>>>
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

            var value = await context.Request.ReadAsStringAsync();
            var baseRequest = JsonSerializer.Deserialize<BigGamingRequest>(value);

            var response = await GetResponseAsync(baseRequest, error, message).ConfigureAwait(false);

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response, response.GetType());
        }

        #region Private Method

        private async Task<BigGamingResponse> GetResponseAsync(BigGamingRequest baseRequest, GameApiError error, string message)
        {
            var response = default(BigGamingResponse);
            if (error is
                GameApiError.UnknownException or
                GameApiError.AuthenticationSignIsInvalid or
                GameApiError.AccountDoesNotExist or
                GameApiError.InsufficientBettingBalance)
            {
                response = new BigGamingResponse()
                {
                    Id = baseRequest.Id,
                    Error = new BigGamingErrorResponse()
                    {
                        Code = error.ToString(),
                        Message = error.Description(),
                        Reason = message,
                        Action = baseRequest.Method
                    },
                    Jsonrpc = baseRequest.Jsonrpc
                };
            }
            else
            {
                if (!_methodMapper.TryGetValue(baseRequest.Method.ToLower(), out var execute))
                {
                    response = new BigGamingResponse()
                    {
                        Id = baseRequest.Id,
                        Error = new BigGamingErrorResponse()
                        {
                            Code = error.ToString(),
                            Message = GameApiError.AccountDoesNotExist.Description(),
                            Action = baseRequest.Method
                        },
                        Jsonrpc = baseRequest.Jsonrpc
                    };
                }
                else
                {
                    response = await execute(baseRequest, error, message).ConfigureAwait(false);
                }
            }
            return response;
        }

        private Task<BigGamingResponse> GetResponseBalanceAsync(BigGamingRequest baseRequest, GameApiError error, string message)
        {
            return Task.FromResult((new BigGamingResponse()
            {
                Id = baseRequest.Id,
                Error = new BigGamingErrorResponse()
                {
                    Code = GameApiError.AccountDoesNotExist.ToString(),
                    Message = GameApiError.AccountDoesNotExist.Description()
                },
                Jsonrpc = baseRequest.Jsonrpc
            }));
        }

        private Task<BigGamingResponse> GetResponseBetAsync(BigGamingRequest baseRequest, GameApiError error, string message)
        {
            var data = JsonSerializer.Deserialize<BigGamingBetRequest>(JsonSerializer.Serialize(baseRequest.Params));
            var result = new BigGamingBetResponse()
            {
                UserId = data.UserId,
                Sn = data.Sn,
                AvailableAmount = 0,
                OrderResult = "0",
                TranId = data.TranId
            };
            var errorResponse = new BigGamingErrorResponse()
            {
                Code = GameApiError.HandleExceptionsSubmittedByUsersForBatchBetting.ToString(),
                Sn = data.Sn,
                Message = GameApiError.HandleExceptionsSubmittedByUsersForBatchBetting.Description()
            };
            if (error == GameApiError.AbnormalFlowOfBulkFundsIntoTheWarehouse)
            {
                errorResponse.Code = GameApiError.BettingIsInconsistentWithTheNumberOfBettingSlips.ToString();
                errorResponse.Message = GameApiError.BettingIsInconsistentWithTheNumberOfBettingSlips.Description();
            }

            return Task.FromResult((new BigGamingResponse()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(result)),
                Error = errorResponse,
                Jsonrpc = baseRequest.Jsonrpc
            }));
        }

        private Task<BigGamingResponse> GetResponseSettleAsync(BigGamingRequest baseRequest, GameApiError error, string message)
        {
            var data = JsonSerializer.Deserialize<BigGamingSettleRequest>(JsonSerializer.Serialize(baseRequest.Params));
            var result = new BigGamingSettleResponse()
            {
                UserId = data.UserId,
                Sn = data.Sn,
                AvailableAmount = 0,
                OrderResult = "0",
                TranId = data.TranId
            };
            var errorResponse = new BigGamingErrorResponse()
            {
                Code = GameApiError.ExceptionOfBatchSettlementSubmittedByUsers.ToString(),
                Sn = data.Sn,
                Message = GameApiError.ExceptionOfBatchSettlementSubmittedByUsers.Description()
            };

            if (error == GameApiError.AbnormalFlowOfBulkFundsIntoTheWarehouse)
            {
                errorResponse.Code = GameApiError.SettlementIsInconsistentWithTheNumberOfWarehoused.ToString();
                errorResponse.Message = GameApiError.SettlementIsInconsistentWithTheNumberOfWarehoused.Description();
            }

            return Task.FromResult((new BigGamingResponse()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(result)),
                Error = errorResponse,
                Jsonrpc = baseRequest.Jsonrpc
            }));
        }

        private Task<BigGamingResponse> GetResponseOrderStatusAsync(BigGamingRequest baseRequest, GameApiError error, string message)
        {
            var data = JsonSerializer.Deserialize<BigGamingOrderStatusRequest>(JsonSerializer.Serialize(baseRequest.Params));
            var result = new BigGamingOrderStatusResponse()
            {
                Result = null
            };

            return Task.FromResult((new BigGamingResponse()
            {
                Id = baseRequest.Id,
                Result = JsonDocument.Parse(JsonSerializer.Serialize(result)),
                Error = new BigGamingErrorResponse()
                {
                    Code = GameApiError.BetSlipDoesNotExist.ToString(),
                    Sn = data.Sn,
                    Message = GameApiError.BetSlipDoesNotExist.Description()
                },
                Jsonrpc = baseRequest.Jsonrpc
            }));
        }

        private Task<BigGamingResponse> GetResponsePingAsync(BigGamingRequest baseRequest, GameApiError error, string message)
        {
            return Task.FromResult((new BigGamingResponse()
            {
                Id = baseRequest.Id,
                Result = null,
                Error = new BigGamingErrorResponse()
                {
                    Code = GameApiError.UnknownExceptionAtTheApplicationLayer.ToString(),
                    Message = GameApiError.UnknownExceptionAtTheApplicationLayer.Description()
                },
                Jsonrpc = baseRequest.Jsonrpc
            }));
        }

        #endregion

    }

}
