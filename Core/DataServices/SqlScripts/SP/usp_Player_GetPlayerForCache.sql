SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Player_GetPlayerForCache]
      @playerCode  NVARCHAR(16)
AS 
BEGIN
    SET NOCOUNT ON
    
    ;WITH player 
    as
    (
        SELECT 
            p.BrandId,
	        p.Type,
	        p.Username,
	        p.Status,
	        p.DefaultWalletId,
	        p.CultureCode,
	        p.BetLimitId,
	        p.PlayerCode
        FROM dbo.[Players] p
        WHERE p.PlayerCode = @playerCode
    )
    SELECT 
        p.*,
	    b.Code as [BrandCode],
	    w.CurrencyCode as [DefaultWalletCurrencyCode],
	    w.ExternalCurrencyCode
    FROM player p
	JOIN dbo.[Brands] b on p.BrandId = b.Id 
	JOIN dbo.[Wallets] w on p.DefaultWalletId = w.Id
END
GO