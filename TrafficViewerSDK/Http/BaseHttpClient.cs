using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Options;
using System.Net;
using TrafficViewerSDK.Properties;
using System.Threading;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Abstract base class for implementing HTTP Client functionality
	/// </summary>
	public abstract class BaseHttpClient : IHttpClient
	{

		protected int _timeout = SdkSettings.Instance.HttpRequestTimeout * 1000;

        protected bool _canHandleCookies = false;
        /// <summary>
        /// Gets whether the client can handle cookies, by default false
        /// </summary>
        public bool CanHandleCookies
        {
            get { return _canHandleCookies; }
        }

        private bool _shouldHandleCookies = false;
        /// <summary>
        /// If the client has cookie handling capabilities this indicates that it should handle them
        /// By default is false
        /// </summary>
        public bool ShouldHandleCookies
        {
            get { return _shouldHandleCookies; }
            set { _shouldHandleCookies = value; }
        }

		private int _requestDelay = SdkSettings.Instance.RequestDelay * 1000;
		private string _requestDelayFilter = SdkSettings.Instance.RequestDelayFilter;

		protected void ProcessRequestDelay(HttpRequestInfo requestInfo)
		{
			//delay the request
			if (_requestDelay > 0)
			{
				bool delay = false;

				if (!String.IsNullOrEmpty(_requestDelayFilter) && !_requestDelayFilter.Trim().Equals(".*"))
				{
					
					delay = Utils.IsMatch(requestInfo.FullUrl, _requestDelayFilter) || 
						Utils.IsMatch(requestInfo.ContentDataString,_requestDelayFilter);
				}
				else
				{
					delay = true;
				}

				if (delay)
				{
					Thread.Sleep(_requestDelay);
				}
			}
		}

		/// <summary>
		/// Contains the network settings
		/// </summary>
		protected INetworkSettings NetworkSettings;

		#region Abstract

		/// <summary>
		/// Sends the requests to site and returns an array of bytes
		/// </summary>
		/// <param name="requestInfo"></param>
		/// <returns></returns>
		public abstract HttpResponseInfo SendRequest(HttpRequestInfo requestInfo);
	
		#endregion

		/// <summary>
		/// Sets various settings such as proxy, ssl callback and credentials for a spefic host
		/// </summary>
		/// <param name="networkSettings"></param>
		public void SetNetworkSettings(INetworkSettings networkSettings)
		{
			NetworkSettings = networkSettings;
		}

		/// <summary>
		/// Sets proxy settings specifically
		/// </summary>
		/// <param name="proxyHost"></param>
		/// <param name="proxyPort"></param>
		/// <param name="credentials"></param>
		public void SetProxySettings(string proxyHost, int proxyPort, ICredentials credentials)
		{
			if (NetworkSettings == null)
			{
				NetworkSettings = new DefaultNetworkSettings();
			}

			NetworkSettings.WebProxy = new WebProxy(proxyHost, proxyPort);
			NetworkSettings.WebProxy.Credentials = credentials;
		}


	}
}