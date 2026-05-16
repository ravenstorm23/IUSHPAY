using IUSHPAY.Application.Common.Interfaces;
using IUSHPAY.Application.UseCases.Access.GenerateQR;
using IUSHPAY.Application.UseCases.Access.GetLog;
using IUSHPAY.Application.UseCases.Access.ValidateAccess;
using IUSHPAY.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IUSHPAY.WebAPI.Controllers;

[ApiController]
[Route("api/access")]
[Authorize]
public class AccessController : ControllerBase
{
	private readonly GenerateQRHandler _qrHandler;
	private readonly ValidateAccessHandler _accessHandler;
	private readonly GetAccessLogHandler _logHandler;
	private readonly ICurrentUserService _currentUser;

	public AccessController(
		GenerateQRHandler qrHandler,
		ValidateAccessHandler accessHandler,
		GetAccessLogHandler logHandler,
		ICurrentUserService currentUser)
	{
		_qrHandler = qrHandler;
		_accessHandler = accessHandler;
		_logHandler = logHandler;
		_currentUser = currentUser;
	}

	/// <summary>Genera QR de acceso — usa el userId del token automáticamente</summary>
	[HttpGet("qr")]
	public async Task<IActionResult> GenerateQR()
	{
		// FIX: userId viene del token, no de la URL
		var userId = _currentUser.UserId;
		if (userId == Guid.Empty) return Unauthorized();

		var result = await _qrHandler.HandleAsync(new GenerateQRCommand(userId));
		return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
	}

	/// <summary>Valida QR y descuenta saldo — solo Admin</summary>
	[HttpPost("validate")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Validate([FromBody] AccessRequestDto dto)
	{
		var result = await _accessHandler.HandleAsync(
			new ValidateAccessCommand(dto.Token, dto.Method, dto.Fee));
		return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
	}

	/// <summary>Log de accesos al parqueadero — solo Admin</summary>
	[HttpGet("log")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> GetLog([FromQuery] DateTime? from, [FromQuery] DateTime? to)
	{
		var result = await _logHandler.HandleAsync(new GetAccessLogQuery(from, to));
		return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
	}
}

public record AccessRequestDto(string Token, AccessMethod Method, decimal Fee);