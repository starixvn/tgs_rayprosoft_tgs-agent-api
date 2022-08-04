using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Tgs.Payment.Clients;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.BrandPayment.PaymentConditions;

namespace TGS.Services.BackOffice.Application.Services
{
    internal class BrandPaymentsQueryService : IQueryService<BrandPaymentsQuery, BrandPaymentInfo>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly IPlayerServiceClient _playerServiceClient;

        public BrandPaymentsQueryService(
            IPaymentServiceClient paymentServiceClient,
            IPlayerServiceClient playerServiceClient)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _playerServiceClient = playerServiceClient ?? throw new ArgumentNullException(nameof(playerServiceClient));
        }

        public async IAsyncEnumerable<BrandPaymentInfo> QueryAsync(
            BrandPaymentsQuery query,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var brandPayments = await _paymentServiceClient
            .GetBrandPaymentsAsync(query.BrandId)
            .ToArrayAsync()
            .ConfigureAwait(false)!;

            var acls = await _paymentServiceClient
                .GetBrandPaymentAclsAsync(query.PlayerCode, brandPayments.Select(bp => bp.Id))
                .ToArrayAsync()
                .ConfigureAwait(false);

            var blockList = acls
                .Where(acl => acl.IsBlock)
                .Select(acl => acl.BrandPaymentId)
                .Distinct()
                .ToArray();

            var whiteList = acls
                .Where(acl => !acl.IsBlock)
                .Select(acl => acl.BrandPaymentId)
                .Distinct()
                .ToArray();

            var brandPaymentVipLevels = await _paymentServiceClient
                .GetBrandPaymentVipsAsync(brandPayments.Select(bp => bp.Id))
                .ToLookupAsync(bv => bv.BrandPaymentId, bv => bv.VipLevelId)
                .ConfigureAwait(false);

            var playerInfo = await _playerServiceClient
                .GetPlayerInfoAsync(query.PlayerCode, cancellationToken)
                .ConfigureAwait(false)!;

            var depositedAmount = await _playerServiceClient
                .GetPlayerDepositedAmountAsync(query.PlayerCode, cancellationToken)
                .ConfigureAwait(false);

            var blockListCondition = new BlockListCondition(blockList);
            var whiteListCondition = new WhiteListCondition(whiteList);
            var showOnAllPlayerCondition = new ShowOnAllPlayersCondition();

            foreach (var bp in brandPayments)
            {
                if (!bp.Enabled)
                    continue;

                var depositedAmountCondition = new DepositedAmountCondition(bp.DepositedAmountLimit);
                var identityCondition = new IdentityCondition(bp.AuthenticateIdentity);
                var phoneCondition = new PhoneCondition(bp.AuthenticatePhone);
                var vipCondition = new VipCondition(brandPaymentVipLevels.Contains(bp.Id)
                        ? brandPaymentVipLevels[bp.Id]
                        : Array.Empty<Guid>());

                var filterCondition = new BrandPaymentFilterCondition(
                    blockListCondition,
                    whiteListCondition,
                    showOnAllPlayerCondition,
                    bp.ConditionMatchType == ConditionMatchType.MatchAll
                        ? new MatchAllCondition(
                            depositedAmountCondition,
                            identityCondition,
                            phoneCondition,
                            vipCondition)
                        : new MatchOneCondition(
                            depositedAmountCondition,
                            identityCondition,
                            phoneCondition,
                            vipCondition));

                if (filterCondition.IsValid(bp, playerInfo!, depositedAmount))
                    yield return bp;
            }
        }
    }
}
