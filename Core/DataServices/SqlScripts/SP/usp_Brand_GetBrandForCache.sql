SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Brand_GetBrandForCache') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Brand_GetBrandForCache AS');
GO

ALTER PROCEDURE [dbo].[usp_Brand_GetBrandForCache]
      @brandId  UNIQUEIDENTIFIER
AS 
BEGIN
    SET NOCOUNT ON
    
	;WITH brand
	AS
	(
		SELECT 
			ResellerId,Id,TimeZoneId,Code,[Status],CashierUrl,HelpUrl,LoginUrl,TermsUrl,MobileCashierUrl
			,MobileHelpUrl,MobileLoginUrl,MobileTermsUrl
		FROM dbo.Brands 
		WHERE Id=@brandId
	)
	SELECT 
		brand.Id,brand.Code,brand.TimeZoneId,brand.[Status],CashierUrl,HelpUrl,LoginUrl,TermsUrl,MobileCashierUrl
		,MobileHelpUrl,MobileLoginUrl,MobileTermsUrl
		,reseller.Id AS [ResellerId],reseller.Code AS [ResellerCode]		
	FROM brand 
	JOIN dbo.Resellers reseller 
	ON reseller.Id = brand.ResellerId     
END
GO