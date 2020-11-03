IF EXISTS (SELECT * FROM sys.objects WHERE [name] = N'PaymentAuditLog' AND [type] = 'TR')
BEGIN
      DROP TRIGGER [dbo].[PaymentAuditLog]
END
GO

-- Triggrt
CREATE TRIGGER [dbo].[PaymentAuditLogs] ON [dbo].[Payments]
AFTER INSERT, UPDATE
AS
BEGIN
DECLARE @Operation char(1);
     
    IF EXISTS(SELECT 1 FROM inserted) AND NOT EXISTS(SELECT 1 FROM deleted)   
        SET @Operation = 0        --Insert   
    
    IF EXISTS(SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM deleted)  
        SET @Operation = 1        --Update  
     
    IF NOT EXISTS(SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM deleted)  
        SET @Operation = 2        --Delete 

    SET NOCOUNT ON;
    INSERT INTO [dbo].[PaymentAuditLogs] VALUES ( 
    Id,
    ChannelName,
    Enabled,
    TransferLimitFrom,
    TransferLimitTo,
    TransferCondition,
    AllPlayer,
    IdentityVerification,
    PhoneVerification,
    DepositedAmountUnlimited,
    DepositedAmountFrom,
    DepositedAmountTo,
    Reason,
    UpdateOn,
    UpdatedByUsername,
    @Operation)  
        SELECT * FROM inserted
END
GO
