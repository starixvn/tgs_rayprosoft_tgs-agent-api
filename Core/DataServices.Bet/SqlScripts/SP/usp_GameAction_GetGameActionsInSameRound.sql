SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GameAction_GetGameActionsInSameRound]
    @gameProviderConfigurationId    UNIQUEIDENTIFIER,
    @walletId                       UNIQUEIDENTIFIER,
    @externalRoundId                NVARCHAR(255)
AS
BEGIN

    SET NOCOUNT ON;
    SET LOCK_TIMEOUT 3000;
    SET XACT_ABORT ON;

    SELECT ga.[Id] AS [GameActionId],
           ga.[ReferencedGameActionId] AS [ReferenceGameActionId],
           ga.[ExternalTransactionId] AS [ExternalTransactionId],
           ga.[Turnover] AS [Turnover],
           ga.[WinLoss] AS [WinLoss],
           ga.[UnsettledBets] AS [UnsettledBets],
           ga.[IsClosingRound] AS [IsClosingRound],
           ga.[ClosingRoundExplicitRequest] AS [ClosingRoundExplicitRequest],
           ga.[Type] AS [Type],
           r.[Id] AS [RoundId],
           r.[GameId] AS [GameId],
           r.[Status] AS [RoundStatus],
           r.[ExternalRoundId] AS [ExternalRoundId],
           ga.[Amount] AS [Amount],
           ga.[RecordedOn] AS [GameActionRecordedOn],
		   ga.[SentOn] AS [GameActionSentOn],
           ga.[BetType],
		   ga.[WalletId] AS [WalletId]
      FROM dbo.[Rounds] r WITH(UPDLOCK)
	  JOIN dbo.[GameActions] ga ON ga.[RoundId] = r.Id
      WHERE r.[ExternalRoundId] = @externalRoundId
        AND r.[WalletId] = @walletId 
        AND r.[GameProviderConfigurationId] = @gameProviderConfigurationId
        AND ga.[RecordedOn] >= DATEADD(minute, -5, r.CreatedOn)
     ORDER BY ga.[RecordedOn] ASC
END

GO


