using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using TrafficViewerSDK;
using System.Diagnostics;

namespace TrafficViewerControls.Browsing
{
	/// <summary>
	/// the struct used to pass the browser configuration to the browser.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct INTERNET_PROXY_INFO2
	{
		/// <summary>
		/// 
		/// </summary>
		public int dwAccessType;
		/// <summary>
		/// 
		/// </summary>
		public string lpszProxy;
		/// <summary>
		/// 
		/// </summary>
		public string lpszProxyBypass;
	}

	public static class BrowserUtils
	{
		[DllImport("wininet.dll", CharSet = CharSet.Auto)]
		private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

		[DllImport("wininet.dll", CharSet = CharSet.Auto)]
		private static extern bool InternetGetCookie(string lpszUrlName, string lpszCookieName, System.Text.StringBuilder lpszCookieData, [MarshalAs(UnmanagedType.U4)]ref int lpdwSize);

		[DllImport("wininet.dll", CharSet = CharSet.Auto)]
		private static extern bool InternetSetCookie(string lpszUrlName, string lpszCookieName, string lpszCookieData);

		const int INTERNET_OPTION_PROXY = 38;
		const int INTERNET_OPTION_END_BROWSER_SESSION = 42;

		/// <summary>
		/// Configures the proxy for this browser.
		/// </summary>
		/// <param name="config">The proxy configuration.</param>
		public static void ConfigureProxy(string host, int port)
		{
			INTERNET_PROXY_INFO2 proxyInfo = new INTERNET_PROXY_INFO2();
			// I'm pretty sure proxyInfo is correct
			proxyInfo.dwAccessType = 3;
			proxyInfo.lpszProxy = String.Format("http={0}:{1} https={0}:{1}",host,port);
			proxyInfo.lpszProxyBypass = "";
			//Console.Out.WriteLine(Marshal.SizeOf(proxyInfo));
			// I saw a website that tried passing 0 as the first parameter and a place with something like ie.HWND

			IntPtr ptrToProxyStruct;
			ptrToProxyStruct = Marshal.AllocHGlobal(Marshal.SizeOf(proxyInfo));
			Marshal.StructureToPtr(proxyInfo, ptrToProxyStruct, false);

			InternetSetOptionWrapper(IntPtr.Zero, INTERNET_OPTION_PROXY, ptrToProxyStruct, Marshal.SizeOf(proxyInfo));
			Marshal.FreeHGlobal(ptrToProxyStruct);
		}

		private static bool InternetSetOptionWrapper(IntPtr hInternet, int dwOption, IntPtr lbBuffer, int dwBufferLength)
		{
			try
			{
				if (InternetSetOption(hInternet, dwOption, lbBuffer, dwBufferLength))
				{
					return true;
				}
			}
			catch (Exception e)
			{
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, e.Message + e.Source + "\n\n" + e.StackTrace);
			}
			return false;
			//MessageBox.Show(Strings.Instance.GetString("AppScanBrowser.CouldNotSetInternetOptions"),
			//    Strings.Instance.GetString("Global.ProductNameShort"),
			//    MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		/// <summary>
		/// Clear browser's session cookies
		/// </summary>
		public static void ClearSessionCookies()
		{
			InternetSetOptionWrapper(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);
		}

		public static void DeletePermanentCookie(string URL)
		{
			int bufSize = 4096;
			System.Text.StringBuilder buffer = new System.Text.StringBuilder(bufSize);
			if (InternetGetCookie(URL, null, buffer, ref bufSize) == false)
			{
				return;
			}

			string cookie = buffer.ToString();
			int index = cookie.IndexOf('=');
			string cookieString = "=; expires = Thu, 18-Apr-2000 00:00:00 GMT";

			while (index >= 0)
			{
				// Ok, LHS of cookie is name, RHS is not
				string name = cookie.Substring(0, index);
				InternetSetCookie(URL, null, name + cookieString);

				//Debug.Logger.Log(System.Diagnostics.TraceLevel.Verbose, "DeletePermanentCookie: cookie name:" + name + " url:" + URL); // Automatic log

				// Find the next cookie
				index = cookie.IndexOf(';', index + 1);
				if (index > 0)
				{
					cookie = cookie.Substring(index + 1);
					index = cookie.IndexOf('=');
				}
			}

		}
	}
}
