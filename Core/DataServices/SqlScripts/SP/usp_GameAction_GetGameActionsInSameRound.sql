SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_GameAction_GetGameActionsInSameRound') IS NULL
    EXEC('CREATE PROCEDURE [dbo].[usp_GameAction_GetGameActionsInSameRound] AS');
GO

ALTER PROCEDURE [dbo].[usp_GameAction_GetGameActionsInSameRound]
    @gameProviderConfigurationId    UNIQUEIDENTIFIER,
    @walletId                       UNIQUEIDENTIFIER,
    @externalRoundId                NVARCHAR(255)
AS
BEGIN

    SET NOCOUNT ON;

    EXEC [Bet].[dbo].[usp_GameAction_GetGameActionsInSameRound] @gameProviderConfigurationId, @walletId, @externalRoundId

END

GO


