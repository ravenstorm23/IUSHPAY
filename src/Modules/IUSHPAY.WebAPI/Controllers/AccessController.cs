using IUSHPAY.Application.UseCases.Access.GenerateQR;
using IUSHPAY.Application.UseCases.Access.GetLog;
using IUSHPAY.Application.UseCases.Access.ValidateAccess;
using IUSHPAY.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IUSHPAY.WebAPI.Controllers;

[ApiController]
[Route("api/access")]
public class AccessController : ControllerBase
{
	private readonly GenerateQRHandler _qrHandler;
	private readonly ValidateAccessHandler _accessHandler;
	private readonly GetAccessLogHandler _logHandler;

	public AccessController(
		GenerateQRHandler qrHandler,
		ValidateAccessHandler accessHandler,
		GetAccessLogHandler logHandler)
	{
		_qrHandler = qrHandler;
		_accessHandler = accessHandler;
		_logHandler = logHandler;
	}

	/// <summary>Genera QR de acceso para el usuario autenticado</summary>
	[HttpGet("qr/{userId:guid}")]
	[Authorize]
	public async Task<IActionResult> GenerateQR(Guid userId)
	{
		var result = await _qrHandler.HandleAsync(new GenerateQRCommand(userId));
		return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
	}

	/// <summary>Valida QR y descuenta saldo — solo para lectores del parqueadero (Admin)</summary>
	[HttpPost("validate")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Validate([FromBody] AccessRequestDto dto)
	{
		var result = await _accessHandler.HandleAsync(
			new ValidateAccessCommand(dto.Token, dto.Method, dto.Fee));
		return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
	}

	/// <summary>Consulta el log de accesos al parqueadero — solo Admin</summary>
	[HttpGet("log")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> GetLog([FromQuery] DateTime? from, [FromQuery] DateTime? to)
	{
		var result = await _logHandler.HandleAsync(new GetAccessLogQuery(from, to));
		return result.IsSuccess ? Ok(result.Value) : BadRequest(new { message = result.Error });
	}
}

public record AccessRequestDto(string Token, AccessMethod Method, decimal Fee);