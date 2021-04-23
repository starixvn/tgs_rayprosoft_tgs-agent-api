namespace TGS.Shared.Game.Models.PragmaticPlay.Enums
{
    public enum GameApiError
    {
        Success = 0,
        InsufficientBalance = 1,
        PlayerNotFound = 2,
        BetIsNotAllowed = 3,
        PlayerTokenInvalid = 4,
        InvalidHash = 5,
        PlayerIsFrozen = 6,
        BadParameters = 7,
        GameIsNotFoundOrDisabled = 8,
        BetLimitHasBeenReached = 50,
        InternalServerErrorWillReconcile = 100,
        InternalServerErrorWillNotReconcile = 120,
        RealityCheckWarning = 210
    }
}