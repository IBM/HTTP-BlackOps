using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Windows.Forms;
using TrafficViewerControls.Properties;
using System.Net;
using TrafficViewerInstance;
using System.IO;

namespace TrafficViewerControls.Browsing
{
	public static class SSLValidationCallback
	{
		private static List<string> hostsAndPortsToIgnore = new List<string>();

		// callback used to validate the certificate in an SSL conversation
		public static bool ValidateRemoteCertificate(
			object sender,
			X509Certificate certificate,
			X509Chain chain,
			SslPolicyErrors policyErrors)
		{


			X509Certificate2 cert = new X509Certificate2(certificate);

			if (cert.Verify())
			{
				return true;
			}

            if (TrafficViewerOptions.Instance.IgnoreInvalidSslCert)
            {
                return true;
            }

			HttpWebRequest webRequest = sender as HttpWebRequest;
			string hostAndPort = null;
			if (webRequest != null)
			{
				hostAndPort = String.Format("{0}:{1}", webRequest.RequestUri.Host, webRequest.RequestUri.Port);
				if (hostsAndPortsToIgnore.Contains(hostAndPort))
				{
					return true;
				}
			}

			DialogResult dr = MessageBox.Show(Resources.UntrustedSSL,Resources.Warning, MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
			if (dr == DialogResult.Yes)
			{
				if (hostAndPort != null)
				{ 
					//save this so we don't ask again
					hostsAndPortsToIgnore.Add(hostAndPort);
				}
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
