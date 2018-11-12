using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Constains network settings such as proxy, ssl callback and credentials for host
	/// </summary>
	public class DefaultNetworkSettings : INetworkSettings
	{
		private IWebProxy _webProxy;
		/// <summary>
		/// Use to pass a WebProxy 
		/// </summary>
		public IWebProxy WebProxy
		{
			get { return _webProxy; }
			set { _webProxy = value;}
		}

	
		RemoteCertificateValidationCallback _certificateValidationCallback;
		/// <summary>
		/// Method to execute when the certificate is invalid by default always true
		/// </summary>
		public RemoteCertificateValidationCallback CertificateValidationCallback
		{
			get { return _certificateValidationCallback; }
			set { _certificateValidationCallback = value; }
		}

		private ICredentialsByHost _credentialsByHost;
		/// <summary>
		/// Allows setting credentials, but only for a specific host, this is done this way
		/// to prevent sending credentials to unauthorized hosts
		/// </summary>
		public ICredentialsByHost CredentialsByHost
		{
			get { return _credentialsByHost; }
			set { _credentialsByHost = value; }
		}

	}
}
