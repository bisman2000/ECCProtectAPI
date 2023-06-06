using System.Security.Cryptography;

namespace AT150732.WebAPI.Helper
{
    public static class ECDSABuilder
    {
        public static ECDsa LoadFromHex()
        {
            var pub = "3059301306072A8648CE3D020106082A8648CE3D030107034200048D1AC4E5F9236F66931CB02A02039561C40F0493C2FEEF081793C7E27DE6EF8C647C0C802547C973BE1DA6DAFEB2A387E024C63523E9EB6556101273D3549D81";
            var priv = "307702010104204076D30CCADA4F9C257155B6225700DEC9D1006C1A2D0ACF552BF7FCC524B2DAA00A06082A8648CE3D030107A14403420004E409F3DA29E6B18FB0284FF789CBEA9D1C6D168EBE0719DE8E9042BF9E52753F3B00174909AF2459113C6EA5109A13783024EEA4E79663633CAACFCFCA6E7472";
            var ecd = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            ecd.ImportSubjectPublicKeyInfo(Convert.FromHexString(pub),out _);
            ecd.ImportECPrivateKey(Convert.FromHexString(priv), out _);
            return ecd;
        }

    }
}
/* var privateKey = "c711e5080f2b58260fe19741a7913e8301c1128ec8e80b8009406e5047e6e1ef";
            var publicKey = "04e33993f0210a4973a94c26667007d1b56fe886e8b3c2afdd66aa9e4937478ad20acfbdc666e3cec3510ce85d40365fc2045e5adb7e675198cf57c6638efa1bdb";
            var privateKeyBytes = Convert.FromHexString(privateKey);
            var publicKeyBytes = Convert.FromHexString(publicKey);
            return ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                D = privateKeyBytes,
                Q = new ECPoint
                {
                    //skip first byte cuz is indicated is compressed or not
                    //base on RFC 5480 https://www.rfc-editor.org/rfc/rfc5480#section-2.2
                    // 0x04 mean uncompressed, 0x03 and 0x04 mean compress
                    // 256 bit = 32 bytes
                    // public key store X and y each 32 bytes => both is 64 
                    X = publicKeyBytes.Skip(1).Take(32).ToArray(),
                    Y = publicKeyBytes.Skip(33).ToArray()
                }
            });*/