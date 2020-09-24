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
			[Status]
		FROM dbo.[Licensees]
		WHERE [Id] = @licenseeId
	)
	SELECT 
		l.[Id],
		l.[Code],
		l.[Name],
		l.[TimeZoneId],
		l.[Status],		
		r.[Id] AS [ResellerId],
		r.[Code] AS [ResellerCode]		
	FROM [Licensee] l
	JOIN dbo.[Resellers] r 
	ON r.[Id] = l.[ResellerId]    
END
GO