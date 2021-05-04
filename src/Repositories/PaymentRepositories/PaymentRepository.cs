using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Payment;

namespace TGS.Payment.Repositories
{
    internal class PaymentRepository : IPaymentRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public PaymentRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask CreateBankInfoAsync(BankInfo bankInfo, string modifyUser)
        {
            var createTime = DateTimeOffset.UtcNow;

            await _tgsRepository.PlayerBankInfos
                .AddAsync(new PlayerBankInfo
                {
                    Id = bankInfo.Id,
                    PlayerCode = bankInfo.PlayerCode,
                    BrandId = bankInfo.BrandId,
                    PaymentChannelId = bankInfo.PaymentChannelId,
                    BankType = bankInfo.BankType,
                    Detail = bankInfo.Detail,
                    CreatedByUsername = modifyUser,
                    CreatedOn = createTime,
                    UpdatedByUsername = modifyUser,
                    UpdatedOn = createTime
                })
                .ConfigureAwait(false);

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask CreateDepositFormAsync(Guid createFormId, DepositFormInfo form)
        {
            var currentDate = DateTimeOffset.UtcNow;

            await _tgsRepository.DepositForms
                .AddAsync(new DepositForm
                {
                    Id = createFormId,
                    PlayerCode = form.PlayerCode,
                    BrandId = form.BrandId,
                    BrandPaymentId = form.BrandPaymentId,
                    Amount = form.Amount,
                    ApplyInfo = form.ApplyInfo,
                    PaymentReturn = form.PaymentReturn,
                    CreatedOn = currentDate,
                    UpdatedOn = currentDate
                })
                .ConfigureAwait(false);

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask CreateWithdrawFormAsync(Guid createFormId, WithdrawFormCreateInfo form)
        {
            var currentDate = DateTimeOffset.UtcNow;

            await _tgsRepository.WithdrawForms
                .AddAsync(new WithdrawForm
                {
                    Id = createFormId,
                    BrandId = form.BrandId,
                    PlayerCode = form.PlayerCode,
                    BrandPaymentId = form.BrandPaymentId,
                    Amount = form.Amount,
                    BankInfoId = form.BankInfoId,
                    BankInfo = form.BankInfo,
                    ApplyInfo = form.ApplyInfo,
                    PaymentReturn = form.PaymentReturn,
                    CreatedOn = currentDate,
                    UpdatedOn = currentDate
                })
                .ConfigureAwait(false);

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask DeleteBankInfoAsync(string playerCode, Guid bankInfoId)
        {
            var bankInfo = await _tgsRepository.PlayerBankInfos
                .FindAsync(bankInfoId)
                .ConfigureAwait(false);

            if (bankInfo.PlayerCode == playerCode)
            {
                _tgsRepository.PlayerBankInfos.Remove(bankInfo);

                await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public IAsyncEnumerable<PaymentChannelInfo> GetAllPaymentChannelsAsync()
            => _tgsRepository.PaymentChannels.AsAsyncEnumerable()
                .Select(row => new PaymentChannelInfo(row.Id, row.Code));

        public IAsyncEnumerable<PaymentMethodInfo> GetAllPaymentMethodsAsync()
            => _tgsRepository.PaymentMethods.AsAsyncEnumerable()
                .Select(row => new PaymentMethodInfo(row.Id, row.Code, row.BankType));

        public async ValueTask<BankInfo> GetBankInfoByIdAsync(Guid id)
        {
            var bankInfo = await _tgsRepository.PlayerBankInfos
                .FindAsync(id)
                .ConfigureAwait(false);

            return new BankInfo(
                bankInfo.Id,
                bankInfo.PlayerCode,
                bankInfo.BrandId,
                bankInfo.PaymentChannelId,
                bankInfo.BankType,
                bankInfo.Detail);
        }

        public IAsyncEnumerable<BankInfo> GetBankInfosByPlayerCodeAsync(string playerCode)
            => _tgsRepository.PlayerBankInfos.AsAsyncEnumerable()
                .Where(info => info.PlayerCode == playerCode)
                .Select(info => new BankInfo(
                    info.Id,
                    info.PlayerCode,
                    info.BrandId,
                    info.PaymentChannelId,
                    info.BankType,
                    info.Detail));

        public IAsyncEnumerable<BrandPaymentAcl> GetBrandPaymentAclsAsync(IEnumerable<Guid> brandPaymentIds, string playerCode)
        {
            var ids = brandPaymentIds.ToArray();

            var query = from acl in _tgsRepository.BrandPaymentPlayers.AsAsyncEnumerable()
                        where acl.PlayerCode == playerCode
                            && ids.Contains(acl.BrandPaymentId)
                        select acl;

            return query.Select(acl => new BrandPaymentAcl(
                acl.BrandPaymentId,
                acl.PlayerCode,
                acl.ListType == PlayerListType.Black));
        }

        public async ValueTask<BrandPaymentInfo> GetBrandPaymentByIdAsync(Guid id)
        {
            var row = await _tgsRepository.BrandPayments
                .Include(bp => bp.PaymentMethodChannel)
                .Where(bp => bp.Id == id)
                .SingleAsync()
                .ConfigureAwait(false);

            return new BrandPaymentInfo(
                row.Id,
                row.PaymentMethodChannelId,
                row.PaymentMethodChannel.PaymentId,
                row.PaymentMethodChannel.MethodId,
                row.PaymentMethodChannel.ChannelId,
                row.ChannelName,
                row.ExtendedData,
                new ValueRange<decimal>
                {
                    From = row.TransferLimitFrom,
                    To = row.TransferLimitTo
                },
                row.AllPlayers,
                row.IdentityAuthentication,
                row.PhoneAuthentication,
                new ValueRange<decimal>
                {
                    From = row.DepositedAmountMin,
                    To = row.DepositedAmountUnlimited ? decimal.MaxValue : row.DepositedAmountMax
                },
                row.PlayerListCondition == PlayerListCondition.MatchOne ? ConditionMatchTypes.MatchOne : ConditionMatchTypes.MatchAll,
                row.TransferType switch
                {
                    PaymentTransferType.Withdrawal => TransferTypes.Withdraw,
                    _ => TransferTypes.Deposit,
                });
        }

        public IAsyncEnumerable<BrandPaymentInfo> GetBrandPaymentsAsync(Guid brandId)
        {
            var query = from bp in _tgsRepository.BrandPayments
                            .Include(bp => bp.PaymentMethodChannel)
                            .AsAsyncEnumerable()
                        where bp.BrandId == brandId
                        select bp;

            return query.Select(bp => new BrandPaymentInfo(
                bp.Id,
                bp.PaymentMethodChannelId,
                bp.PaymentMethodChannel.PaymentId,
                bp.PaymentMethodChannel.MethodId,
                bp.PaymentMethodChannel.ChannelId,
                bp.ChannelName,
                bp.ExtendedData,
                new ValueRange<decimal>
                {
                    From = bp.TransferLimitFrom,
                    To = bp.TransferLimitTo
                },
                bp.AllPlayers,
                bp.IdentityAuthentication,
                bp.PhoneAuthentication,
                new ValueRange<decimal>
                {
                    From = bp.DepositedAmountMin,
                    To = bp.DepositedAmountUnlimited ? decimal.MaxValue : bp.DepositedAmountMax
                },
                bp.PlayerListCondition == PlayerListCondition.MatchOne ? ConditionMatchTypes.MatchOne : ConditionMatchTypes.MatchAll,
                bp.TransferType switch
                {
                    PaymentTransferType.Withdrawal => TransferTypes.Withdraw,
                    _ => TransferTypes.Deposit
                }));
        }

        public IAsyncEnumerable<BrandPaymentVip> GetBrandPaymentVipListAsync(IEnumerable<Guid> brandPaymentIds)
        {
            var ids = brandPaymentIds.ToArray();

            return _tgsRepository.BrandPaymentVipLevels.AsAsyncEnumerable()
                .Where(v => ids.Contains(v.BrandPaymentId))
                .Select(v => new BrandPaymentVip(v.BrandPaymentId, v.VipLevelId));
        }

        public async ValueTask<DepositFormInfo> GetDepositFormByIdAsync(Guid id)
        {
            var data = await _tgsRepository.DepositForms.FindAsync(id).ConfigureAwait(false);

            return new DepositFormInfo(
                data.PlayerCode,
                data.BrandId,
                data.BrandPaymentId,
                data.Amount,
                data.ApplyInfo,
                data.PaymentReturn);
        }

        public async ValueTask<PaymentInfo> GetPaymentAsync(Guid id)
        {
            var payment = await _tgsRepository.Payments
                .FindAsync(id)
                .ConfigureAwait(false);

            return new PaymentInfo(
                payment.Id,
                payment.Code);
        }

        public IAsyncEnumerable<PaymentMethodChannelInfo> GetPaymentMethodChannelsAsync()
        {
            return _tgsRepository.PaymentMethodChannels.ToAsyncEnumerable()
                .Select(pmc => new PaymentMethodChannelInfo(pmc.Id, pmc.PaymentId, pmc.MethodId, pmc.ChannelId));
        }

        public ValueTask<WithdrawFormInfo> GetWithdrawFormByIdAsync(Guid formId)
        {
            var query = from df in _tgsRepository.WithdrawForms
                            .Include(f => f.Player)
                            .Include(f => f.BrandPayment)
                            .ThenInclude(bp => bp.PaymentMethodChannel)
                            .AsAsyncEnumerable()
                        where df.Id == formId
                        select new WithdrawFormInfo(
                            df.Id,
                            df.PlayerCode,
                            df.Player.Username,
                            df.BrandId,
                            df.Amount,
                            df.Status,
                            df.Remark,
                            df.BankInfoId,
                            df.CreatedOn,
                            df.UpdatedOn,
                            df.BrandPaymentId,
                            df.BrandPayment?.ChannelName,
                            df.BrandPayment?.PaymentMethodChannel?.MethodId,
                            df.BankInfo,
                            df.ApplyInfo,
                            df.PaymentReturn);

            return query.SingleAsync();
        }

        public async ValueTask SignOffWithdrawFormAsync(Guid formId, ReviewStatus reviewStatus, string? remark)
        {
            var formData = await _tgsRepository.WithdrawForms
                .FindAsync(formId)
                .ConfigureAwait(false);

            formData.Status = reviewStatus switch
            {
                ReviewStatus.Approve => 8000,
                ReviewStatus.Reject => 7000,
                _ => formData.Status
            };
            formData.Remark = remark;
            formData.UpdatedOn = DateTimeOffset.UtcNow;

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask SubmitFormStatusAsync(Guid formId, int status, string paymentReturn)
        {
            var formData = await _tgsRepository.DepositForms.FindAsync(formId).ConfigureAwait(false);

            formData.Status = status;
            formData.PaymentReturn = paymentReturn;

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
