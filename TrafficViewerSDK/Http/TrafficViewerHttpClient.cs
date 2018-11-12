using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Properties;
using TrafficViewerSDK.Options;
using System.Net;
using System.Threading;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// A simple, low level HTTP client used by Traffic Viewer
	/// Allows, malformed, test requests to be sent
	/// </summary>
	public class TrafficViewerHttpClient : BaseHttpClient
	{
        HttpClientConnection _connection = null;

		/// <summary>
		/// Gets/sets the timeout in milisecons for this client
		/// </summary>
		public int Timeout
		{
			get { return _timeout; }
			set { _timeout = value; }
		}


		/// <summary>
		/// Sends the request to the site
		/// </summary>
		/// <param name="requestInfo"></param>
		/// <returns></returns>
		public override HttpResponseInfo SendRequest(HttpRequestInfo requestInfo)
		{
            PrepareConnection(requestInfo);

			HttpClientRequest request = new HttpClientRequest(_connection);

            //check if the request requires credentials
            HttpAuthenticationInfo authInfo;
			if (HttpAuthenticationManager.Instance.RequiresCredentials(requestInfo, out authInfo))
            {
				requestInfo.Headers.Add(HttpAuthenticationManager.Instance.GetBasicAuthHeader(authInfo));  
            }

			//delay the request
			ProcessRequestDelay(requestInfo);

			request.SendRequest(requestInfo, requestInfo.IsSecure);

			bool success = request.RequestCompleteEvent.WaitOne(_timeout);
			
			if (!success || request.Response == null)
			{
				throw new Exception(Resources.RequestTimeout);
			}
            string rawResponse = Constants.DefaultEncoding.GetString(request.Response);
			string responseStatus = HttpResponseInfo.GetResponseStatus(rawResponse);
			bool isPlatformAuth = String.Compare(responseStatus, "401") == 0 ;
			bool isProxyAuth = String.Compare(responseStatus, "407") == 0;
			if (isPlatformAuth || isProxyAuth)
            { 
                //check the headers
                HttpResponseInfo respInfo = new HttpResponseInfo(rawResponse);
				List<HTTPHeader> authHeaders = new List<HTTPHeader>();
				if (isPlatformAuth)
				{
					authHeaders = respInfo.Headers.GetHeaders("WWW-Authenticate");
				}
				else if (isProxyAuth)
				{
					authHeaders = respInfo.Headers.GetHeaders("Proxy-Authenticate");
				}

                bool usesBasic = false;

                for (int i = authHeaders.Count-1;i>-1;i--) //go backwards the basic header is usually last
                {
                    HTTPHeader header = authHeaders[i];
                    if (header.Value.IndexOf("Basic", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        usesBasic = true;
                        break;
                    }
                }

                if (usesBasic)
                { 
                    //try to get the credentials from the user
                    authInfo = HttpAuthenticationManager.Instance.GetCredentials(requestInfo, isProxyAuth);
					if (authInfo != null)
                    {
						//add the basic auth header
						requestInfo.Headers.Add(HttpAuthenticationManager.Instance.GetBasicAuthHeader(authInfo));
						//create a new request
						PrepareConnection(requestInfo);
						request = new HttpClientRequest(_connection);
                        //proceed with new request
                        request.SendRequest(requestInfo, requestInfo.IsSecure);
                        success = request.RequestCompleteEvent.WaitOne(_timeout);

                        if (!success)
                        {
                            throw new Exception(Resources.RequestTimeout);
                        }
                    }
                    
                }
            }

			if (request.Response != null)
			{ 
				return new HttpResponseInfo(request.Response);
			}

			return null;
		}



        private void PrepareConnection(HttpRequestInfo requestInfo)
        {
			WebProxy proxy = null;

			if (NetworkSettings != null)
			{
				if (NetworkSettings.WebProxy != null)
				{
					proxy = (NetworkSettings.WebProxy as WebProxy);

					if(proxy != null)
					{
						_connection = new HttpClientConnection(proxy.Address.Host, proxy.Address.Port, false);
					}
				}
			}

			if (proxy != null)
            {
				_connection = new HttpClientConnection(proxy.Address.Host, proxy.Address.Port, false);
            }
            else
            {
                _connection = new HttpClientConnection(requestInfo.Host, requestInfo.Port, requestInfo.IsSecure);
            }
        }

        

	}
}
