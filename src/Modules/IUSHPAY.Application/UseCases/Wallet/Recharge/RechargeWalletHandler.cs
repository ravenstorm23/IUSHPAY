using FluentValidation;
using IUSHPAY.Application.Behaviors;
using IUSHPAY.Application.Common.Interfaces;
using IUSHPAY.Application.Common.Models;
using IUSHPAY.Domain.Interfaces.Repositories;
using IUSHPAY.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace IUSHPAY.Application.UseCases.Wallet.Recharge;

public class RechargeWalletHandler
{
    private readonly IWalletRepository _repo;
    private readonly IPaymentGatewayService _pse;
    private readonly ValidationBehavior<RechargeWalletCommand> _validation;
    private readonly LoggingBehavior<RechargeWalletCommand> _logging;

    public RechargeWalletHandler(
        IWalletRepository repo,
        IPaymentGatewayService pse,
        IEnumerable<IValidator<RechargeWalletCommand>> validators,
        ILogger<LoggingBehavior<RechargeWalletCommand>> logger,
        ICurrentUserService currentUser)
    {
        _repo = repo;
        _pse = pse;
        _validation = new ValidationBehavior<RechargeWalletCommand>(validators);
        _logging = new LoggingBehavior<RechargeWalletCommand>(logger, currentUser);
    }

    public Task<Result<string>> HandleAsync(RechargeWalletCommand cmd)
        => _logging.LogAndHandleAsync(cmd, c =>
            _validation.ValidateAndHandleAsync(c, ExecuteAsync));

    private async Task<Result<string>> ExecuteAsync(RechargeWalletCommand cmd)
    {
        var wallet = await _repo.GetByUserIdAsync(cmd.UserId);
        if (wallet == null)
            return Result<string>.Failure("Wallet no encontrada");

        var url = await _pse.InitiateAsync(wallet.Id, cmd.Amount);
        return Result<string>.Success(url);
    }
}
