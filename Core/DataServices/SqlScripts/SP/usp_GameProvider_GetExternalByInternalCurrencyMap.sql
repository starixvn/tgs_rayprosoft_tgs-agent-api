SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_GameProvider_GetExternalByInternalCurrencyMap') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_GameProvider_GetExternalByInternalCurrencyMap AS');
GO

ALTER PROCEDURE [dbo].[usp_GameProvider_GetExternalByInternalCurrencyMap]
      @gameProviderId  uniqueidentifier
AS 
BEGIN
    SET NOCOUNT ON

    SELECT [Id] FROM [dbo].[GameProviders]
    WITH (NOLOCK)
    WHERE [Code] = @gameProviderCode
END
GO