

namespace AT150732.Common.Model;

public class AppUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string? ECDHPrivateKey { get; set; }
}
