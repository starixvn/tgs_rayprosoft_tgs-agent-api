SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Wallet_GetBalance') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Wallet_GetBalance AS');
GO

ALTER PROCEDURE [dbo].[usp_Wallet_GetBalance]
      @walletId    UNIQUEIDENTIFIER,
	  @isDirtyRead BIT
AS 
BEGIN
    SET NOCOUNT ON
    
    DECLARE @sql NVARCHAR(MAX) = '
        SELECT [Balance]
        FROM dbo.[Wallets]';

    IF @isDirtyRead = 1
        SET @sql += ' WITH (NOLOCK)';
    
    SET @sql += N'         
        WHERE [Id] = @walletId';

    DECLARE @param NVARCHAR(MAX) = N'@walletId  UNIQUEIDENTIFIER';

    EXEC SP_EXECUTESQL @sql, @param,
        @walletId
END
GO