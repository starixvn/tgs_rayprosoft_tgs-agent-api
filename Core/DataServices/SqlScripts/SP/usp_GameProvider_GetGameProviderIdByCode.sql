SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_GameProvider_GetGameProviderIdByCode') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_GameProvider_GetGameProviderIdByCode AS');
GO

ALTER PROCEDURE [dbo].[usp_GameProvider_GetGameProviderIdByCode]
      @gameProviderCode  NVARCHAR(10)
AS 
BEGIN
    SET NOCOUNT ON

    SELECT [Id] FROM [dbo].[GameProviders]
    WHERE [Code] = @gameProviderCode
END
GO