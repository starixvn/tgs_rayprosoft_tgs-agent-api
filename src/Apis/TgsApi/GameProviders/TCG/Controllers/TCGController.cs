using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.Extensions;
using TGS.Apis.TgsApi.GameProviders.TCG.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Services.GameOperations;
using TGS.Shared.Game.Models.TCG.Common;
using TGS.Shared.Game.Models.TCG.Enums;
using TGS.Shared.Game.Models.TCG.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Controllers
{
    [Route("api/tcg")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.TCG)]
    public class TCGController : ControllerBase
    {
        private readonly IGamePlayActions _actions;
        private readonly ITCGService _service;
        private readonly ITCGActionModelMapper _mapper;

        public TCGController(
            IGamePlayActions actions,
            ITCGService service,
            ITCGActionModelMapper mapper)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost("entrance")]
        public ValueTask<TCGSingleWalletResponse> Entrance([FromForm] TCGSingleWalletRequest request)
            => ValueTask.FromResult(new TCGSingleWalletResponse
            {
                Status = (int)GameApiError.MethodNotFound,
                ErrorDesc = GameApiError.MethodNotFound.Description()
            });

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("balance")]
        public async ValueTask<TCGBalanceResponse> GetBalanceAsync(TCGBalanceRequest request)
        {
            var getBalance = await _actions.GetBalanceAsync(Consts.GameProvider.TCG, request.username).ConfigureAwait(false);

            return new TCGBalanceResponse()
            {
                Status = (int)GameApiError.Success,
                ErrorDesc = GameApiError.Success.Description(),
                Balance = getBalance.Balance
            };
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("bet")]
        public async ValueTask<TCGSingleWalletTradeResponse> BetAsync(TCGDebitRequest request)
        {
            var betDatas = await _mapper.BetAsync(request).ConfigureAwait(false);
            if (betDatas == null)
            {
                return new TCGSingleWalletTradeResponse()
                {
                    Status = (int)GameApiError.UnknownSystemErrorPleaseContactTCGCustomerSupport,
                    ErrorDesc = GameApiError.UnknownSystemErrorPleaseContactTCGCustomerSupport.Description(),
                };
            }
            var balanceInfos = new List<BalanceInfo>();
            foreach (var betData in betDatas)
            {
                var betResult = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);
                balanceInfos.Add(new BalanceInfo()
                {
                    Usernam = betData.PlayerCode,
                    Balance = betResult.Balance
                });
            }

            return new TCGSingleWalletTradeResponse()
            {
                Status = (int)GameApiError.Success,
                ErrorDesc = GameApiError.Success.Description(),
                BalanceInfos = balanceInfos.ToArray()
            };
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("settle")]
        public async ValueTask<TCGSingleWalletTradeResponse> SettleAsync(TCGCreditRequest request)
        {
            var betData = await _mapper.SettleAsync(request).ConfigureAwait(false);
            var betResult = await _actions.SettleBatchAsync(betData).ConfigureAwait(false);

            var balanceInfos = (from PlayerBalance in betResult.PlayerBalances
                                select new BalanceInfo
                                {
                                    Usernam = PlayerBalance.Key,
                                    Balance = PlayerBalance.Value
                                }).ToArray();
            return new TCGSingleWalletTradeResponse()
            {
                Status = (int)GameApiError.Success,
                ErrorDesc = GameApiError.Success.Description(),
                BalanceInfos = balanceInfos
            };
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("reservemoney")]
        public async ValueTask<TCGSingleWalletTradeResponse> ReserveMoneyAsync(TCGReserveMoneyRequest request)
        {
            var betDatas = await _mapper.ReserveMoneyAsync(request).ConfigureAwait(false);
            if (betDatas == null)
            {
                return new TCGSingleWalletTradeResponse()
                {
                    Status = (int)GameApiError.UnknownSystemErrorPleaseContactTCGCustomerSupport,
                    ErrorDesc = GameApiError.UnknownSystemErrorPleaseContactTCGCustomerSupport.Description(),
                };
            }
            var balanceInfos = new List<BalanceInfo>();
            foreach (var betData in betDatas)
            {
                var betResult = await _actions.PlaceBetAsync(betData).ConfigureAwait(false);
                balanceInfos.Add(new BalanceInfo()
                {
                    Usernam = betData.PlayerCode,
                    Balance = betResult.Balance
                });
            }

            return new TCGSingleWalletTradeResponse()
            {
                Status = (int)GameApiError.Success,
                ErrorDesc = GameApiError.Success.Description(),
                BalanceInfos = balanceInfos.ToArray()
            };
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("cancelreserve")]
        public async ValueTask<TCGSingleWalletTradeResponse> CancelReserveAsync(TCGCancelReserveRequest request)
        {
            var betData = await _mapper.CancelReserveAsync(request).ConfigureAwait(false);
            var betResult = await _actions.CancelBatchAsync(betData).ConfigureAwait(false);

            var balanceInfos = (from PlayerBalance in betResult.PlayerBalances
                                select new BalanceInfo
                                {
                                    Usernam = PlayerBalance.Key,
                                    Balance = PlayerBalance.Value
                                }).ToArray();
            return new TCGSingleWalletTradeResponse()
            {
                Status = (int)GameApiError.Success,
                ErrorDesc = GameApiError.Success.Description(),
            };
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("confirmreserve")]
        public async ValueTask<TCGSingleWalletTradeResponse> ConfirmReserveAsync(TCGConfirmReserveRequest request)
        {
            return new TCGSingleWalletTradeResponse()
            {
                Status = (int)GameApiError.Success,
                ErrorDesc = GameApiError.Success.Description(),
            };
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost("checktransactionstatus")]
        public async ValueTask<TCGCheckTransactionStatusResponse> CheckTransactionStatusAsync(TCGCheckTransactionStatusRequest request)
        {

            return new TCGCheckTransactionStatusResponse()
            {
                Status = (int)GameApiError.Success,
                ErrorDesc = GameApiError.Success.Description(),
            };
        }
    }
}

