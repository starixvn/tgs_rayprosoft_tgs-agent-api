SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_GameAction_InsertGameAction') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_GameAction_InsertGameAction AS');
GO

ALTER PROCEDURE [dbo].[usp_GameAction_InsertGameAction]
          @gameActionId                 uniqueidentifier,
          @gameProviderConfigurationId  uniqueidentifier,
          @walletId                     uniqueidentifier,
          @roundId                      uniqueidentifier,
          @externalTransactionId        nvarchar(128),
          @type                         smallint,
          @amount                       decimal(18,6),
          @turnover                     decimal(18,6),
          @winLoss                      decimal(18,6),
          @unsettledBets                decimal(18,6),
          @balance                      decimal(18,6),
          @betType                      nvarchar(128) NULL,
          @externalBetId                nvarchar(64) NULL,
          @externalGameActionGroupId    nvarchar(128) NULL,
          @referencedGameActionId       uniqueidentifier NULL,
          @gameActionBatchId            uniqueidentifier NULL,
          @recordedOn                   datetimeoffset,
          @sentOn                       datetimeoffset,
          @isClosingRound               bit,
          @closingRoundExplicitRequest  bit NULL,
          @licenseeId                   uniqueidentifier,
          @playerCode                   varchar(16),
          @playerIp                     varchar(128),
          @description                  nvarchar(4000) NULL
AS 
BEGIN
    SET NOCOUNT ON
    
    INSERT INTO [dbo].[GameActions]
           ([Id],
            [GameProviderConfigurationId],
            [WalletId],
            [RoundId],
            [ExternalTransactionId],
            [Type],
            [Amount],
            [Turnover],
            [WinLoss],
            [UnsettledBets],
            [Balance],
            [BetType],
            [ExternalBetId],
            [ExternalGameActionGroupId],
            [ReferencedGameActionId],
            [GameActionBatchId],
            [RecordedOn],
            [SentOn],
            [IsClosingRound],
            [ClosingRoundExplicitRequest],
            [LicenseeId],
            [PlayerCode],
            [PlayerIp],
            [Description])
     VALUES
           (@gameActionId,
            @gameProviderConfigurationId,
            @walletId,
            @roundId,
            @externalTransactionId,
            @type,
            @amount,
            @turnover,
            @winLoss,
            @unsettledBets,
            @balance,
            @betType,
            @externalBetId,
            @externalGameActionGroupId,
            @referencedGameActionId,
            @gameActionBatchId,
            @recordedOn,
            @sentOn,
            @isClosingRound,
            @closingRoundExplicitRequest,
            @licenseeId,
            @playerCode,
            @playerIp,
            @description);
END
GO