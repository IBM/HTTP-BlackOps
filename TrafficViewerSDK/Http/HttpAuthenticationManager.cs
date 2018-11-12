using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Stores authentication credentials in the memory
	/// </summary>
	public class HttpAuthenticationManager : CacheManager<CacheEntry>
	{

        private ICredentialsProvider _credentialsProvider = null;
        /// <summary>
        /// Object that obtains credentials from the user
        /// </summary>
        public ICredentialsProvider CredentialsProvider
        {
            get { return _credentialsProvider; }
            set { _credentialsProvider = value; }
        }

		private static HttpAuthenticationManager _instance = null;
		private static object _instanceLock = new object();

		/// <summary>
		/// Gets the authentication manager instance
		/// </summary>
		public static HttpAuthenticationManager Instance
		{
			get
			{
				lock (_instanceLock)
				{
					if (_instance == null)
					{
						_instance = new HttpAuthenticationManager();
					}
				}

				return _instance;
			}
		}


		private HttpAuthenticationManager() 
		{
			MaxSize = 9999;
		}

        /// <summary>
        /// Checks if the specified request requires credentials (an entry exists in the memory)
        /// </summary>
		/// <param name="reqInfo"></param>
        /// <param name="authInfo"></param>
        /// <returns></returns>
		public bool RequiresCredentials(HttpRequestInfo reqInfo, out HttpAuthenticationInfo authInfo)
        {
            int reqHash = reqInfo.HostAndPort.GetHashCode();
            authInfo = null;
            CacheEntry entry = this.GetEntry(reqHash);
            if (entry != null)
            {
                authInfo = entry.GetClone() as HttpAuthenticationInfo;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the credentials for the specified requests using the credential provider
        /// </summary>
        /// <param name="reqInfo"></param>
		/// <param name="isProxy"></param>
        /// <returns></returns>
		public HttpAuthenticationInfo GetCredentials(HttpRequestInfo reqInfo, bool isProxy)
        {
			HttpAuthenticationInfo authInfo = null;
            string domain, userName, password;
            if (_credentialsProvider != null && _credentialsProvider.Execute(out domain, out userName, out password))
            { 
                //encrypt 
                string encryptedPassword = Encryptor.EncryptToString(password);
                
                authInfo = new HttpAuthenticationInfo(new NetworkCredential(userName, encryptedPassword, domain), isProxy);
                //store the auth info in the cache
                this.Add(reqInfo.HostAndPort.GetHashCode(), new CacheEntry((ICloneable)authInfo.Clone()));
                
            }
			return authInfo;
        }

        /// <summary>
        /// Gets the basic authentication header to be added to a HttpRequest
        /// </summary>
        /// <param name="authInfo"></param>
        /// <returns></returns>
        public HTTPHeader GetBasicAuthHeader(HttpAuthenticationInfo authInfo)
        {
            HTTPHeader header = null;
            string credentialsString;
			string decryptedPassword = Encryptor.DecryptToString(authInfo.Credentials.Password);

            if (String.IsNullOrEmpty(authInfo.Credentials.Domain))
            {
				credentialsString = String.Format("{0}:{1}", authInfo.Credentials.UserName, decryptedPassword);
            }
            else
            {
				credentialsString = String.Format("{0}\\{1}:{2}", authInfo.Credentials.Domain, authInfo.Credentials.UserName, decryptedPassword);
            }
            //base64 encode the string
            credentialsString = Utils.Base64Encode(credentialsString);
			if (authInfo.IsProxy)
			{
				header = new HTTPHeader("Proxy-Authorization", String.Format("Basic {0}", credentialsString));
			}
			else
			{
				header = new HTTPHeader("Authorization", String.Format("Basic {0}", credentialsString));
			}
			return header;
        }
	}
}
