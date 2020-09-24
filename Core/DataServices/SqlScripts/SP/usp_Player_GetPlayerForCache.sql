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
            p.LicenseeId,
	        p.Type,
	        p.Username,
	        p.Status,
	        p.DefaultWalletId,
	        p.CultureCode,
	        p.BetLimitGroupId,
	        p.PlayerCode
        FROM dbo.[Players] p
        WHERE p.PlayerCode = @playerCode
    )
    SELECT 
        p.*,
	    l.Code as [LicenseeCode],
	    w.CurrencyCode as [DefaultWalletCurrencyCode],
	    w.ExternalCurrencyCode
    FROM player p
	JOIN dbo.[Licensees] l on p.LicenseeId = l.Id 
	JOIN dbo.[Wallets] w on p.DefaultWalletId = w.Id
END
GO