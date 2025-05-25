using System.Security.Cryptography;

namespace BlazeGate.Common
{
    public class RsaParametersHelper
    {
        public static string RsaParametersToPem(RSAParameters parameters, bool includePrivate)
        {
            using var rsa = RSA.Create();
            rsa.ImportParameters(parameters);

            if (includePrivate)
            {
                // 导出PKCS#8私钥
                var privateKeyBytes = rsa.ExportPkcs8PrivateKey();
                return new string(PemEncoding.Write("PRIVATE KEY", privateKeyBytes));
            }
            else
            {
                // 导出SubjectPublicKeyInfo公钥
                var publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
                return new string(PemEncoding.Write("PUBLIC KEY", publicKeyBytes));
            }
        }

        public static RSAParameters PemToRsaParameters(string pem)
        {
            using var rsa = RSA.Create();
            if (pem.Contains("PRIVATE KEY"))
            {
                // 解析PKCS#8私钥
                var privateKeyBytes = DecodePemContent(pem);
                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

                return rsa.ExportParameters(true);
            }
            else if (pem.Contains("PUBLIC KEY"))
            {
                // 解析SubjectPublicKeyInfo公钥
                var publicKeyBytes = DecodePemContent(pem);
                rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

                return rsa.ExportParameters(false);
            }
            else
            {
                throw new ArgumentException("PEM格式无效");
            }
        }

        private static byte[] DecodePemContent(string pem)
        {
            // 使用Base64解码PEM内容
            var lines = pem.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var base64Content = string.Join("", lines.Where(line => !line.StartsWith("-----")));
            return Convert.FromBase64String(base64Content);
        }
    }
}