using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace ImpostorHqR.Core.Web.Common.Cryptography.Ssl
{
    public class SslCertificate
    {
        public X509Certificate2 Certificate { get; set; }

        public byte[] PfxPrivateBytes { get; set; }

        public X509Certificate2 GetHttpsCert()
        {
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(@"dimaHttps.pfx", FileMode.Create)))
            {
                binWriter.Write(PfxPrivateBytes);
            }

            File.WriteAllText("add-to-browser.cer",
                $"-----BEGIN CERTIFICATE-----" +
                $"\r\n{Convert.ToBase64String(Certificate.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks)}" +
                $"\r\n-----END CERTIFICATE-----");
            return new X509Certificate2(@"dimaHttps.pfx");
        }
    }
}
