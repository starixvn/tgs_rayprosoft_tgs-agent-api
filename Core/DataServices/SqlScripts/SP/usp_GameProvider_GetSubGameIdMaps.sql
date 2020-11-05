SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_GameProvider_GetSubGameIdMaps]
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