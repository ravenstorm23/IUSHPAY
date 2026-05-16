using IUSHPAY.Application.Common.Interfaces;
using IUSHPAY.Application.UseCases.Wallet.GetBalance;
using IUSHPAY.Application.UseCases.Wallet.GetHistory;
using IUSHPAY.Application.UseCases.Wallet.Recharge;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IUSHPAY.WebAPI.Controllers;

[ApiController]
[Route("api/wallet")]
//[Authorize]
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

	/// <summary>Consulta el saldo — usa el userId del token automáticamente</summary>
	[HttpGet("balance")]
	public async Task<IActionResult> GetBalance()
	{
		// FIX: userId viene del token, no de la URL
		var userId = _currentUser.UserId;
		if (userId == Guid.Empty) return Unauthorized();

		var result = await _balanceHandler.HandleAsync(new GetBalanceQuery(userId));
		return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
	}

	/// <summary>Recarga saldo — usa el userId del token automáticamente</summary>
	[HttpPost("recharge")]
	public async Task<IActionResult> Recharge([FromBody] RechargeRequest request)
	{
		// FIX: userId viene del token, no del body
		var userId = _currentUser.UserId;
		if (userId == Guid.Empty) return Unauthorized();

		var cmd = new RechargeWalletCommand(userId, request.Amount);
		var result = await _rechargeHandler.HandleAsync(cmd);
		return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
	}

	/// <summary>Historial de transacciones — usa el userId del token automáticamente</summary>
	[HttpGet("history")]
	public async Task<IActionResult> GetHistory()
	{
		// FIX: userId viene del token, no de la URL
		var userId = _currentUser.UserId;
		if (userId == Guid.Empty) return Unauthorized();

		var result = await _historyHandler.HandleAsync(new GetTransactionHistoryQuery(userId));
		return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
	}
}

/// <summary>Solo pide el monto — el userId lo toma el servidor del token JWT</summary>
public record RechargeRequest(decimal Amount);