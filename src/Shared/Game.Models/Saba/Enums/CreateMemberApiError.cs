namespace TGS.Shared.Game.Models.Saba.Enums
{
    public enum CreateMemberApiError
    {
        Ok = 0,
        Fail = 1,
        DuplicatedUsername = 2,
        WrongOperatorId = 3,
        WrongOddsType = 4,
        InvalidCurrency = 5,
        DuplicatedVendorMemberId = 6,
        MinTransferGreaterThanMaxTransfer = 7,
        InvalidArgument = 8,
        InvalidCredential = 9,
        SystemUnderMaintenance = 10,
        MinTransferLessThanOne = 11,
        IdExceedLengthLimit = 12
    }
}