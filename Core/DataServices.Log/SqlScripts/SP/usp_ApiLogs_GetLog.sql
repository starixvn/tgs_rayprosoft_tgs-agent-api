SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_ApiLogs_GetLog') IS NULL
    EXEC('CREATE PROCEDURE [dbo].[usp_ApiLogs_GetLog] AS');
GO

ALTER PROCEDURE [dbo].[usp_ApiLogs_GetLog]
    @trackingId		UNIQUEIDENTIFIER
AS
BEGIN

    SET NOCOUNT ON;

    SELECT
		RequestHeaders,
		RequestContent,
		ResponseHeaders,
		ResponseContent
    FROM dbo.ApiLogs WITH ( NOLOCK )
    WHERE TrackingId = @trackingId

END

GO


