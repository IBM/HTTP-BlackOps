using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Security;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Contains settings for the proxy, not to be confused with proxy settings for the client
	/// </summary>
	public interface INetworkSettings
	{
		/// <summary>
		/// Use to pass a WebProxy 
		/// </summary>
		IWebProxy WebProxy
		{
			get;
			set;
		}


		/// <summary>
		/// Pass credentials
		/// </summary>
		ICredentialsByHost CredentialsByHost
		{
			get;
			set;
		}

		/// <summary>
		/// Method to execute when the certificate is invalid
		/// </summary>
		RemoteCertificateValidationCallback CertificateValidationCallback
		{
			get;
			set;
		}
	}
}
