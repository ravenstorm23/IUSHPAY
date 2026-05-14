using Microsoft.AspNetCore.Mvc;
using IUSHPAY.Application.UseCases.Payment.Webhook;

namespace IUSHPAY.WebAPI.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
	private readonly ProcessPSEWebhookHandler _handler;

	public PaymentController(ProcessPSEWebhookHandler handler)
	{
		_handler = handler;
	}

	[HttpPost("webhook")]
	public async Task<IActionResult> Webhook([FromBody] WebhookDto dto)
	{
		var signature = Request.Headers["X-Signature"].ToString();

		var result = await _handler.HandleAsync(
			new ProcessPSEWebhookCommand(
				dto.Payload,
				signature,
				dto.WalletId,
				dto.Amount));

		return result.IsSuccess ? Ok() : BadRequest(result.Error);
	}
}

public record WebhookDto(string Payload, Guid WalletId, decimal Amount);