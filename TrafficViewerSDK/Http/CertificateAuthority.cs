/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
 */
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TrafficViewerSDK.Http
{
    /// <summary>
    /// Responsible for dynamic certificate generation
    /// </summary>
    public class CertificateAuthority
    {
        const string CA_NAME = "CN=Http Black Ops";
        private static object _lock = new object();
        private static AsymmetricKeyParameter _caPrivKey;
        private static Dictionary<string, X509Certificate2> _dictionary = new Dictionary<string,X509Certificate2>();

        /// <summary>
        /// Gets a certificate for the hostname signed by the blackops ca
        /// </summary>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public static X509Certificate2 GetBlackOpsCert(string hostName)
        {
            string subject = String.Format("CN={0}, OU=Security, O=Company, L=Ottawa, C=CA", hostName);
            return GetBlackOpsCertWithCustomSubject(subject);
           
        }

        public static X509Certificate2 GetBlackOpsCertWithCustomSubject(string subject)
        {
            lock (_lock)
            {
                if (String.IsNullOrWhiteSpace(subject)) return null;
                if (_dictionary.ContainsKey(subject))
                {
                    return _dictionary[subject];
                }
                if (_caPrivKey == null)
                {
                    if (String.IsNullOrEmpty(SdkSettings.Instance.CAPrivateKey))
                    {
                        throw new Exception("Black Ops CA is not configured");
                    }
                    else
                    {
                        try
                        {
                            _caPrivKey = PrivateKeyFactory.CreateKey(Convert.FromBase64String(SdkSettings.Instance.CAPrivateKey));
                        }
                        catch (Exception ex)
                        {
                            SdkSettings.Instance.Logger.Log(System.Diagnostics.TraceLevel.Error, "Error loading private key: {0}", ex.Message);
                            throw new Exception("Could not load private key. Reconfigure Black Ops CA.");
                        }
                    }

                }
                var cert = GenerateCertificate(subject, CA_NAME, _caPrivKey);
                _dictionary.Add(subject, cert);
                return cert;
            }
        }

        /// <summary>
        /// Generates a ca certificate
        /// </summary>
        /// <param name="privateKey">The CA private key used to sign certificates</param>
        /// <param name="base64EncodedCer">The cer file used to configure the browser</param>
        /// <returns></returns>
        public static void GenerateCACert(out string privateKey, out string base64EncodedCer)
        {
            string subjectName = CA_NAME; 
            int keyStrength = 1024;

            // Generating Random Numbers
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);

            // The Certificate Generator
            var certificateGenerator = new X509V3CertificateGenerator();

            // Serial Number
            var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);


            // Issuer and Subject Name
            var subjectDN = new X509Name(subjectName);
            var issuerDN = subjectDN;
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);

            // Valid For
            var notBefore = DateTime.UtcNow.Date;
            var notAfter = notBefore.AddYears(2);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);
            certificateGenerator.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(0));

            // Subject Public Key
            AsymmetricCipherKeyPair subjectKeyPair;
            var keyGenerationParameters = new KeyGenerationParameters(random, keyStrength);
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            certificateGenerator.SetPublicKey(subjectKeyPair.Public);
            

            // Generating the Certificate
            var issuerKeyPair = subjectKeyPair;
            ISignatureFactory signatureFactory = new Asn1SignatureFactory("SHA256WithRSA", issuerKeyPair.Private, random);
            var certificate = certificateGenerator.Generate(signatureFactory);
        

            base64EncodedCer = String.Format("-----BEGIN CERTIFICATE-----\r\n{0}\r\n-----END CERTIFICATE-----", 
                Convert.ToBase64String(certificate.GetEncoded()));

            //var x509 = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificate.GetEncoded());
            //System.Security.Cryptography.X509Certificates.X509Certificate dotNetCert = DotNetUtilities.ToX509Certificate(certificate);
            //X509Certificate2 dotNetCert2 = new X509Certificate2(dotNetCert);
            // Add CA certificate to Root store
            //AddCertToStore(dotNetCert2, StoreName.Root, StoreLocation.LocalMachine);

            PrivateKeyInfo info = PrivateKeyInfoFactory.CreatePrivateKeyInfo(issuerKeyPair.Private);
            privateKey = Convert.ToBase64String(info.GetEncoded());

        }

        /// <summary>
        /// Adds a certificate to the specified store
        /// </summary>
        /// <param name="cert"></param>
        /// <param name="st"></param>
        /// <param name="sl"></param>
        /// <returns></returns>
        public static bool AddCertToStore(X509Certificate2 cert, System.Security.Cryptography.X509Certificates.StoreName st, System.Security.Cryptography.X509Certificates.StoreLocation sl)
        {
            bool bRet = false;

            try
            {
                X509Store store = new X509Store(st, sl);
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);

                store.Close();
            }
            catch
            {

            }

            return bRet;
        }


        /// <summary>
        /// Generates a certificate signed by the CA
        /// </summary>
        /// <param name="subjectName"></param>
        /// <param name="issuerName"></param>
        /// <param name="issuerPrivKey"></param>
        /// <param name="keyStrength"></param>
        /// <returns></returns>
        public static X509Certificate2 GenerateCertificate(string subjectName, string issuerName, AsymmetricKeyParameter issuerPrivKey, int keyStrength = 1024)
        {
            // Generating Random Numbers
            CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
            SecureRandom random = new SecureRandom(randomGenerator);
            ISignatureFactory signatureFactory = new Asn1SignatureFactory("SHA256WITHRSA", issuerPrivKey, random);
            // The Certificate Generator
            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();
           
            // Serial Number
            BigInteger serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);

           
            // Issuer and Subject Name
            X509Name subjectDN = new X509Name(subjectName);
            X509Name issuerDN = new X509Name(issuerName);
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);

            // Valid For
            DateTime notBefore = DateTime.UtcNow.Date;
            DateTime notAfter = notBefore.AddYears(2);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);

            // Subject Public Key
            AsymmetricCipherKeyPair subjectKeyPair;
            var keyGenerationParameters = new KeyGenerationParameters(random, keyStrength);
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            certificateGenerator.SetPublicKey(subjectKeyPair.Public);
            // Generating the Certificate
            AsymmetricCipherKeyPair issuerKeyPair = subjectKeyPair;

            // selfsign certificate
            Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate(signatureFactory);

            // correcponding private key
            PrivateKeyInfo info = PrivateKeyInfoFactory.CreatePrivateKeyInfo(subjectKeyPair.Private);


            // merge into X509Certificate2
            X509Certificate2 x509 = new X509Certificate2(certificate.GetEncoded());

            Asn1Sequence seq = (Asn1Sequence)Asn1Object.FromByteArray(info.ParsePrivateKey().GetDerEncoded());
            if (seq.Count != 9)
            {
                throw new PemException("malformed sequence in RSA private key");
            }

            RsaPrivateKeyStructure rsa = RsaPrivateKeyStructure.GetInstance(seq);
            RsaPrivateCrtKeyParameters rsaparams = new RsaPrivateCrtKeyParameters(
                rsa.Modulus, rsa.PublicExponent, rsa.PrivateExponent, rsa.Prime1, rsa.Prime2, rsa.Exponent1, rsa.Exponent2, rsa.Coefficient);

            x509.PrivateKey = DotNetUtilities.ToRSA(rsaparams);
            return x509;
        }

    }
}
