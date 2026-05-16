using IUSHPAY.Domain.Enums;

namespace IUSHPAY.Application.DTOs;

public record AccessLogDto(
	Guid AccessId,
	Guid UserId,
	string UserFullName,
	AccessMethod Method,
	bool IsAuthorized,
	DateTime AccessedAt
);