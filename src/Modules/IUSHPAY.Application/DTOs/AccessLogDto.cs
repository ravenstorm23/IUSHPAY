using IUSHPAY.Domain.Enums;

namespace IUSHPAY.Application.DTOs;

public record AccessLogDto(
	Guid AccessId,
	Guid UserId,
	string UserFullName,
	string UserCarnet,
	AccessMethod Method,
	bool IsAuthorized,
	DateTime AccessedAt
);