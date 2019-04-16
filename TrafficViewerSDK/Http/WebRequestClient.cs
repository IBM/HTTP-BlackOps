/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
 */
using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Properties;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

namespace TrafficViewerSDK.Http
{  
	/// <summary>
	/// Http Client using the .Net builtin WebRequest
	/// </summary>
	public class WebRequestClient : BaseHttpClient
	{
		const int MAX_BUFFER_SIZE = 10024;	//max buffer size for response stream chunks
        CookieContainer _cookieJar = new CookieContainer();
        
		

        /// <summary>
		/// Constructor
		/// </summary>
        public WebRequestClient()
        { 
            //initialize common web request client settings, this will happen only once
            WebRequestCommonSettingsInitializer.Initialize();
            _canHandleCookies = true;
        }

		/// <summary>
		/// Sends the request to the site
		/// </summary>
		/// <param name="requestInfo"></param>
		/// <returns></returns>
		public override HttpResponseInfo SendRequest(HttpRequestInfo requestInfo)
		{
			

			HttpWebRequest webRequest = WebRequest.Create(requestInfo.FullUrl) as HttpWebRequest;
            if (ShouldHandleCookies)
            {
                webRequest.CookieContainer = _cookieJar;
            }

			ProcessRequestDelay(requestInfo);

			webRequest.Timeout = _timeout;

            //attempt to reserve a connection group name for the current object
            //this will make the web request keep alive work properly
            webRequest.ConnectionGroupName = GetHashCode().ToString(); 

			SetNetworkSettings(webRequest);

			CopyHeaders(requestInfo, webRequest);

			//request Method (GET, POST, etc..)
			webRequest.Method = requestInfo.Method;

			string version = requestInfo.HttpVersion.Replace("HTTP/","");
            if (!version.Equals("1.0") && !version.Equals("1.1"))
            {
                version = "1.1";
            }
            webRequest.ProtocolVersion = Version.Parse(version);
			webRequest.AllowAutoRedirect = false;

			//add POST data, if any

			byte[] postData = requestInfo.ContentData;
			if (postData != null && postData.Length > 0)
			{
				try
				{
                    webRequest.ContentLength = postData.Length;
					Stream requestStream = webRequest.GetRequestStream();
					requestStream.Write(postData, 0, postData.Length);
					requestStream.Close();
				}
				catch(ProtocolViolationException ex) //ignore malformed request such as AppScan requests
				{
					SdkSettings.Instance.Logger.Log(TraceLevel.Verbose, "WebRequestClient: Got a malformed request: {0}", ex.Message);
				}
			}
            
            HttpWebResponse response = null;
			//pass the request to the target server

			try
			{
				response = webRequest.GetResponse() as HttpWebResponse;
			}
			catch (WebException webException)
			{
				response = webException.Response as HttpWebResponse;

				if (response == null)
				{
					throw webException;
				}
			}

			HttpResponseInfo responseInfo = null;
			//read response
			if (response != null)
			{
				string responseLine = String.Format("HTTP/{0} {1} {2}\r\n", response.ProtocolVersion, (int)response.StatusCode, response.StatusDescription);

				responseInfo = new HttpResponseInfo(responseLine);

				AddResponseHeaders(response, responseInfo);

				//add response body
				string transferEncoding = response.Headers["Transfer-Encoding"];
				responseInfo.ResponseBody.IsChunked = transferEncoding != null && String.Compare("chunked", transferEncoding, true) == 0;

				Stream responseStream = response.GetResponseStream();
				int read;
				byte[] buffer = new byte[MAX_BUFFER_SIZE];

				while ((read = responseStream.Read(buffer, 0, MAX_BUFFER_SIZE)) > 0)
				{
					responseInfo.ResponseBody.AddChunkReference(buffer, read);
				}
				
				response.Close();
			}

			return responseInfo;
		}

		

		private void CopyHeaders(HttpRequestInfo requestInfo, HttpWebRequest webRequest)
		{
			//by default set 100Continue to false to prevent 417 errors
			//however if the client sends an expect header we will change this back
			webRequest.ServicePoint.Expect100Continue = false;	

			//add headers
			foreach (HTTPHeader header in requestInfo.Headers)
			{
				if (String.Compare(header.Name, "Accept", true) == 0)
				{
					webRequest.Accept = header.Value;
				}
				else if (String.Compare(header.Name, "Connection", true) == 0 || String.Compare(header.Name, "Proxy-Connection", true) == 0)
				{
					if (String.Compare(header.Value, "close", true) == 0)
					{
						webRequest.KeepAlive = false;
					}
					else
					{
						webRequest.KeepAlive = true;
					}
				}
				else if (String.Compare(header.Name, "Content-Length", true) == 0)
				{
					long length;
					if (long.TryParse(header.Value, out length))
					{
						webRequest.ContentLength = length;
					}
				}
				else if (String.Compare(header.Name, "Content-Type", true) == 0)
				{
					webRequest.ContentType = header.Value;
				}
				else if (String.Compare(header.Name, "If-Modified-Since", true) == 0)
				{
					DateTime date;
					if (DateTime.TryParse(header.Value, out date))
					{
						webRequest.IfModifiedSince = date;
					}
				}
				else if (String.Compare(header.Name, "Referer", true) == 0)
				{
					webRequest.Referer = header.Value;
				}
				else if (String.Compare(header.Name, "Transfer-Encoding", true) == 0)
				{
					webRequest.TransferEncoding = header.Value;
				}
				else if (String.Compare(header.Name, "User-Agent", true) == 0)
				{
					webRequest.UserAgent = header.Value;
				}
				else if (String.Compare(header.Name, "Date", true) == 0)
				{
					DateTime date;
					if (DateTime.TryParse(header.Value, out date))
					{
						webRequest.Date = date;
					}
					else
					{
						//the date could not be parsed but try to set now as the date
						webRequest.Date = DateTime.Now;
					}
				}
				else if (String.Compare(header.Name, "Range", true) == 0)
				{
					string specifier;
					int from;
					int to;

					HttpUtil.ParseRange(header.Value, out specifier, out from, out to);
					if (from < to)
					{
						webRequest.AddRange(specifier, from, to);
					}
				}
				else if (String.Compare(header.Name, "Expect", true) == 0)
				{
					if (String.Compare(header.Value, "100-Continue", true) == 0)
					{
						webRequest.ServicePoint.Expect100Continue = true;
					}
				}
				else if (String.Compare(header.Name, "Host", true) == 0) 
				{
					//do nothing for these headers
					;
				}
                else if (String.Compare(header.Name, "Cookie", true) == 0 && ShouldHandleCookies)
                {
                    //add the additional cookies to the jar
                    //if should handle cookies is on cookie management is done by the jar (we want to be able to add cookies)
                    HttpVariables vars = new HttpVariables(header.Value, RequestLocation.Cookies);
                    foreach (var cookie in vars)
                    { 
                        _cookieJar.Add(new Cookie(cookie.Key, cookie.Value, requestInfo.Path, requestInfo.Host));
                    }
                }
				else
				{
					try
					{
                        //webRequest doesn't support PseudoHeaders
                        if(!header.Name.StartsWith(":"))
						    webRequest.Headers.Set(header.Name, header.Value);
					}
					catch
					{
                        SdkSettings.Instance.Logger.Log(TraceLevel.Error, "WebRequestClient error. Cannot set header {0}", header.Name);
					}
				}
			}
		}

		private static void AddResponseHeaders(HttpWebResponse response, HttpResponseInfo responseInfo)
		{   
			foreach (string key in response.Headers.Keys)
			{
				string headerValue = response.Headers[key].Trim();
				//these headers can occur multiple times in the response
				//as per the RFC if the value is split by a comma should be 
				//considered multiple message header fields however there are cases when the
                //header can contain a comma for example Date: Sun, December 23
				if (String.Compare(key, "WWW-Authenticate", true) == 0 ||
					String.Compare(key, "Proxy-Authenticate", true) == 0)
				{
					if (headerValue.StartsWith("Basic", StringComparison.OrdinalIgnoreCase)
						|| headerValue.StartsWith("NTLM", StringComparison.OrdinalIgnoreCase)
						|| headerValue.StartsWith("Negotiate", StringComparison.OrdinalIgnoreCase))
					{
						//get the values
						string[] values = headerValue.Split(',');
						foreach (string value in values)
						{
							responseInfo.Headers.Add(key, value);
						}
					}
					else
					{
						responseInfo.Headers.Add(key, headerValue);
					}
				}
                else if (String.Compare(key, "Set-Cookie", true) == 0)
                {
                    AddCookies(responseInfo, headerValue);
                }
                else 
                {
                    responseInfo.Headers.Add(key, headerValue);
                }
			}
		}

        /// <summary>
        /// Checks whether the given string contains characters not allowed in a cookie name
        /// </summary>
        /// <param name="str"></param>
        /// <returns>True if the string contains such characters, false otherwise</returns>
        private static bool HasInvalidCookieNameChars(string str)
        {
            /*
             * From RFC2616 section 2.2: 
             * Syntax for valid tokens
             * 
             * token          = 1*<any CHAR except CTLs or separators>
             * separators     = "(" | ")" | "<" | ">" | "@"
             *                | "," | ";" | ":" | "\" | <">
             *                | "/" | "[" | "]" | "?" | "="
             *                | "{" | "}" | SP | HT 
             */

            List<char> invalidNameChars = new List<char>();

            //add all the CTLS and separators
            for (int i = 0; i <= 32; i++)
            {
                invalidNameChars.Add((char)i);
            }
            invalidNameChars.Add((char)127);
            
            //add all the other chars
            invalidNameChars.Add('(');
            invalidNameChars.Add(')');
            invalidNameChars.Add('<');
            invalidNameChars.Add('>');
            invalidNameChars.Add('@');
            invalidNameChars.Add(',');
            invalidNameChars.Add(';');
            invalidNameChars.Add(':');
            invalidNameChars.Add('\\');
            invalidNameChars.Add('"');
            invalidNameChars.Add('/');
            invalidNameChars.Add('[');
            invalidNameChars.Add(']');
            invalidNameChars.Add('?');
            invalidNameChars.Add('=');
            invalidNameChars.Add('{');
            invalidNameChars.Add('}');

            return (str.IndexOfAny(invalidNameChars.ToArray()) > -1);
        }

        /// <summary>
        /// Checks whether setCookieHeader is actually a valid cookie
        /// </summary>
        /// <param name="setCookieHeader"></param>
        /// <returns>True if valid, false otherwise</returns>
        private static bool IsValidSetCookieHeader(string setCookieHeader)
        {
            bool isValid = true;

            /*
             * A Set-Cookie header starts with a cookie name-value pair e.g., name=value, 
             * which is optionally followed by some attributes, separated by ';'
             */

            string[] cookieParts = setCookieHeader.Split(new char[]{';'});
            string nameValuePair = cookieParts[0];  //first one is the name-value pair

            //Split the name and the value
            string[] nameAndValue = nameValuePair.Split(new char[]{'='}, 2);
            
            if (nameAndValue.Length == 2)   //Must contain a name and a value
            {
                string name = nameAndValue[0].Trim();

                if (!String.IsNullOrEmpty(name))
                {
                    if (HasInvalidCookieNameChars(name))
                    {
                        isValid = false;
                    }
                }
            }
            else
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Method for adding cookies to the HttpResponseInfo object
        /// </summary>
        /// <param name="responseInfo">The HttpResponseInfo object</param>
        /// <param name="headerValue">The "Set-Cookie" header value</param>
        private static void AddCookies(HttpResponseInfo responseInfo, string headerValue)
        {
            /*
             * In case of multiple Set-Cookie headers, .NET collaspses them into a single Set-Cookie header, separated by comma.
             * But cookie attributes themselves can contain commas. Therefore, in order to correctly parse the Set-Cookie header, 
             * we need to ensure that the string tokens that we get after splitting are valid cookies. A valid cookie must start 
             * with a name-value pair separated by "=" that contains the name of the cookie. 
             */

            string[] setCookieHeaders = headerValue.Split(',');
            
            HTTPHeader previousSetCookieHeader = null;

            foreach (string part in setCookieHeaders)
            {   
                if (IsValidSetCookieHeader(part))   //Valid cookie start found, create a new "Set-Cookie"
                {
                    previousSetCookieHeader = new HTTPHeader("Set-Cookie", part);
                    responseInfo.Headers.Add(previousSetCookieHeader);
                }
                else //not a valid cookie start, this should be appended to the last "Set-Cookie"
                {
                    if (previousSetCookieHeader != null)
                    {
                        previousSetCookieHeader.Values[0] += "," + part;
                    }
                }
            }
        }

		private void SetNetworkSettings(HttpWebRequest webRequest)
		{
			// set the default as this is global property.
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls12;

			if (NetworkSettings != null)
			{
				ServicePointManager.ServerCertificateValidationCallback = NetworkSettings.CertificateValidationCallback;

				webRequest.Proxy = NetworkSettings.WebProxy;
				if (NetworkSettings.CredentialsByHost != null)
				{
					webRequest.Credentials = NetworkSettings.CredentialsByHost.GetCredential(webRequest.RequestUri.Host, webRequest.RequestUri.Port, null);
				}

			}

			//configure the client to use the local certificate store
			X509Store store = new X509Store("My");
			store.Open(OpenFlags.ReadOnly);
			webRequest.ClientCertificates = store.Certificates;

		}
	}
}