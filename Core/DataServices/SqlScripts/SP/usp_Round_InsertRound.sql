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
    
    EXEC [Bet].[dbo].[usp_Round_InsertRound] 
        @roundId,
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
        @externalClosedOn,
        @roundDetail
END
GO