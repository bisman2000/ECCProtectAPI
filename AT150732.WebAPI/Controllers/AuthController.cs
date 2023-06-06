using AT150732.Common.Model;
using AT150732.WebAPI.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AT150732.WebAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> userManager;

    private readonly IConfiguration configuration;

    public AuthController(UserManager<AppUser> userManager, IConfiguration configuration)
    {
        this.userManager = userManager;
        this.configuration = configuration;
    }
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username);
        if (model.publicKey != null && user != null && await userManager.CheckPasswordAsync(user, model.Password))
        {
            var authClaims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //save client public key
                new Claim("clientKey", model.publicKey)
            });
            var token = CreateToken(authClaims);
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await userManager.UpdateAsync(user);
            //get public key and store it inside cookie
            var publicServerKey = CryptoWorker.GetPublicKey(configuration["ECDHPrivkey"]);

            // set token in cookie with httpOnly flag avoid xss (prevent script-clientside),
            // SameSite flag avoid xsrf (only same site access), Secure flag to only transmit in TLS and IsEssential flag indicate is important
            Response.Cookies.Append("X-PublicKey", publicServerKey, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, IsEssential = true });
            Response.Cookies.Append("X-Access-Token", token,
                new CookieOptions()
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Secure = true,
                    IsEssential = true
                });
            Response.Cookies.Append("X-Username", user.UserName, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, IsEssential = true });
            Response.Cookies.Append("X-Refresh-Token", user.RefreshToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, IsEssential = true });
            return Ok();
        }
        return Unauthorized();
    }
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] SignupViewModel model)
    {
        var userExists = await userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
        AppUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };
        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        return Ok(new Response { Status = "Success", Message = "User created successfully!" });
    }

    [HttpGet]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        if (!(Request.Cookies.TryGetValue("X-Username", out var userName)
            && Request.Cookies.TryGetValue("X-Refresh-Token", out var refreshToken)
             && Request.Cookies.TryGetValue("X-Access-Token", out var accessToken)
             && Request.Cookies.TryGetValue("X-PublicKey", out var publicKey)))
        {
            return BadRequest("Invalid client request");
        }

        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal == null) return BadRequest("Invalid access token or refresh token");

        var user = await userManager.FindByNameAsync(userName);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return BadRequest("Invalid access token or refresh token");
        }

        var newAccessToken = CreateToken(principal);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await userManager.UpdateAsync(user);
        //get public key and store it inside cookie
        Response.Cookies.Append("X-PublicKey", publicKey, new CookieOptions()
        { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, IsEssential = true });
        Response.Cookies.Append("X-Access-Token", newAccessToken, new CookieOptions()
        { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, IsEssential = true });
        Response.Cookies.Append("X-Username", user.UserName, new CookieOptions()
        { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, IsEssential = true });
        Response.Cookies.Append("X-Refresh-Token", user.RefreshToken, new CookieOptions()
        { HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = true, IsEssential = true });

        return Ok();
    }
    private string CreateToken(ClaimsIdentity authClaims)
    {
        var ecc = ECDSABuilder.LoadFromHex();
        var handler = new JsonWebTokenHandler();
        string token = handler.CreateToken(new SecurityTokenDescriptor
        {
            Expires = DateTime.Now.AddMinutes(5),
            Subject = authClaims,
            SigningCredentials = new SigningCredentials(new ECDsaSecurityKey(ecc), SecurityAlgorithms.EcdsaSha256),
            EncryptingCredentials = new EncryptingCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Secret"])),
            SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256)
        });

        return token;
    }


    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsIdentity? GetPrincipalFromExpiredToken(string? token)
    {
        var ecc = ECDSABuilder.LoadFromHex();
        var encKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Secret"]));
        var handler = new JsonWebTokenHandler();
        TokenValidationResult result = handler.ValidateToken(token,

         new TokenValidationParameters
         {
             ValidateAudience = false,
             ValidateIssuer = false,
             ValidateIssuerSigningKey = true,
             IssuerSigningKey = new ECDsaSecurityKey(ecc),
             ValidateLifetime = false,
             TokenDecryptionKey = encKey
         });
        if (!result.IsValid) throw new SecurityTokenException("Invalid token");
        var claims = result.ClaimsIdentity;
        return claims;

    }
}
