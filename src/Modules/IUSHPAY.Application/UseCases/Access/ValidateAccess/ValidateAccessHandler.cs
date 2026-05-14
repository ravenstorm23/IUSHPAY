using IUSHPAY.Application.Common.Models;
using IUSHPAY.Application.DTOs;
using IUSHPAY.Domain.Entities;
using IUSHPAY.Domain.Exceptions;
using IUSHPAY.Domain.Interfaces.Repositories;
using IUSHPAY.Domain.Interfaces.Services;

namespace IUSHPAY.Application.UseCases.Access.ValidateAccess;

public class ValidateAccessHandler
{
    private readonly IQRService _qr;
    private readonly IWalletRepository _wallet;
    private readonly IParkingAccessRepository _access;

    public ValidateAccessHandler(
        IQRService qr,
        IWalletRepository wallet,
        IParkingAccessRepository access)
    {
        _qr = qr;
        _wallet = wallet;
        _access = access;
    }

    public async Task<Result<AccessResultDto>> HandleAsync(ValidateAccessCommand cmd)
    {
        var userId = await _qr.ValidateAsync(cmd.Token);
        if (userId == null)
            return Result<AccessResultDto>.Failure("QR inválido");

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
