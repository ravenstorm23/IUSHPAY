using FluentValidation;
using IUSHPAY.Application.Behaviors;
using IUSHPAY.Application.Common.Interfaces;
using IUSHPAY.Application.Common.Models;
using IUSHPAY.Application.DTOs;
using IUSHPAY.Domain.Entities;
using IUSHPAY.Domain.Exceptions;
using IUSHPAY.Domain.Interfaces.Repositories;
using IUSHPAY.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace IUSHPAY.Application.UseCases.Access.ValidateAccess;

public class ValidateAccessHandler
{
    private readonly IQRValidatorService _qr;
    private readonly IWalletRepository _wallet;
    private readonly IParkingAccessRepository _access;
    private readonly ValidationBehavior<ValidateAccessCommand> _validation;
    private readonly LoggingBehavior<ValidateAccessCommand> _logging;

    public ValidateAccessHandler(
        IQRValidatorService qr,
        IWalletRepository wallet,
        IParkingAccessRepository access,
        IEnumerable<IValidator<ValidateAccessCommand>> validators,
        ILogger<LoggingBehavior<ValidateAccessCommand>> logger,
        ICurrentUserService currentUser)
    {
        _qr = qr;
        _wallet = wallet;
        _access = access;
        _validation = new ValidationBehavior<ValidateAccessCommand>(validators);
        _logging = new LoggingBehavior<ValidateAccessCommand>(logger, currentUser);
    }

    public Task<Result<AccessResultDto>> HandleAsync(ValidateAccessCommand cmd)
        => _logging.LogAndHandleAsync(cmd, c =>
            _validation.ValidateAndHandleAsync(c, ExecuteAsync));

    private async Task<Result<AccessResultDto>> ExecuteAsync(ValidateAccessCommand cmd)
    {
        var userId = await _qr.ValidateAsync(cmd.Token);
        if (userId == null)
            return Result<AccessResultDto>.Failure("QR inválido o expirado");

        var wallet = await _wallet.GetByUserIdAsync(userId.Value);
        if (wallet == null)
            return Result<AccessResultDto>.Failure("Wallet no encontrada");

        try
        {
            wallet.Debit(cmd.Fee, "Acceso parqueadero");
        }
        catch (InsufficientBalanceException ex)
        {
            await _access.AddAsync(ParkingAccess.Record(userId.Value, cmd.Method, false));
            return Result<AccessResultDto>.Failure(ex.Message);
        }

        await _wallet.UpdateAsync(wallet);
        await _access.AddAsync(ParkingAccess.Record(userId.Value, cmd.Method, true));

        return Result<AccessResultDto>.Success(new AccessResultDto(true, wallet.Balance));
    }
}
