using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using TrafficViewerSDK.Properties;
using System.IO;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Loads and stores the internal proxy certificate
	/// </summary>
	public static class AppScanProxyCert
	{
		private static X509Certificate2 _cert;
		private static object _certLock = new object();
		/// <summary>
		/// Gets the proxy certificate
		/// </summary>
		public static X509Certificate2 Cert
		{
			get
			{
				lock (_certLock)
				{
					if (_cert == null)
					{
						LoadCert();
					}

				}
				return _cert;
			}
		}

		private static void LoadCert()
		{
			try
			{
				_cert = new X509Certificate2();
				_cert.Import(Resources.blackopscert, "Demo1234", X509KeyStorageFlags.Exportable);
			}
			catch
			{
				_cert = null;
			}
		}

		/// <summary>
		/// Loads a certificate from the specified file
		/// </summary>
		/// <param name="path"></param>
		/// <param name="pass"></param>
		public static void LoadCertFromFile(string path, string pass)
		{
			try
			{
				_cert = new X509Certificate2();
				if (File.Exists(path))
				{
					_cert.Import(path, pass, X509KeyStorageFlags.Exportable);
				}
			}
			catch
			{
				_cert = null;
			}
		}

	}
}
