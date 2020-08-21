SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Player_GetPlayerForCache') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Player_GetPlayerForCache AS');
GO

ALTER PROCEDURE [dbo].[usp_Player_GetPlayerForCache]
      @playerCode  NVARCHAR(16)
AS 
BEGIN
    SET NOCOUNT ON
    
    ;WITH player 
    as
    (
        SELECT 
            p.BrandId,
	        p.ExternalId,
	        p.Type,
	        p.Username,
	        p.Status,
	        p.DefaultWalletId,
	        p.CultureCode,
	        p.BetLimitGroupId,
	        p.LastCashierUrl,
	        p.LastHelpUrl,
	        p.LastLoginUrl,
	        p.LastTermsUrl,
	        p.PlayerCode
        FROM dbo.[Players] p
        WHERE p.PlayerCode = @playerCode
    )
    SELECT 
        player.*,
        --brand.ProxyWalletConfigurationId as [BrandProxyWalletConfigurationId],
	    brand.Code as [BrandCode],
	    wallet.CurrencyCode as [DefaultWalletCurrencyCode],
	    wallet.ExternalCurrencyCode
    FROM player 
	JOIN dbo.[Brands] brand on player.BrandId=brand.Id 
	JOIN dbo.[Wallets] wallet on player.DefaultWalletId=wallet.Id
END
GO