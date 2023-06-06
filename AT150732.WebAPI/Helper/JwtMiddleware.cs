using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AT150732.WebAPI.Helper;

public class JwtMiddleware
{
    private readonly RequestDelegate next;
    private readonly IConfiguration configuration;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        this.next = next;
        this.configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Cookies["X-Access-Token"];
        var userName = context.Request.Cookies["X-Username"];
        if (userName != null && token != null)
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
                 TokenDecryptionKey = encKey,
                 ClockSkew = TimeSpan.Zero
             });
            if (!result.IsValid) throw new SecurityTokenException("Invalid token");
            await next(context);
        }




    }
}
