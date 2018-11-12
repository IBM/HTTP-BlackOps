using System;
using System.Collections.Generic;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;
using TrafficViewerSDK.Options;

namespace RequestSender
{
	/// <summary>
	/// Helper class that allows tracking of session ids
	/// </summary>
	public class SessionIdHelper
	{
		private List<string> _responsePatterns = SdkSettings.Instance.ResponsePatterns;
		private List<string> _sessionIdNames = SdkSettings.Instance.SessionIdPatterns;
		/// <summary>
		/// session id parameter names/values global to the current session
		/// </summary>
		private Dictionary<string, string> _sessionIds = new Dictionary<string,string>();
		private object _lock = new object();
        private bool _handleCookies = false;
        /// <summary>
        /// Whether to track cookies
        /// </summary>
        public bool HandleCookies
        {
            get { return _handleCookies; }
            set { _handleCookies = value; }
        }
		
		/// <summary>
		/// Updates the parameters and cookies of the request with the parameters and cookies of the specified response
		/// </summary>
		/// <param name="newRequest"></param>
		/// <param name="prevRequest"></param>
		/// <param name="prevResponse"></param>
		public void UpdateSessionIds(HttpRequestInfo newRequest, HttpRequestInfo prevRequest, HttpResponseInfo prevResponse)
		{
            

			if (prevRequest != null && _handleCookies)
			{
				//copy the cookie values from the previous request
				foreach (string cookieName in prevRequest.Cookies.Keys)
				{
					if (newRequest.Cookies.ContainsKey(cookieName))
					{
						newRequest.Cookies[cookieName] = prevRequest.Cookies[cookieName];
					}
					else
					{
						newRequest.Cookies.Add(cookieName, prevRequest.Cookies[cookieName]);
					}
				}
				
			}
            

			if (prevResponse != null)
			{
				//update cookies
                if (_handleCookies)
                {
                    List<HTTPHeader> setCookieHeaders = prevResponse.Headers.GetHeaders("Set-Cookie");
                    foreach (HTTPHeader setCookieHeader in setCookieHeaders)
                    {
                        if (!String.IsNullOrEmpty(setCookieHeader.Value))
                        {

                            string[] attributes = setCookieHeader.Value.Split(';');
                            string[] nameAndValue = attributes[0].Split(new String[1] { "=" }, 2, StringSplitOptions.RemoveEmptyEntries);
                            string name = nameAndValue[0];
                            string value = String.Empty;
                            if (nameAndValue.Length > 1)
                            {
                                value = nameAndValue[1];
                            }

                            if (newRequest.Cookies.ContainsKey(name))
                            {
                                newRequest.Cookies[name] = value;
                            }
                            else
                            {
                                newRequest.Cookies.Add(name, value);
                            }
                        }
                    }
                }
				//update params
				string responseText = prevResponse.Headers.ToString() + Environment.NewLine + prevResponse.ResponseBody.ToString();
				UpdateParams(newRequest.QueryVariables, responseText);
				UpdateParams(newRequest.BodyVariables, responseText);
			}
		}


		private void UpdateParams(HttpVariables variables, string responseText)
		{
			List<string> paramNames = new List<string>(variables.Keys);
			foreach (string name in paramNames)
			{
				if (Utils.IsMatchInList(name, _sessionIdNames))
				{
					//update value from previous requests
					if (_sessionIds.ContainsKey(name))
					{
						lock (_lock)
						{
							variables[name] = _sessionIds[name];
						}
					}

					foreach (string responsePattern in _responsePatterns)
					{
						//match known response patterns
						string regex = String.Format(responsePattern, name);
						string value = Utils.RegexFirstGroupValue(responseText, regex);

						if (!String.IsNullOrEmpty(value))
						{
							//update the parameter with the new value
							variables[name] = value;
							lock (_lock)
							{
								_sessionIds[name] = value;
							}
							break;//pattern was found break the loop
						}
					}
				}
			}
		}
	}
}
