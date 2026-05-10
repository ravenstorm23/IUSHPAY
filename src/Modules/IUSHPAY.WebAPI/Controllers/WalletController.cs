using IUSHPAY.Application.UseCases.Wallet.GetBalance;
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

    public WalletController(GetBalanceHandler balanceHandler, RechargeWalletHandler rechargeHandler)
    {
        _balanceHandler = balanceHandler;
        _rechargeHandler = rechargeHandler;
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetBalance(Guid userId)
    {
        var result = await _balanceHandler.HandleAsync(new GetBalanceQuery(userId));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }

    [HttpPost("recharge")]
    public async Task<IActionResult> Recharge([FromBody] RechargeWalletCommand cmd)
    {
        var result = await _rechargeHandler.HandleAsync(cmd);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
    }
}
