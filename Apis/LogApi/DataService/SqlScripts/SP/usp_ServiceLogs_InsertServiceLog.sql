SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_ServiceLogs_InsertServiceLog') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_ServiceLogs_InsertServiceLog AS');
GO

ALTER PROCEDURE [dbo].[usp_ServiceLogs_InsertServiceLog]
      @recordedon  datetimeoffset,
      @machineName nvarchar(512),
      @level nvarchar(10),
      @serverAddress nvarchar(512),
      @remoteAddress nvarchar(512),
      @logger nvarchar(512),
      @port nvarchar(255),
      @properties nvarchar(512),
      @callsite nvarchar(512),
      @message nvarchar(512),
      @exception nvarchar(4000),
      @serverName nvarchar(512),
      @url nvarchar(512),
      @https nvarchar(255),
      @stackTrace nvarchar(max)
AS 
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[ServiceLogs]
    (
        [RecordedOn],
        [MachineName],
        [Level],
        [ServerAddress],
        [RemoteAddress],
        [Logger],
        [Port],
        [Properties],
        [Callsite],
        [Message],
        [Exception],
        [ServerName],
        [Url],
        [Https],
        [StackTrace]
    )
    VALUES
    (
        @recordedon,
        @machineName,
        @level,
        @serverAddress,
        @remoteAddress,
        @logger,
        @port,
        @properties,
        @callsite,
        @message,
        @exception,
        @serverName,
        @url,
        @https,
        @stackTrace
    );
    
END
GO