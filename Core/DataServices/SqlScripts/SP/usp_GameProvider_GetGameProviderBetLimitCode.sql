SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_GameProvider_GetGameProviderBetLimitCode') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_GameProvider_GetGameProviderBetLimitCode AS');
GO

ALTER PROCEDURE [dbo].[usp_GameProvider_GetGameProviderBetLimitCode]
      @gameProviderId   uniqueidentifier,
      @currencyCode     nvarchar(10),
      @betLimitGroupId  int      
AS 
BEGIN
    SET NOCOUNT ON

    SELECT Code     
    FROM dbo.[GameProviderBetLimits]
    WHERE GameProviderId = @gameProviderId
    AND CurrencyCode = @currencyCode
    AND BetLimitGroupId = @betLimitGroupId
END
GO