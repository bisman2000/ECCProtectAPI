using System.Security.Cryptography;
using System.Text;

namespace AT150732.WebApp;

public static class CryptoWorker
{
    public static string GetPublicKey(string privatekey)
    {
        var cngKey = CngKey.Import(Convert.FromBase64String(privatekey), CngKeyBlobFormat.EccPrivateBlob);

        return Convert.ToHexString(cngKey.Export(CngKeyBlobFormat.EccPublicBlob));
    }
    public static string GeneratePrivateKey()
    {
        var cngKey = CngKey.Create(CngAlgorithm.ECDiffieHellmanP256, null,
               new CngKeyCreationParameters
               {
                   ExportPolicy = CngExportPolicies.AllowPlaintextExport
               });
        var privatekey = cngKey.Export(CngKeyBlobFormat.EccPrivateBlob);
        return Convert.ToBase64String(privatekey);
    }
    public static string EncryptData(string data, string privateKey, string Iv, string serverKey)
    {
        var key = Convert.FromHexString(serverKey);
        var ecdh = LoadECD(privateKey);
        ecdh.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
        ecdh.HashAlgorithm = CngAlgorithm.Sha256;
        var sharedKey = ecdh.DeriveKeyMaterial(CngKey.Import(key, CngKeyBlobFormat.EccPublicBlob));
        var aes = LoadAES(sharedKey, Iv);
        var enc = aes.EncryptCbc(Encoding.Default.GetBytes(data), aes.IV);
        return Convert.ToBase64String(enc);
    }
    public static string DecryptData(string data, string serverKey, string privateKey, string Iv)
    {
        var key = Convert.FromHexString(serverKey);
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
}
