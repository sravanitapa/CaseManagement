using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Infrastructure.Identity.Entities;

namespace CaseManagement.Infrastructure.Identity;

public interface ITokenService
{
    // Builds a signed JWT for the given user and returns it along with its expiry time.
    (string Token,DateTime ExpiresAt) CreateToken(ApplicationUser user, IEnumerable<string> roles);
}
