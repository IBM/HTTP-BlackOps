using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace TrafficViewerSDK.Http
{

	/// <summary>
	/// Stores authentication information
	/// </summary>
	public class HttpAuthenticationInfo : ICloneable
	{

		private NetworkCredential _credentials;
		private bool _isProxy = false;
		/// <summary>
		/// Whether these are proxy credentials or not
		/// </summary>
		public bool IsProxy
		{
			get { return _isProxy; }
			set { _isProxy = value; }
		}
		/// <summary>
		/// Credentials
		/// </summary>
		public NetworkCredential Credentials
		{
			get { return _credentials; }
			set { _credentials = value; }
		}

		/// <summary>
		/// Constructor 
		/// </summary>
		public HttpAuthenticationInfo()
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="credentials"></param>
		/// <param name="isProxy"></param>
        public HttpAuthenticationInfo(NetworkCredential credentials, bool isProxy)
        {
            _credentials = credentials;
			_isProxy = isProxy;
        }


		#region ICloneable Members
		/// <summary>
		/// Makes a clone of this authentication infor object
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			HttpAuthenticationInfo clone = new HttpAuthenticationInfo();
		

			if (_credentials != null)
			{
				clone._credentials = new NetworkCredential(_credentials.UserName, _credentials.Password, _credentials.Domain);
				clone._isProxy = _isProxy;
			}

			return clone;
		}

		#endregion
	}



}
