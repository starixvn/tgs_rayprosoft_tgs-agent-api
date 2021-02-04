namespace TGS.Core.Models.Enums
{
    public enum InvalidGameActionErrorCode
    {
        None,
        Duplicate,
        Cancelled,
        Settled,
        TransactionNotFound,
        TransactionBeingCancelledNotFound,
        TransactionBeingReferencedNotFound,
        RoundNotFound,
        GameNotFound,
        ActionProhibited        
    }
}