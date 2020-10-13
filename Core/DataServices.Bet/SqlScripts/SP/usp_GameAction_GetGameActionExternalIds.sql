SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_GameAction_GetGameActionExternalIds') IS NULL
    EXEC('CREATE PROCEDURE [dbo].[usp_GameAction_GetGameActionExternalIds] AS');
GO

ALTER PROCEDURE [dbo].[usp_GameAction_GetGameActionExternalIds]
    @gameProviderId		UNIQUEIDENTIFIER,
	@walletId			UNIQUEIDENTIFIER,
	@externalRoundId    NVARCHAR(255),
	@gameActionTypes    IntValuesType READONLY
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
    FROM dbo.GameActions ga --WITH ( NOLOCK )
    JOIN dbo.Rounds r --WITH ( NOLOCK ) 
    ON r.Id = ga.RoundId
    WHERE r.ExternalRoundId = @externalRoundId        
      AND r.GameProviderId = @gameProviderId
      AND ga.GameProviderConfigurationId = r.GameProviderConfigurationId
      AND ga.WalletId = @walletId
      AND ga.[Type] IN (SELECT [Value] FROM @gameActionTypes) 	
END

GO


