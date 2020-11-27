SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GameAction_GetGameActionExternalIds]
    @gameProviderId		UNIQUEIDENTIFIER,
	@playerCode			VARCHAR(16),
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
        ga.RecordedOn,
        ExternalReferencedTransactionId = (SELECT ExternalTransactionId FROM GameActions WHERE ga.ReferencedGameActionId = Id)
    FROM dbo.GameActions ga --WITH ( NOLOCK )
    JOIN dbo.Rounds r --WITH ( NOLOCK ) 
    ON r.Id = ga.RoundId
    WHERE r.ExternalRoundId = @externalRoundId        
      AND r.GameProviderId = @gameProviderId
      AND r.PlayerCode = @playerCode
      AND ga.GameProviderConfigurationId = r.GameProviderConfigurationId
      AND ga.WalletId = r.WalletId
      AND ga.[Type] IN (SELECT [Value] FROM @gameActionTypes)
END

GO


