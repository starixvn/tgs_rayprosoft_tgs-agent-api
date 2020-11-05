using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.DataService.Services
{
    public interface IPaymentService
    {
        Task CreatePaymentAsync(CreatePaymentRequest request, string username);
        Task UpdatePaymentAsync(Payment payment, UpdatePaymentRequest request, string username);
    }

    public class PaymentService : IPaymentService
    {
        private readonly ITgsRepository _repository;

        public PaymentService(ITgsRepository repository)
        {
            _repository = repository;
        }

        async Task IPaymentService.CreatePaymentAsync(CreatePaymentRequest request, string username)
        {
            var paymentId = Guid.NewGuid();
            var payment = new Payment
            {
                Id = paymentId,
                DisplayOrder = 99,
                PaymentVendorId = request.PaymentVendorId,
                PaymentMethodId = request.PaymentMethodId,
                PaymentChannelId = request.PaymentChannelId,
                ChannelName = request.ChannelName,
                TransferLimitFrom = request.TransferLimitFrom,
                TransferLimitTo = request.TransferLimitTo,
                PlayerListCondition = request.PlayerListCondition,
                AllPlayer = request.AllPlayer,
                IdentityVerification = request.IdentityVerification,
                PhoneVerification = request.PhoneVerification,
                DepositedAmountUnlimited = request.DepositedAmountUnlimited,
                DepositedAmountFrom = request.DepositedAmountFrom,
                DepositedAmountTo = request.DepositedAmountTo,
                Reason = request.Reason,
                CreatedByUsername = username,
                UpdatedByUsername = username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            var whitelist = await GeneratePaymentPlayerList(paymentId, request.Whitelist);

            var blacklist = await GeneratePaymentPlayerList(paymentId, request.Blacklist);

            var playerList = whitelist.Union(blacklist);

            payment.PaymentPlayerList = new HashSet<PaymentPlayerList>(playerList);

            var vips = await _repository.VipLevels.Where(x => request.VipLevelIds.Contains(x.Id))
                .ToArrayAsync();
            var vipLevels = vips.Select(vip => new PaymentVipLevel
            {
                PaymentId = paymentId,
                VipLevelId = vip.Id,
                VipLevel = vip
            }).ToArray();

            payment.VipLevels = new HashSet<PaymentVipLevel>(vipLevels);

            await _repository.Payments.AddAsync(payment);
            await _repository.SaveChangesAsync();
        }

        async Task IPaymentService.UpdatePaymentAsync(Payment payment, UpdatePaymentRequest request, string username)
        {
            payment.ChannelName = request.ChannelName;
            payment.TransferLimitFrom = request.TransferLimitFrom;
            payment.TransferLimitTo = request.TransferLimitTo;
            payment.PlayerListCondition = request.PlayerListCondition;
            payment.AllPlayer = request.AllPlayer;
            payment.IdentityVerification = request.IdentityVerification;
            payment.PhoneVerification = request.PhoneVerification;
            payment.DepositedAmountUnlimited = request.DepositedAmountUnlimited;
            payment.DepositedAmountFrom = request.DepositedAmountFrom;
            payment.DepositedAmountTo = request.DepositedAmountTo;
            payment.Reason = request.Reason;
            payment.UpdatedByUsername = username;
            payment.UpdatedOn = DateTimeOffset.Now;

            await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            _repository.PaymentPlayerList.RemoveRange(payment.PaymentPlayerList);
            _repository.PaymentVipLevels.RemoveRange(payment.VipLevels);
            await _repository.SaveChangesAsync();

            var whitelist = await GeneratePaymentPlayerList(payment.Id, request.Whitelist);
            var blacklist = await GeneratePaymentPlayerList(payment.Id, request.Blacklist);
            var playerList = whitelist.Union(blacklist).ToArray();
            payment.PaymentPlayerList = new HashSet<PaymentPlayerList>(playerList);

            var vips = await _repository.VipLevels.Where(x => request.VipLevelIds.Contains(x.Id)).ToArrayAsync();
            var vipLevels = vips.Select(vip => new PaymentVipLevel
            {
                PaymentId = payment.Id,
                VipLevelId = vip.Id,
                VipLevel = vip
            }).ToArray();
            payment.VipLevels = new HashSet<PaymentVipLevel>(vipLevels);

            _repository.Entry(payment).State = EntityState.Modified;
            await _repository.SaveChangesAsync();

            await tran.CommitAsync();
        }

        private async Task<PaymentPlayerList[]> GeneratePaymentPlayerList(Guid paymentId, string[] list)
        {
            var playerWhitelist = list != null
                ? await _repository.Players.Where(x => list.Contains(x.PlayerCode))
                    .ToArrayAsync()
                : Array.Empty<Player>();
            return playerWhitelist.Select(player => new PaymentPlayerList
            {
                ListType = ListType.White,
                PaymentId = paymentId,
                PlayerCode = player.PlayerCode,
                Player = player
            }).ToArray();
        }
    }
}