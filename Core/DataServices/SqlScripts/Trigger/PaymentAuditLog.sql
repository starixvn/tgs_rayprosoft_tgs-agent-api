IF EXISTS (SELECT * FROM sys.objects WHERE [name] = N'Tr_PaymentAuditLogs' AND [type] = 'TR')
BEGIN
      DROP TRIGGER [dbo].[Tr_PaymentAuditLogs]
END
GO

-- Triggrt
CREATE TRIGGER [dbo].[Tr_PaymentAuditLogs] ON [dbo].[Payments]
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

    INSERT INTO [dbo].[PaymentAuditLogs] ( 
    Id,
    PaymentId,
    ChannelName,
    PaymentStatus,
    TransferLimitFrom,
    TransferLimitTo,
    PlayerListCondition,
    AllPlayer,
    IdentityVerification,
    PhoneVerification,
    DepositedAmountUnlimited,
    DepositedAmountFrom,
    DepositedAmountTo,
    Reason,
    CreatedOn,
    CreatedByUsername,
    Operation)  
        SELECT NEWID() Id,
        Id AS PaymentId,
    ChannelName,
    PaymentStatus,
    TransferLimitFrom,
    TransferLimitTo,
    PlayerListCondition,
    AllPlayer,
    IdentityVerification,
    PhoneVerification,
    DepositedAmountUnlimited,
    DepositedAmountFrom,
    DepositedAmountTo,
    Reason,
    UpdatedOn,
    UpdatedByUsername,
	@Operation FROM inserted
END
GO
