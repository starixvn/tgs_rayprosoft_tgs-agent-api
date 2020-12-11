SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Round_InsertRound]
        @roundId                        uniqueidentifier,
        @externalRoundId                nvarchar(255),
        @walletId                       uniqueidentifier,
        @gameId                         int,
        @gameName                       nvarchar(255),
        @gameType                       smallint,
        @gameProviderId                 uniqueidentifier,
        @gameProviderCode               nvarchar(16),
        @gameProviderName               nvarchar(255),        
        @playerCode                     varchar(16),
        @playerType                     smallint,
        @currencyCode                   varchar(10),
        @platformType                   smallint,
        @status                         smallint,
        @gameProviderConfigurationId    uniqueidentifier,   
        @resellerId                     uniqueidentifier,
        @resellerCode                   nvarchar(16),
        @licenseeId                     uniqueidentifier,
        @licenseeCode                   nvarchar(16),
        @brandId                        uniqueidentifier,
        @brandCode                      nvarchar(16),
        @createdOn                      datetimeoffset,
        @closedOn                       datetimeoffset,
        @updatedOn                      datetimeoffset,
        @externalCreatedOn              datetimeoffset,
        @externalClosedOn               datetimeoffset NULL,
        @roundDetail                    nvarchar(max) NULL
AS 
BEGIN
    SET NOCOUNT ON
    
    INSERT INTO [dbo].[Rounds]
           ([Id],
            [ExternalRoundId],
            [WalletId],
            [GameId],
            [GameName],
            [GameType],
            [GameProviderId],
            [GameProviderCode],
            [GameProviderName],           
            [PlayerCode],
            [PlayerType],
            [CurrencyCode],
            [PlatformType],
            [Status],
            [GameProviderConfigurationId],
            [ResellerId],
            [ResellerCode],
            [LicenseeId],
            [LicenseeCode],
            [BrandId],
            [BrandCode],
            [CreatedOn],
            [ClosedOn],
            [UpdatedOn],
            [ExternalCreatedOn],
            [ExternalClosedOn])
     VALUES
           (@roundId,
            @externalRoundId,
            @walletId,
            @gameId,
            @gameName,
            @gameType,
            @gameProviderId,
            @gameProviderCode,
            @gameProviderName,           
            @playerCode,
            @playerType,
            @currencyCode,
            @platformType,
            @status,
            @gameProviderConfigurationId,
            @resellerId,
            @resellerCode,
            @licenseeId,
            @licenseeCode,
            @brandId,
            @brandCode,
            @createdOn,
            @closedOn,
            @updatedOn,
            @externalCreatedOn,
            @externalClosedOn);

   IF ISNULL(@roundDetail, '') <> ''
   BEGIN
        INSERT INTO [dbo].[RoundDetails]
               ([RoundId],
                [AdditionalData])
         VALUES
               (@roundId,           
                @roundDetail);  
   END

END
GO