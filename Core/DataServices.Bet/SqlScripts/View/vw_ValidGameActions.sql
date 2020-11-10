SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER VIEW [dbo].[vw_ValidGameActions]
AS
    SELECT ga.Id, 
            ga.ExternalTransactionId, 
            ga.[Type], 
            ga.RoundId, 
            ga.WalletId,
            ga.ReferencedGameActionId, 
            ga.GameActionBatchId, 
            ga.GameProviderConfigurationId, 
            ga.RecordedOn, 
            ga.ExternalBetId, 
            ga.SentOn,
            ga.PlayerIp, 
            ga.Turnover, 
            ga.WinLoss, 
            ga.UnsettledBets, 
            ga.IsClosingRound, 
            ga.ClosingRoundExplicitRequest, 
            ga.LicenseeId, 
            ga.Amount,
            ga.ExternalGameActionGroupId,
            ga.Balance, 
            ga.BetType, 
            ga.[Description]
     FROM dbo.GameActions AS ga --WITH ( NOLOCK, FORCESEEK )
     WHERE ( ga.Type <> 5 )
       AND NOT EXISTS (SELECT 1 
                       FROM dbo.GameActions ref --WITH ( NOLOCK, FORCESEEK ) 
                       WHERE ref.Type = 5 
                          AND ref.ReferencedGameActionId = ga.Id 
                          AND ref.RoundId = ga.RoundId
                          AND NOT EXISTS (SELECT 1 
                                          FROM dbo.GameActions cancel --WITH ( NOLOCK, FORCESEEK ) 
                                          WHERE cancel.Type = 5
                                             AND cancel.ReferencedGameActionId = ref.Id 
                                             AND cancel.RoundId = ref.RoundId ) )
GO
