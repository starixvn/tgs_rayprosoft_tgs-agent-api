using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Deposit
{
    internal class BrandPaymentsQueryService : IQueryService<BrandPaymentsQuery, BrandPaymentInfo>
    {
        private readonly IWalletRepository _walletRepository;

        public BrandPaymentsQueryService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async IAsyncEnumerable<BrandPaymentInfo> QueryAsync(
            BrandPaymentsQuery query,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var brandPayments = await _walletRepository
                .GetBrandPaymentsAsync(query.BrandId)
                .ToArrayAsync()
                .ConfigureAwait(false);
            var acls = await _walletRepository
                .GetBrandPaymentAclsAsync(brandPayments.Select(bp => bp.Id), query.PlayerCode)
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

            var brandPaymentVipLevels = await _walletRepository.GetBrandPaymentVipListAsync(brandPayments.Select(bp => bp.Id))
                .ToLookupAsync(bv => bv.BrandPaymentId, bv => bv.VipLevelId)
                .ConfigureAwait(false);

            var playerInfo = await _walletRepository.GetPlayerInfoAsync(query.PlayerCode).ConfigureAwait(false);

            var blockListCondition = new BlockListCondition(blockList);
            var whiteListCondition = new WhiteListCondition(whiteList);
            var showOnAllPlayerCondition = new ShowOnAllPlayersCondition();

            foreach (var bp in brandPayments)
            {
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
                    bp.ConditionMatchType == ConditionMatchTypes.AllMatch
                        ? new AllMatchCondition(
                            depositedAmountCondition,
                            identityCondition,
                            phoneCondition,
                            vipCondition)
                        : new SomeOneMatchCondition(
                            depositedAmountCondition,
                            identityCondition,
                            phoneCondition,
                            vipCondition));

                if (filterCondition.IsValid(bp, playerInfo))
                    yield return bp;
            }
        }
    }
}
