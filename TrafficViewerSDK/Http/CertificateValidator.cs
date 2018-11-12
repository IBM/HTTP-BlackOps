using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrafficViewerSDK.Http
{
    

    /// <summary>
    /// Validates remote certificate
    /// </summary>
    public class CertificateValidator
    {
        private static X509Certificate2 _cert;
        private static object _lock = new object();

        /// <summary>
        /// Util function to validate certs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="policyErrors"></param>
        /// <returns></returns>
        public static bool ValidateRemoteCertificate(
           object sender,
           X509Certificate certificate,
           X509Chain chain,
           SslPolicyErrors policyErrors)
        {
            lock (_lock)
            {

                if (_cert != null && _cert.Equals(certificate))
                {
                    return true;
                }

                var cert = new X509Certificate2(certificate);

                if (!cert.Verify())
                {

                    //save the certificate in the local trusted certificate store
                    X509Store certStore = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
                    certStore.Open(OpenFlags.ReadWrite);
                    X509Certificate2Collection certCol = certStore.Certificates.Find(
                        X509FindType.FindByThumbprint, cert.Thumbprint, false);

                    if (certCol.Count > 0 && certCol[0].Equals(cert))
                    {

                        _cert = cert;
                        return true;
                    }

                    string message = String.Format(
                        "Do you want to trust the certificate from {0} with thumbprint {1}?", cert.Subject, cert.Thumbprint);
                    if (MessageBox.Show(message, "Certificate Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {

                        certStore.Add(cert);
                        certStore.Close();
                        _cert = cert;
                        return true;
                    }
                    else
                    {
                        certStore.Close();
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
