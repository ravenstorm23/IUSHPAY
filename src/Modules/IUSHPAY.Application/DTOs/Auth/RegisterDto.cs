namespace IUSHPAY.Application.DTOs.Auth;

public class RegisterDto
{
    public string InstitutionalCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CarnetNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
