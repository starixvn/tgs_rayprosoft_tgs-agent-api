SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID('usp_GameAction_GetGameActionsByExternalRoundId') IS NULL
    EXEC('CREATE PROCEDURE [dbo].[usp_GameAction_GetGameActionsByExternalRoundId] AS');
GO

ALTER PROCEDURE [dbo].[usp_GameAction_GetGameActionsByExternalRoundId]
    @gameProviderId		    UNIQUEIDENTIFIER,
	@playerCode			    VARCHAR(16),
	@externalRoundId        NVARCHAR(255)
AS
BEGIN

    SET NOCOUNT ON;
    
    SELECT
        ga.Id,        
        ga.ExternalBetId, 
        ga.ExternalTransactionId,
        r.Id AS RoundId,
        r.ExternalRoundId,
        ga.ExternalGameActionGroupId,
        ga.[Type] AS GameActionType,
        ga.Amount,
        ga.RecordedOn,
        ga.ReferencedGameActionId,
        r.[Status] AS RoundStatus,
        r.BetAmount AS RoundBetAmount,
        r.WinLoss AS RoundWinLoss,
        r.Commission,
        r.TieBetAdjustment,
        r.ClosedOn AS RoundClosedOn
    FROM dbo.GameActions ga WITH (NOLOCK)
    JOIN dbo.Rounds r WITH (NOLOCK) 
    ON r.Id = ga.RoundId
    WHERE r.ExternalRoundId = @externalRoundId        
      AND r.GameProviderId = @gameProviderId
      AND r.PlayerCode = @playerCode
      AND ga.GameProviderConfigurationId = r.GameProviderConfigurationId
      AND ga.WalletId = r.WalletId
END

GO


