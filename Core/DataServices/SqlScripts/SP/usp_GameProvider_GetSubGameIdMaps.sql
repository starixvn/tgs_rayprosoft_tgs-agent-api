SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_GameProvider_GetSubGameIdMaps') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_GameProvider_GetSubGameIdMaps AS');
GO

ALTER PROCEDURE [dbo].[usp_GameProvider_GetSubGameIdMaps]
      @gameProviderId  UNIQUEIDENTIFIER,
      @platformType    SMALLINT
AS 
BEGIN
    SET NOCOUNT ON
    
	SELECT
        [Id],        
        [ExternalId]
    FROM dbo.[Games]
    WHERE [GameProviderId] = @gameProviderId
      AND [SubGame] = 1
      AND [PlatformType] = @platformType
END
GO