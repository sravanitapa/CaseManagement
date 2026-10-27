using System;
using System.Collections.Generic;
using System.Text;

namespace CaseManagement.Infrastructure.Identity;

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    //Access tokens are short-lived;refresh tokens live much longer.
    public int ExpiryMinutes { get; set; }
    public int RefreshTokenDays { get; set; }
}
