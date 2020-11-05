SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_ApiLogs_GetLog]
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


