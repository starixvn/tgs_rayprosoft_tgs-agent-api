using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TGS.Apis.BackendOffice.Controllers.Messages.Payment;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.DataService.Services
{
    public interface IPaymentService
    {
        Task<Payment> GetPaymentAsync(Expression<Func<Payment, bool>> predicate,
            Func<IQueryable<Payment>, IQueryable<Payment>> preQuery = null, bool tracking = false);

        IQueryable<Payment> GetPaymentsAsync(bool tracking = false);
        Task CreatePaymentAsync(CreatePaymentRequest request, string username);
        Task UpdatePaymentAsync(Payment payment, UpdatePaymentRequest request, string username);
        Task ChangeStatusAsync(Payment payment, PaymentStatus status, string username);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IMapper _mapper;
        private readonly ITgsRepository _repository;

        public PaymentService(ITgsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
                AllPlayers = request.AllPlayers,
                IdentityAuthentication = request.IdentityAuthentication,
                PhoneAuthentication = request.PhoneAuthentication,
                DepositedAmountUnlimited = request.DepositedAmountUnlimited,
                DepositedAmountMin = request.DepositedAmountMin,
                DepositedAmountMax = request.DepositedAmountMax,
                Reason = request.Reason,
                CreatedByUsername = username,
                UpdatedByUsername = username,
                CreatedOn = DateTimeOffset.Now,
                UpdatedOn = DateTimeOffset.Now
            };

            var whitelist = await GeneratePaymentPlayerList(paymentId, request.Whitelist, PlayerListType.White);

            var blacklist = await GeneratePaymentPlayerList(paymentId, request.Blacklist, PlayerListType.Black);

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
            var beforePayment = _mapper.Map<Payment>(payment);

            payment.ChannelName = request.ChannelName;
            payment.TransferLimitFrom = request.TransferLimitFrom;
            payment.TransferLimitTo = request.TransferLimitTo;
            payment.PlayerListCondition = request.PlayerListCondition;
            payment.AllPlayers = request.AllPlayers;
            payment.IdentityAuthentication = request.IdentityAuthentication;
            payment.PhoneAuthentication = request.PhoneAuthentication;
            payment.DepositedAmountUnlimited = request.DepositedAmountUnlimited;
            payment.DepositedAmountMin = request.DepositedAmountMin;
            payment.DepositedAmountMax = request.DepositedAmountMax;
            payment.Reason = request.Reason;
            payment.UpdatedByUsername = username;
            payment.UpdatedOn = DateTimeOffset.Now;

            await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            _repository.PaymentPlayerList.RemoveRange(payment.PaymentPlayerList);
            _repository.PaymentVipLevels.RemoveRange(payment.VipLevels);

            await _repository.SaveChangesAsync();

            var whitelist = await GeneratePaymentPlayerList(payment.Id, request.Whitelist, PlayerListType.White);
            var blacklist = await GeneratePaymentPlayerList(payment.Id, request.Blacklist, PlayerListType.Black);
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

            await _repository.PaymentAuditLogs.AddAsync(GeneratePaymentAuditLog(payment, beforePayment));

            await _repository.SaveChangesAsync();

            await tran.CommitAsync();
        }

        async Task IPaymentService.ChangeStatusAsync(Payment payment, PaymentStatus status, string username)
        {
            var beforePayment =
                JsonConvert.DeserializeObject<Payment>(JsonConvert.SerializeObject(payment, new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));

            payment.PaymentStatus = status;
            payment.UpdatedOn = DateTimeOffset.Now;
            payment.UpdatedByUsername = username;

            _repository.Entry(payment).State = EntityState.Modified;
            await _repository.PaymentAuditLogs.AddAsync(GeneratePaymentAuditLog(payment, beforePayment));
            await _repository.SaveChangesAsync();
        }

        Task<Payment> IPaymentService.GetPaymentAsync(Expression<Func<Payment, bool>> predicate,
            Func<IQueryable<Payment>, IQueryable<Payment>> preQuery, bool tracking)
        {
            var payment = tracking ? _repository.Payments : _repository.Payments.AsNoTracking();
            preQuery ??= c => c;

            return preQuery(payment).FirstOrDefaultAsync(predicate);
        }

        IQueryable<Payment> IPaymentService.GetPaymentsAsync(bool tracking)
        {
            return tracking ? _repository.Payments : _repository.Payments.AsNoTracking();
        }

        private async Task<PaymentPlayerList[]> GeneratePaymentPlayerList(Guid paymentId, string[] list,
            PlayerListType playerListType)
        {
            var playerWhitelist = list != null
                ? await _repository.Players.Where(x => list.Contains(x.Username))
                    .ToArrayAsync()
                : Array.Empty<Player>();
            return playerWhitelist.Select(player => new PaymentPlayerList
            {
                ListType = playerListType,
                PaymentId = paymentId,
                PlayerCode = player.PlayerCode,
                Player = player
            }).ToArray();
        }

        private PaymentAuditLog GeneratePaymentAuditLog(Payment payment, Payment beforePayment)
        {
            return new PaymentAuditLog
            {
                Id = Guid.NewGuid(),
                PaymentId = payment.Id,
                BeforeChannelName = beforePayment.ChannelName,
                AfterChannelName = payment.ChannelName,
                BeforePaymentStatus = beforePayment.PaymentStatus,
                AfterPaymentStatus = payment.PaymentStatus,
                BeforeTransferLimitFrom = beforePayment.TransferLimitFrom,
                AfterTransferLimitFrom = payment.TransferLimitFrom,
                BeforeTransferLimitTo = beforePayment.TransferLimitTo,
                AfterTransferLimitTo = payment.TransferLimitTo,
                BeforePlayerListCondition = beforePayment.PlayerListCondition,
                AfterPlayerListCondition = payment.PlayerListCondition,
                BeforeAllPlayers = beforePayment.AllPlayers,
                AfterAllPlayers = payment.AllPlayers,
                BeforeIdentityAuthentication = beforePayment.IdentityAuthentication,
                AfterIdentityAuthentication = payment.IdentityAuthentication,
                BeforePhoneAuthentication = beforePayment.PhoneAuthentication,
                AfterPhoneAuthentication = payment.PhoneAuthentication,
                BeforeDepositedAmountUnlimited = beforePayment.DepositedAmountUnlimited,
                AfterDepositedAmountUnlimited = payment.DepositedAmountUnlimited,
                BeforeDepositedAmountMin = beforePayment.DepositedAmountMin,
                AfterDepositedAmountMin = payment.DepositedAmountMin,
                BeforeDepositedAmountMax = beforePayment.DepositedAmountMax,
                AfterDepositedAmountMax = payment.DepositedAmountMax,
                Reason = payment.Reason,
                CreatedOn = payment.UpdatedOn,
                CreatedByUsername = payment.UpdatedByUsername,
                Operation = AuditLogOperation.Update
            };
        }
    }
}