SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_GameAction_GetValidGameActionExternalIds') IS NULL
    EXEC('CREATE PROCEDURE [dbo].[usp_GameAction_GetValidGameActionExternalIds] AS');
GO

ALTER PROCEDURE [dbo].[usp_GameAction_GetValidGameActionExternalIds]
    @gameProviderId					UNIQUEIDENTIFIER,
	@walletId						UNIQUEIDENTIFIER,
	@externalRoundId                NVARCHAR(255),
	@onlyPlacements					BIT
AS
BEGIN

    SET NOCOUNT ON;
    
    SELECT
        ga.ExternalBetId, 
        ga.ExternalTransactionId, 
        r.ExternalRoundId,
        ga.ExternalGameActionGroupId,
        ga.[Type] AS GameActionType,
        ga.Amount,
        ga.RecordedOn
    FROM dbo.vw_ValidGameActions ga --WITH ( NOLOCK )
    JOIN dbo.Rounds r --WITH ( NOLOCK ) 
    ON r.Id = ga.RoundId
    WHERE
        r.ExternalRoundId = @externalRoundId
        AND
        ga.GameProviderConfigurationId = r.GameProviderConfigurationId
        AND
        r.GameProviderId = @gameProviderId
        AND
        ga.WalletId = @walletId
        AND
        (@onlyPlacements = 0 OR ga.[Type] = 0) -- 0 is a PlaceBet	
END

GO


