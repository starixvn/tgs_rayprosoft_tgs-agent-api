using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Apis.BackendOffice.Controllers.Messages.BrandPayment;
using TGS.Core.DataServices;
using TGS.Core.DataServices.Models;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.DataService.Services
{
    public interface IBrandPaymentService
    {
        Task<BrandPayment> GetPaymentAsync(Expression<Func<BrandPayment, bool>> predicate,
            Func<IQueryable<BrandPayment>, IQueryable<BrandPayment>> preQuery = null, bool tracking = false);

        IQueryable<BrandPayment> GetPaymentsAsync(bool tracking = false);
        Task CreatePaymentAsync(CreateBrandPaymentRequest request, string username);
        Task UpdatePaymentAsync(BrandPayment payment, UpdateBrandPaymentRequest request, string username);
        Task ChangeStatusAsync(BrandPayment payment, SwitchStatus status, string username);
        Task<bool> ExistsAsync(Expression<Func<BrandPayment, bool>> predicate);
        Task UpdatePaymentHandlingFeeAsync(BrandPayment payment, UpdatePaymentFeeRequest request, string username);
        Task ChangeOrderAsync(ChangeOrderRequest request);
    }

    public class BrandPaymentService : IBrandPaymentService
    {
        private readonly ITgsRepository _repository;

        public BrandPaymentService(ITgsRepository repository)
        {
            _repository = repository;
        }

        async Task IBrandPaymentService.CreatePaymentAsync(CreateBrandPaymentRequest request, string username)
        {
            var paymentId = Guid.NewGuid();
            var payment = new BrandPayment
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

        async Task IBrandPaymentService.UpdatePaymentAsync(BrandPayment payment, UpdateBrandPaymentRequest request,
            string username)
        {
            var beforePayment = new BrandPayment
            {
                ChannelName = payment.ChannelName,
                PaymentStatus = payment.PaymentStatus,
                TransferLimitFrom = payment.TransferLimitFrom,
                TransferLimitTo = payment.TransferLimitTo,
                PlayerListCondition = payment.PlayerListCondition,
                AllPlayers = payment.AllPlayers,
                IdentityAuthentication = payment.IdentityAuthentication,
                PhoneAuthentication = payment.PhoneAuthentication,
                DepositedAmountUnlimited = payment.DepositedAmountUnlimited,
                DepositedAmountMin = payment.DepositedAmountMin,
                DepositedAmountMax = payment.DepositedAmountMax
            };

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

        Task<BrandPayment> IBrandPaymentService.GetPaymentAsync(Expression<Func<BrandPayment, bool>> predicate,
            Func<IQueryable<BrandPayment>, IQueryable<BrandPayment>> preQuery, bool tracking)
        {
            var payment = tracking ? _repository.Payments : _repository.Payments.AsNoTracking();
            preQuery ??= c => c;

            return preQuery(payment).FirstOrDefaultAsync(predicate);
        }

        IQueryable<BrandPayment> IBrandPaymentService.GetPaymentsAsync(bool tracking) =>
            tracking ? _repository.Payments : _repository.Payments.AsNoTracking();

        Task<bool> IBrandPaymentService.ExistsAsync(Expression<Func<BrandPayment, bool>> predicate) =>
            _repository.Payments.AnyAsync(predicate);

        Task IBrandPaymentService.UpdatePaymentHandlingFeeAsync(BrandPayment payment, UpdatePaymentFeeRequest request,
            string username)
        {
            payment.FeePercentage = request.FeePercentage;
            payment.LowestFee = request.LowestFee;
            payment.HighestFee = request.HighestFee;
            _repository.Entry(payment).State = EntityState.Modified;
            return _repository.SaveChangesAsync();
        }

        async Task IBrandPaymentService.ChangeOrderAsync(ChangeOrderRequest request)
        {
            var ids = request.Orders.Select(q => q.Id);
            var payments = await _repository.Payments.Where(q => ids.Contains(q.Id)).ToListAsync();
            payments.ForEach(p => p.DisplayOrder = request.Orders.FirstOrDefault(x => x.Id == p.Id).DisplayOrder);

            await _repository.BulkInsertOrUpdateAsync(payments.ToArray());
        }

        Task IBrandPaymentService.ChangeStatusAsync(BrandPayment payment, SwitchStatus status, string username)
        {
            var beforePayment =
                JsonConvert.DeserializeObject<BrandPayment>(JsonConvert.SerializeObject(payment,
                    new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }));

            payment.PaymentStatus = status;
            payment.UpdatedOn = DateTimeOffset.Now;
            payment.UpdatedByUsername = username;

            _repository.Entry(payment).State = EntityState.Modified;
            _repository.PaymentAuditLogs.AddAsync(GeneratePaymentAuditLog(payment, beforePayment));
            return _repository.SaveChangesAsync();
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

        private PaymentAuditLog GeneratePaymentAuditLog(BrandPayment payment, BrandPayment beforePayment) =>
            new()
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