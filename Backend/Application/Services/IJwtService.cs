using Backend.Core.Entities;

namespace Backend.Application.Services;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
    int? ValidateToken(string token);
}
