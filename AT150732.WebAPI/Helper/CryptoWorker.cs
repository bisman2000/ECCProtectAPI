using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace AT150732.WebAPI.Helper;

public static class CryptoWorker
{
    public static string GetPublicKey(string privatekey)
    {
        var cngKey = CngKey.Import(Convert.FromBase64String(privatekey), CngKeyBlobFormat.EccPrivateBlob);
        // when convert to base64, the string will contain special character like +,/ which will be url encode
        // + -> %2b, / -> %2f when response back to app. So to avoid it, the key will convert to hexString
        return Convert.ToHexString(cngKey.Export(CngKeyBlobFormat.EccPublicBlob));
    }
    public static string EncryptData(string data, string privateKey, string Iv, string secret, string token)
    {
        var clientKey = LoadClientKey(secret, token);
        var key = Convert.FromHexString(clientKey);
        var ecdh = LoadECD(privateKey);
        ecdh.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
        ecdh.HashAlgorithm = CngAlgorithm.Sha256;
        var sharedKey = ecdh.DeriveKeyMaterial(CngKey.Import(key, CngKeyBlobFormat.EccPublicBlob));
        var aes = LoadAES(sharedKey, Iv);
        var enc = aes.EncryptCbc(Encoding.Default.GetBytes(data), aes.IV);
        return Convert.ToBase64String(enc);
    }
    public static string DecryptData(string data, string privateKey, string Iv, string secret, string token)
    {
        var clientKey = LoadClientKey(secret, token);
        var key = Convert.FromHexString(clientKey);
        var ecdh = LoadECD(privateKey);
        ecdh.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
        ecdh.HashAlgorithm = CngAlgorithm.Sha256;
        var sharedKey = ecdh.DeriveKeyMaterial(CngKey.Import(key, CngKeyBlobFormat.EccPublicBlob));
        var aes = LoadAES(sharedKey, Iv);
        var de = aes.DecryptCbc(Convert.FromBase64String(data), aes.IV);
        return Encoding.Default.GetString(de);
    }
    private static Aes LoadAES(byte[] secretKey, string Iv)
    {
        var aes = Aes.Create();
        aes.Key = secretKey;
        aes.IV = Convert.FromBase64String(Iv);
        return aes;
    }
    private static ECDiffieHellmanCng LoadECD(string privatekey)
    {
        var cngKey = CngKey.Import(Convert.FromBase64String(privatekey), CngKeyBlobFormat.EccPrivateBlob);

        var ecdh = new ECDiffieHellmanCng(cngKey);
        return ecdh;
    }
    private static string LoadClientKey(string secret, string token)
    {
        var ecc = ECDSABuilder.LoadFromHex();
        var encKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
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
        var clientKey = claims.Claims.FirstOrDefault(x => x.Type == "clientKey").Value;
        return clientKey;
    }
}

//    //45434B3120000000C7A872EF3642901BD7D2AC229E2E2773D9685B73E8856D1D55945FCF7D7D48993C520F03C84BD8982D66E3FEC746F17E4989603A90EA4FC031E04197E29B0F5D
