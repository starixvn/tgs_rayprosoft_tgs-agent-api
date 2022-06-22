using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TGS.Apis.TgsApi.GameProviders.AllBet.Exceptions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Interfaces;
using static TGS.Core.Models.Enums.InvalidGameActionErrorCode;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services;

public class JiliErrorMapper : IErrorMapper
{
    /// <summary>
    /// // auth
    /// Success = 0,
    /// Token expired = 4,
    /// Other error = 5
    ///
    /// // bet
    /// Success = 0,
    /// Already accepted = 1,
    /// Not enough balance = 2,
    /// Invalid parameter = 3,
    /// Token expired = 4,
    /// Other error = 5
    ///
    /// // cancelBet
    /// Success = 0,
    /// Already canceled = 1,
    /// Round not found = 2,
    /// Invalid parameter = 3,
    /// Other error = 5
    /// Already accepted and can not be canceled = 6
    /// </summary>

    private static readonly IReadOnlyDictionary<Type, int> _ApiErrorByException =
        new Dictionary<Type, int>
        {
            [typeof(InvalidOperatorIdException)] = 5,
            [typeof(InvalidCredentialsException)] = 5,
            [typeof(MissingTokenException)] = 5,
            [typeof(PlayerLoginExpiredException)] = 4,
            [typeof(PlayerNotFoundException)] = 5,
            [typeof(PlayerDisabledException)] = 5,
            [typeof(CountryRestrictedException)] = 5,
            [typeof(CurrencyNotMatchingException)] = 5,
            [typeof(PlayerHasInsufficientFundsException)] = 2,
            [typeof(InvalidGameActionException)] = 5,
            [typeof(InvalidAmountException)] = 5,
            [typeof(ArgumentException)] = 3,
            [typeof(ModelBindingException)] = 3,
            [typeof(FormatException)] = 3,
            [typeof(BrandUnderMaintenanceException)] = 5,
            [typeof(GameProviderUnderMaintenanceException)] = 5,
            [typeof(GameUnderMaintenanceException)] = 5,
            [typeof(InvalidStatusException)] = 5,
            [typeof(InvalidOperationException)] = 5
        };

    private static readonly IReadOnlyDictionary<InvalidGameActionErrorCode, int>
        _ApiErrorByInvalidGameActionErrorCode =
            new Dictionary<InvalidGameActionErrorCode, int>
            {
                [TransactionNotFound] = 5,
                [TransactionBeingCancelledNotFound] = 5,
                [TransactionBeingReferencedNotFound] = 5,
                [RoundNotFound] = 2,
                [Cancelled] = 1,
                [Settled] = 1,
                [Duplicate] = 1,
                [ActionProhibited] = 5
            };

    private readonly ILogger<JiliErrorMapper> _logger;

    public JiliErrorMapper(
        ILogger<JiliErrorMapper> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public int GetApiErrorByException(Exception exception)
    {
        if (exception == null)
        {
            return 0;
        }

        if (exception is InvalidGameActionException e &&
            _ApiErrorByInvalidGameActionErrorCode.TryGetValue(e.ErrorCode, out var error))
        {
            return error;
        }

        var type = exception.GetType();
        return _ApiErrorByException.TryGetValue(type, out error) ? error : 5;
    }

    public int GetHttpStatusByException(Exception exception)
        => StatusCodes.Status200OK;

    public string GetMessageByException(Exception exception)
        => exception.Message;
}
