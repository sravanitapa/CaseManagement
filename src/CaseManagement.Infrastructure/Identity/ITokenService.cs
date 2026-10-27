using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Infrastructure.Identity.Entities;

namespace CaseManagement.Infrastructure.Identity;

public interface ITokenService
{
    // Builds a signed JWT for the given user and returns it along with its expiry time.
    (string Token,DateTime ExpiresAt) CreateToken(ApplicationUser user, IEnumerable<string> roles);

    // Long-lived random string the client uses to get a new access token.
    string CreateRefreshToken();
}
