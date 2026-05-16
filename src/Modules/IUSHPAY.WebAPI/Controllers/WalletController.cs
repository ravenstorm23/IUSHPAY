using IUSHPAY.Application.Common.Interfaces;
using IUSHPAY.Application.UseCases.Wallet.GetBalance;
using IUSHPAY.Application.UseCases.Wallet.GetHistory;
using IUSHPAY.Application.UseCases.Wallet.Recharge;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IUSHPAY.WebAPI.Controllers;

[ApiController]
[Route("api/wallet")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly GetBalanceHandler _balanceHandler;
    private readonly RechargeWalletHandler _rechargeHandler;
    private readonly GetTransactionHistoryHandler _historyHandler;
    private readonly ICurrentUserService _currentUser;

    public WalletController(
        GetBalanceHandler balanceHandler,
        RechargeWalletHandler rechargeHandler,
        GetTransactionHistoryHandler historyHandler,
        ICurrentUserService currentUser)
    {
        _balanceHandler = balanceHandler;
        _rechargeHandler = rechargeHandler;
        _historyHandler = historyHandler;
        _currentUser = currentUser;
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetBalance(Guid userId)
    {
        if (_currentUser.UserId != userId && _currentUser.Role != "Admin")
            return Forbid();

        var result = await _balanceHandler.HandleAsync(new GetBalanceQuery(userId));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPost("recharge")]
    public async Task<IActionResult> Recharge([FromBody] RechargeWalletCommand cmd)
    {
        if (_currentUser.UserId != cmd.UserId && _currentUser.Role != "Admin")
            return Forbid();

        var result = await _rechargeHandler.HandleAsync(cmd);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpGet("{userId:guid}/history")]
    public async Task<IActionResult> GetHistory(Guid userId)
    {
        if (_currentUser.UserId != userId && _currentUser.Role != "Admin")
            return Forbid();

        var result = await _historyHandler.HandleAsync(new GetTransactionHistoryQuery(userId));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }
}
