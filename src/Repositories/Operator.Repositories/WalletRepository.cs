using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.Operator;
using BrandPayment = TGS.Services.Operator.BrandPayment;
using PaymentChannel = TGS.Services.Operator.PaymentChannel;
using PaymentMethod = TGS.Services.Operator.PaymentMethod;

namespace TGS.Operator.Repositories
{
    internal class WalletRepository : IWalletRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public WalletRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<Guid> CreateDepositFormAsync(BankDepositForm form)
        {
            var formId = Guid.NewGuid();

            _tgsRepository.DepositForms.Add(new DepositForm
            {
                Id = formId,
                PlayerCode = form.PlayerCode,
                BrandId = form.BrandId,
                BrandPaymentId = form.BrandPaymentId,
                BankAccount = form.BankAccount,
                Amount = form.Amount
            });

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);

            return formId;
        }

        public async ValueTask<BalanceInfo> GetBalanceInfoAsync(string playerCode)
        {
            var playerWallet = await _tgsRepository.Wallets.AsAsyncEnumerable()
                .Where(wallet => wallet.PlayerCode == playerCode)
                .SingleAsync()
                .ConfigureAwait(false);

            return new BalanceInfo(playerWallet.Balance);
        }

        public IAsyncEnumerable<PaymentChannel> GetAllPaymentChannelsAsync()
        {
            return _tgsRepository.PaymentChannels.AsAsyncEnumerable()
                .Select(row => new PaymentChannel(row.Id, row.Code));
        }

        public IAsyncEnumerable<PaymentMethod> GetAllPaymentMethodsAsync()
        {
            return _tgsRepository.PaymentMethods.AsAsyncEnumerable()
                .Select(row => new PaymentMethod(row.Id, row.Code));
        }

        public async ValueTask<BrandPayment> GetBrandPaymentByIdAsync(Guid id)
        {
            var row = await _tgsRepository.BrandPayments
                .Include(bp => bp.PaymentMethodChannel)
                .Where(bp => bp.Id == id)
                .SingleAsync()
                .ConfigureAwait(false);

            return new BrandPayment(
                row.Id,
                row.PaymentMethodChannelId,
                row.PaymentMethodChannel.PaymentId,
                row.PaymentMethodChannel.MethodId,
                row.PaymentMethodChannel.ChannelId,
                row.ChannelName,
                row.ExtendedData);
        }

        public async ValueTask<BankDepositForm> GetDepositFormByIdAsync(Guid id)
        {
            var data = await _tgsRepository.DepositForms.FindAsync(id).ConfigureAwait(false);

            return new BankDepositForm(data.PlayerCode, data.BrandId, data.BrandPaymentId, data.BankAccount, data.Amount);
        }

        public async ValueTask SubmitRemittanceReceiptToDepositFormAsync(Guid formId, string remittanceReceiptFileId)
        {
            var formData = await _tgsRepository.DepositForms.FindAsync(formId).ConfigureAwait(false);

            formData.Status = 1000;
            formData.ManualReceiptFileId = remittanceReceiptFileId;

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
