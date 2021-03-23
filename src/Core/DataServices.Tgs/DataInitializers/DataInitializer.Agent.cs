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
            Status status,
            AgentStatus agentStatus,
            AgentLevelType levelType,
            AgentType type,
            Guid brandId,
            decimal positionTakingPercentage,
            decimal bonusPercentage,
            decimal rebatePercentage,
            string currencyCode,
            Guid? agentId = null,
            Guid? parentAgentId = null)
        {
            var (salt, passwordHash) = _passwordHandler.GenerateCredential(UserPassword);
            agentId ??= GuidExtensions.NewSequentialGuid();
            var invitationCode = InvitationCodeGenerator.NewId();

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
                PositionTakingPercentage = positionTakingPercentage,
                BonusPercentage = bonusPercentage,
                RebatePercentage = rebatePercentage,
                RegisteredOn = Now,
                Wallets = new[]
                {
                    new AgentWallet
                    {
                        Id = GuidExtensions.NewSequentialGuid(),
                        AgentId = agentId.Value,
                        CurrencyCode = currencyCode,
                        Balance = 10,
                        CreatedOn = Now,
                        UpdatedOn = Now
                    }
                },
                CreatedByUsername = AdminUsername,
                CreatedOn = DateTimeOffset.Now,
                UpdatedByUsername = AdminUsername,
                UpdatedOn = DateTimeOffset.Now
            };
        }
    }
}