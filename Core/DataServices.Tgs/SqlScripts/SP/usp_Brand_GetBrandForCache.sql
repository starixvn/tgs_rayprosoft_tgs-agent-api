SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_Brand_GetBrandForCache]
      @brandId  UNIQUEIDENTIFIER
AS 
BEGIN
    SET NOCOUNT ON
    
	;WITH Brand
	AS
	(
		SELECT 			
			[Id],
			[TimeZoneId],
			[Code],
			[Name],
			[Status],
			[ClientId],
			[SecretKey],
			[LicenseeId]
		FROM dbo.[Brands]
		WHERE [Id] = @brandId
	)
	SELECT 
		b.[Id],
		b.[Code],
		b.[Name],
		b.[TimeZoneId],
		b.[Status],		
		b.[ClientId],
		b.[SecretKey],
		l.[Id] AS [LicenseeId],
		l.[Code] AS [LicenseeCode],
		r.[Id] AS [ResellerId],
		r.[Code] AS [ResellerCode]		
	FROM [Brand] b
	JOIN dbo.[Licensees] l	ON l.[Id] = b.[LicenseeId]
	JOIN dbo.[Resellers] r 	ON r.[Id] = l.[ResellerId]
END
GO