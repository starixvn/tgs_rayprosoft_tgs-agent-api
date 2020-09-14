SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('usp_Wallet_UpdateWalletBalance') IS NULL
    EXEC('CREATE PROCEDURE dbo.usp_Wallet_UpdateWalletBalance AS');
GO

ALTER PROCEDURE [dbo].[usp_Wallet_UpdateWalletBalance]
      @playerCode  NVARCHAR(16),
      @walletId    UNIQUEIDENTIFIER,
      @amount      DECIMAL(18, 6),
	  @updatedOn   DATETIMEOFFSET
AS 
BEGIN
    SET NOCOUNT ON;
	SET LOCK_TIMEOUT 3000;
	SET XACT_ABORT ON;

    DECLARE @PostBalance TABLE(
        Balance DECIMAL(18,6)
    );

    BEGIN TRAN

    UPDATE dbo.[Wallets]
       SET [Balance] = [Balance] + @amount,
           [UpdatedOn] = @updatedOn
    OUTPUT inserted.Balance into @PostBalance
    WHERE [Id] = @walletId AND PlayerCode = @playerCode

	IF @@ROWCOUNT = 0
		THROW 51000, 'Wallet Not Found.' , 0

	COMMIT TRAN;

	SELECT * FROM @PostBalance

END
GO