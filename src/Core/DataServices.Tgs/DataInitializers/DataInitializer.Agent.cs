using System;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Utility;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    public partial class DataInitializer
    {
        private Agent CreateAgent(
            string userName,
            AgentStatus status,
            AgentConfigStatus agentStatus,
            AgentLevelType levelType,
            AgentType type,
            Guid brandId,
            string currencyCode,
            Guid? agentId = null,
            Guid? parentAgentId = null)
        {
            var (salt, passwordHash) = _passwordHandler.GenerateCredential(UserPassword);
            agentId ??= GuidExtensions.NewSequentialGuid();
            var invitationCode = InvitationCodeGenerator.NewId();
            var defaultWalletId = GuidExtensions.NewSequentialGuid();

            return new()
            {
                Id = agentId.Value,
                Username = userName,
                Password = passwordHash,
                Salt = salt,
                WithdrawPassword = passwordHash,
                WithdrawSalt = salt,
                Status = status,
                AgentStatus = agentStatus,
                LevelType = levelType,
                Type = type,
                InvitationCode = invitationCode,
                BrandId = brandId,
                ParentAgentId = parentAgentId,
                CommissionPercentage = 10,
                DividendPercentage = 10,
                LiveDealerRebatePercentage = 10,
                SlotRebatePercentage = 10,
                LotteryRebatePercentage = 10,
                CardGameRebatePercentage = 10,
                SportsRebatePercentage = 10,
                VirtualSportsRebatePercentage = 10,
                RegisteredOn = _Now,
                DefaultWalletId = defaultWalletId,
                Wallets = new[]
                {
                    new AgentWallet
                    {
                        Id = defaultWalletId,
                        AgentId = agentId.Value,
                        CurrencyCode = currencyCode,
                        Balance = 10,
                        CreatedOn = _Now,
                        UpdatedOn = _Now
                    }
                },
                CreatedByUsername = _AdminUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedByUsername = _AdminUsername,
                UpdatedOn = DateTimeOffset.Now
            };
        }
    }
}