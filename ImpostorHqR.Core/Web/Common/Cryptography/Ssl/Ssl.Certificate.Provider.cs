using ImpostorHqR.Core.Web.Common.Cryptography.Ssl;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace HqResearch.Ssl
{
    public class SslCertificateProvider
    {
        public static SslCertificateProvider Instance = new SslCertificateProvider();

        public static X509Certificate2 GetCertificate(string ipDns)
        {
            if (!File.Exists("dimaHttps.pfx"))
            {
                var ssc = MakeCert(ipDns);
                return ssc.GetHttpsCert();
            }
            return new X509Certificate2("dimaHttps.pfx");
        }

        private static SslCertificate MakeCert(string cn)
        {
            var asymmetricPair = ECDsa.Create(ECCurve.NamedCurves.nistP384);
            var request = new CertificateRequest($"CN={cn}", asymmetricPair, HashAlgorithmName.SHA256);
            var ssc = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(3141));
            return new SslCertificate()
            {
                Certificate = ssc,
                PfxPrivateBytes = ssc.Export(X509ContentType.Pfx),
            };
        }

    }
}
