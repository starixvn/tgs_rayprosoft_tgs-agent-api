SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Round_InsertRound') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Round_InsertRound AS');
GO

ALTER PROCEDURE [dbo].[usp_Round_InsertRound]
        @id     uniqueidentifier,
        @externalRoundId    nvarchar(255),
        @walletId           uniqueidentifier,
        @gameId uniqueidentifier,
        @gameName           nvarchar(255),
        @gameType           smallint,
        @gameProviderId     uniqueidentifier,
        @gameProviderCode   nvarchar(16),
        @gameProviderName   nvarchar(255),        
        @playerCode         varchar(16),
        @playerType         smallint,
        @currencyCode       varchar(10),
        @platformType       smallint,
        @status smallint,
        @gameProviderConfigurationId    uniqueidentifier,       
        @licenseeId         uniqueidentifier,
        @licenseeCode       nvarchar(16),
        @licenseeName       nvarchar(255),
        @createdOn          datetimeoffset,
        @closedOn           datetimeoffset,
        @updatedOn          datetimeoffset,
        @externalCreatedOn  datetimeoffset,
        @externalClosedOn   datetimeoffset NULL,
        @roundDetail        nvarchar(max) NULL
AS 
BEGIN
    SET NOCOUNT ON
    
    EXEC [Bet].[dbo].[usp_Round_InsertRound] 
        @id,
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
        @currencyCode,@platformType,
        @status,
        @gameProviderConfigurationId,
        @licenseeId,
        @licenseeCode,
        @licenseeName,
        @createdOn,
        @closedOn,
        @updatedOn,
        @externalCreatedOn,
        @externalClosedOn,
        @roundDetail
END
GO