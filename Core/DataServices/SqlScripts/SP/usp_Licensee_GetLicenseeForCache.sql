SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Licensee_GetLicenseeForCache') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Licensee_GetLicenseeForCache AS');
GO

ALTER PROCEDURE [dbo].[usp_Licensee_GetLicenseeForCache]
      @licenseeId  UNIQUEIDENTIFIER
AS 
BEGIN
    SET NOCOUNT ON
    
	;WITH Licensee
	AS
	(
		SELECT 
			[ResellerId],
			[Id],
			[TimeZoneId],
			[Code],
			[Name],
			[Status],
			[CashierUrl],
			[HelpUrl],
			[LoginUrl],
			[TermsUrl],
			[MobileCashierUrl],
			[MobileHelpUrl],
			[MobileLoginUrl],
			[MobileTermsUrl]
		FROM dbo.[Licensees]
		WHERE [Id] = @licenseeId
	)
	SELECT 
		l.[Id],
		l.[Code],
		l.[Name],
		l.[TimeZoneId],
		l.[Status],
		l.[CashierUrl],
		l.[HelpUrl],
		l.[LoginUrl],
		l.[TermsUrl],
		l.[MobileCashierUrl],
		l.[MobileHelpUrl],
		l.[MobileLoginUrl],
		l.[MobileTermsUrl],
		r.[Id] AS [ResellerId],
		r.[Code] AS [ResellerCode]		
	FROM [Licensee] l
	JOIN dbo.[Resellers] r 
	ON r.[Id] = l.[ResellerId]    
END
GO