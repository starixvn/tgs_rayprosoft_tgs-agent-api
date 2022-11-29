using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using TGS.Services.Agent.Exceptions;
using AgentNotFoundException = TGS.Core.Models.Exceptions.AgentNotFoundException;

namespace TGS.Apis.AgentApi.Models;

public class ApiErrorMapper : IErrorMapper
{
    private static readonly IReadOnlyDictionary<AgentApiError, string> _ErrorCodeMessageIdMap =
        new Dictionary<AgentApiError, string>
        {
            [AgentApiError.InvalidCredentials] = "InvalidCredentials",
            [AgentApiError.InvalidPassword] = "InvalidCredentials",
            [AgentApiError.InvalidOrExpiredAgentToken] = "InvalidOrExpiredAgentToken",
            [AgentApiError.InvalidCaptcha] = "InvalidCaptcha",
            [AgentApiError.ClientIdNotFound] = "ClientIdNotFound",
            [AgentApiError.AgentNotFound] = "AgentNotFound",
            [AgentApiError.UserBlocked] = "UserBlocked",
            [AgentApiError.CountryRestricted] = "CountryRestricted",
            [AgentApiError.CurrencyMismatch] = "CurrencyMismatch",
            [AgentApiError.InsufficientFunds] = "InsufficientFunds",
            [AgentApiError.InvalidInvitationCode] = "InvalidInvitationCode",
            [AgentApiError.InvalidGameAction] = "InvalidGameAction",
            [AgentApiError.TransactionIdNotFound] = "TransactionIdNotFound",
            [AgentApiError.DuplicateTransaction] = "DuplicateTransaction",
            [AgentApiError.InvalidAmount] = "InvalidAmount",
            [AgentApiError.LoginFail] = "LoginFail",
            [AgentApiError.InvalidArguments] = "InvalidArguments",
            [AgentApiError.IncorrectFormat] = "IncorrectFormat",
            [AgentApiError.UnderMaintenance] = "UnderMaintenance",
            [AgentApiError.InvalidStatus] = "InvalidStatus",
            [AgentApiError.SystemError] = "SystemError",
            [AgentApiError.OperationNotAllowed] = "OperationNotAllowed",
            [AgentApiError.ProviderApiError] = "ProviderApiError",
            [AgentApiError.ValidateCodeNotVerify] = "ValidateCodeNotVerify",
            [AgentApiError.EmailNotVerify] = "EmailNotVerify",
            [AgentApiError.InsufficientBucket] = "InsufficientBucket",
            [AgentApiError.PromotionCannotJoin] = "PromotionCannotJoin",
            [AgentApiError.GameBalanceFundInFailed] = "GameBalanceFundInFailed",
            [AgentApiError.GameBalanceFundOutFailed] = "GameBalanceFundOutFailed",
            [AgentApiError.FavoriteFull] = "FavoriteAlreadyFull",
            [AgentApiError.UserBanned] = "UserBanned",
            [AgentApiError.BrandEmailDuplicate] = "BrandEmailDuplicate",
            [AgentApiError.DividendConditionError] = "DividendConditionError",
            [AgentApiError.DividendPercentageError] = "DividendPercentageError",
            [AgentApiError.PhoneNumberRequired] = "PhoneNumberRequired",
            [AgentApiError.PlayerExistsError] = "PlayerExistsError"
        };

    private static readonly IReadOnlyDictionary<AgentApiError, int> _HttpCodeByException =
        new Dictionary<AgentApiError, int>
        {
            [AgentApiError.InvalidCredentials] = StatusCodes.Status401Unauthorized,
            [AgentApiError.InvalidOrExpiredAgentToken] = StatusCodes.Status401Unauthorized,
            [AgentApiError.InvalidCaptcha] = StatusCodes.Status401Unauthorized,
            [AgentApiError.ClientIdNotFound] = StatusCodes.Status401Unauthorized,
            [AgentApiError.AgentNotFound] = StatusCodes.Status401Unauthorized,
            [AgentApiError.UserBlocked] = StatusCodes.Status403Forbidden,
            [AgentApiError.CountryRestricted] = StatusCodes.Status403Forbidden,
            [AgentApiError.CurrencyMismatch] = StatusCodes.Status500InternalServerError,
            [AgentApiError.InsufficientFunds] = StatusCodes.Status500InternalServerError,
            [AgentApiError.InvalidInvitationCode] = StatusCodes.Status500InternalServerError,
            [AgentApiError.InvalidGameAction] = StatusCodes.Status500InternalServerError,
            [AgentApiError.TransactionIdNotFound] = StatusCodes.Status500InternalServerError,
            [AgentApiError.DuplicateTransaction] = StatusCodes.Status500InternalServerError,
            [AgentApiError.InvalidAmount] = StatusCodes.Status500InternalServerError,
            [AgentApiError.InvalidArguments] = StatusCodes.Status400BadRequest,
            [AgentApiError.IncorrectFormat] = StatusCodes.Status400BadRequest,
            [AgentApiError.UnderMaintenance] = StatusCodes.Status503ServiceUnavailable,
            [AgentApiError.InvalidStatus] = StatusCodes.Status500InternalServerError,
            [AgentApiError.LoginFail] = StatusCodes.Status400BadRequest,
            [AgentApiError.SystemError] = StatusCodes.Status500InternalServerError,
            [AgentApiError.OperationNotAllowed] = StatusCodes.Status405MethodNotAllowed,
            [AgentApiError.ProviderApiError] = StatusCodes.Status500InternalServerError,
            [AgentApiError.ValidateCodeNotVerify] = StatusCodes.Status406NotAcceptable,
            [AgentApiError.PlayerNotFound] = StatusCodes.Status404NotFound,
            [AgentApiError.EmailNotVerify] = StatusCodes.Status400BadRequest,
            [AgentApiError.InsufficientBucket] = StatusCodes.Status400BadRequest,
            [AgentApiError.PromotionCannotJoin] = StatusCodes.Status400BadRequest,
            [AgentApiError.GameBalanceFundInFailed] = StatusCodes.Status400BadRequest,
            [AgentApiError.GameBalanceFundOutFailed] = StatusCodes.Status400BadRequest,
            [AgentApiError.FavoriteFull] = StatusCodes.Status400BadRequest,
            [AgentApiError.UserBanned] = StatusCodes.Status403Forbidden,
            [AgentApiError.BrandEmailDuplicate] = StatusCodes.Status500InternalServerError,
            [AgentApiError.InvalidPassword] = StatusCodes.Status400BadRequest,
            [AgentApiError.DividendConditionError] = StatusCodes.Status500InternalServerError,
            [AgentApiError.DividendPercentageError] = StatusCodes.Status500InternalServerError,
            [AgentApiError.PhoneNumberRequired] = StatusCodes.Status400BadRequest,
            [AgentApiError.PlayerExistsError] = StatusCodes.Status500InternalServerError
        };

    private static readonly IReadOnlyDictionary<Type, AgentApiError> _TgsApiErrorByException =
        new Dictionary<Type, AgentApiError>
        {
            [typeof(InvalidCredentialsException)] = AgentApiError.InvalidCredentials,
            [typeof(PasswordInvalidException)] = AgentApiError.InvalidPassword,
            [typeof(MissingTokenException)] = AgentApiError.InvalidOrExpiredAgentToken,
            [typeof(AgentLoginExpiredException)] = AgentApiError.InvalidOrExpiredAgentToken,
            [typeof(InvalidCaptchaException)] = AgentApiError.InvalidCaptcha,
            [typeof(ClientIdNotFoundException)] = AgentApiError.ClientIdNotFound,
            [typeof(AgentNotFoundException)] = AgentApiError.AgentNotFound,
            [typeof(PlayerDisabledException)] = AgentApiError.UserBlocked,
            [typeof(CountryRestrictedException)] = AgentApiError.CountryRestricted,
            [typeof(CurrencyNotMatchingException)] = AgentApiError.CurrencyMismatch,
            [typeof(PlayerHasInsufficientFundsException)] = AgentApiError.InsufficientFunds,
            [typeof(InvalidInvitationCodeException)] = AgentApiError.InvalidInvitationCode,
            [typeof(InvalidGameActionException)] = AgentApiError.InvalidGameAction,
            [typeof(InvalidAmountException)] = AgentApiError.InvalidAmount,
            [typeof(ArgumentException)] = AgentApiError.InvalidArguments,
            [typeof(ModelBindingException)] = AgentApiError.InvalidArguments,
            [typeof(FormatException)] = AgentApiError.IncorrectFormat,
            [typeof(BrandUnderMaintenanceException)] = AgentApiError.UnderMaintenance,
            [typeof(GameProviderUnderMaintenanceException)] = AgentApiError.UnderMaintenance,
            [typeof(GameUnderMaintenanceException)] = AgentApiError.UnderMaintenance,
            [typeof(InvalidStatusException)] = AgentApiError.InvalidStatus,
            [typeof(InvalidOperationException)] = AgentApiError.SystemError,
            [typeof(OperationNotAllowedException)] = AgentApiError.OperationNotAllowed,
            [typeof(GameProviderApiErrorException)] = AgentApiError.ProviderApiError,
            [typeof(Services.Agent.Exceptions.AgentNotFoundException)] = AgentApiError.AgentNotFound,
            [typeof(UserBannedException)] = AgentApiError.UserBanned,
            [typeof(DividendConditionException)] = AgentApiError.DividendConditionError,
            [typeof(DividendPercentageException)] = AgentApiError.DividendPercentageError,
            [typeof(ValidateCodeNotVerifyException)] = AgentApiError.ValidateCodeNotVerify,
            [typeof(PhoneRequiredException)] = AgentApiError.PhoneNumberRequired,
            [typeof(PlayerIsExistsException)] = AgentApiError.PlayerExistsError
        };

    public bool CanProcess(Exception exception)
        => true;

    public int GetApiErrorByException(Exception exception)
    {
        if (exception == null)
        {
            return (int)AgentApiError.None;
        }

        var type = exception.GetType();

        return _TgsApiErrorByException.TryGetValue(type, out var error)
            ? (int)error
            : (int)AgentApiError.SystemError;
    }

    public int GetHttpStatusByException(Exception exception)
    {
        if (exception == null)
        {
            return StatusCodes.Status200OK;
        }

        var type = exception.GetType();

        return _TgsApiErrorByException.TryGetValue(type, out var error) &&
            _HttpCodeByException.TryGetValue(error, out var statusCode)
                ? statusCode
                : StatusCodes.Status500InternalServerError;
    }

    public string GetMessageByException(Exception exception)
        => exception.Message;

    public string GetMessageKeyByException(Exception exception)
    {
        var type = exception.GetType();

        if (_TgsApiErrorByException.TryGetValue(type, out var errorType)
            && _ErrorCodeMessageIdMap.TryGetValue(errorType, out var messageKey))
            return messageKey;

        return _ErrorCodeMessageIdMap[AgentApiError.SystemError];
    }
}