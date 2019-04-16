using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK;
using System.Text.RegularExpressions;
using TrafficViewerSDK.Options;
using System.IO;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Stores information about the request received from the client
	/// </summary>
	public class HttpRequestInfo
	{
		private const int ESTIMATED_LINE_SIZE = 1024;
		private bool _drop = false;
		/// <summary>
		/// This flag can be used for individual requests to specify if they should be sent over to the server or not
		/// </summary>
		public bool Drop
		{
			get { return _drop; }
			set { _drop = value; }
		}

		private List<HttpVariableDefinition> _variableDefinitions = SdkSettings.Instance.VariableDefinitions;

		private bool _isProxyHttp = false;

		private Encoding _contentEncoding = Constants.DefaultEncoding;

		#region Properties

		private string _httpVersion;
		/// <summary>
		/// Returns the request http version
		/// </summary>
		public string HttpVersion
		{
			get { return _httpVersion; }
			set 
			{
				_httpVersion = value;
			}
		}

		HTTPHeaders _headers = new HTTPHeaders();
		/// <summary>
		/// The current request headers
		/// </summary>
		public HTTPHeaders Headers
		{
			get { return _headers; }
			set
			{
				if (value != null)
				{
					_headers = value;
					//populate some of the properties
					PopulateHostAndPort(_headers["Host"]);
				}
			}
		}

		private bool _isPost;
		/// <summary>
		/// If this is a post request
		/// </summary>
		public bool IsPost
		{
			get { return _isPost; }
		}


		private bool _isPut;
		/// <summary>
		/// If this is a put request
		/// </summary>
		public bool IsPut
		{
			get { return _isPut; }
		}

		/// <summary>
		/// The request content length
		/// </summary>
		public int ContentLength
		{
			get
			{
				int contentLength = 0;
				if (ContentData != null)
				{
					contentLength = ContentData.Length;
				}
				return contentLength;
			}
		}

		
		private string _host = String.Empty;
		/// <summary>
		/// Extracts the host for the request
		/// </summary>
		public string Host
		{
			get { return _host; }
			set
			{
				_host = value;
				UpdateHostHeader();
			}
		}

		/// <summary>
		/// Gets the host (and port) in the format host(:port)
		/// </summary>
		public string HostAndPort
		{
			get
			{
				if (_port == 0 ||
					(!_isSecure && _port == 80) ||
					(_isSecure && _port == 443))
				{
					return _host;
				}
				else
				{
					return String.Format("{0}:{1}",_host,_port);
				}
			}
			set
			{
				PopulateHostAndPort(value);
				UpdateHostHeader();

			}
		}



		private int _port = 0;
		/// <summary>
		/// The port 
		/// </summary>
		public int Port
		{
			get
			{
				//if a port was not specified use the default ports
				if (_port == 0)
				{
					return _isSecure ? 443 : 80;
				}
				else
				{
					return _port;
				}
			}
			set
			{
				_port = value;
				UpdateHostHeader();
			}
		}


		private string UpdateHostHeader()
		{
			return _headers["Host"] = String.Format("{0}:{1}", Host, Port);
		}

		private bool _isConnect = false;
		/// <summary>
		/// Wether this is a Connect request
		/// </summary>
		public bool IsConnect
		{
			get { return _isConnect; }
		}

		private string _method = String.Empty;
		/// <summary>
		/// Http method for this request
		/// </summary>
		public string Method
		{
			get { return _method; }
		}


		/// <summary>
		/// Returns the request line
		/// </summary>
		public string RequestLine
		{
			get 
			{
				return GetRequestLine(_isProxyHttp);
			}
		}

		string _queryString = String.Empty;
		/// <summary>
		/// Returns the query string
		/// </summary>
		public string QueryString
		{
			get
			{
				if (_queryVariables.Count > 0) //generate the query string dynamically
				{
					string qs = _queryVariables.ToString();
					if (String.IsNullOrEmpty(qs))
					{
						qs = _queryVariables.ToString("=","&");
					}
					return qs;
				}

				return _queryString;
			}
		}

		private byte[] _contentData = null;
		/// <summary>
		/// The post data line. Setting this
		/// </summary>
		public byte[] ContentData
		{
			get
			{
				if (_bodyVariables.Count > 0)
				{
					string pd = GetBodyVariablesString();

					return _contentEncoding.GetBytes(pd);
				}

				return _contentData;
			}
			set
			{
				_bodyVariables.Clear();
				_contentData = value;

			}
		}

		/// <summary>
		/// Get the body variables string
		/// </summary>
		/// <returns></returns>
		private string GetBodyVariablesString()
		{
			string pd = _bodyVariables.ToString();
			if (String.IsNullOrEmpty(pd))
			{
				pd = _bodyVariables.ToString("=", "&");
			}
			return pd;
		}


		/// <summary>
		/// Gets the post data string 
		/// </summary>
		public string ContentDataString
		{
			get
			{
				string value = String.Empty;

				if (_bodyVariables.Count > 0)
				{
					value = GetBodyVariablesString();
				}
				else
				{
					if (_contentData != null)
					{
						value = _contentEncoding.GetString(_contentData);
					}
				}

				return value;
			}
		}

		private bool _isSecure = false;
		/// <summary>
		/// Wether this is a secure request or not
		/// </summary>
		public bool IsSecure
		{
			get { return _isSecure; }
			set { _isSecure = value; }
		}

		/// <summary>
		/// Whether this is a multipart request
		/// </summary>
		public bool IsMultipart
		{
			get
			{
				return _headers["Content-Type"] != null && _headers["Content-Type"].IndexOf("multipart") > -1;
			}
		}


		private string _path = String.Empty;
		/// <summary>
		/// Retrieves the path of the request( without host of query)
		/// </summary>
		public string Path
		{
			get
			{
				if (_pathVariables.Count > 0)
				{
					_path = _pathVariables.ToString();
				}
				return _path;
			}
			set
			{
				_path = value;
			}
		}

		/// <summary>
		/// Gets the full url for the current request
		/// </summary>
		public string FullUrl
		{
			get
			{
				
				string scheme = IsSecure ? "https" : "http";
				string port = (IsSecure && Port == 443) || (!IsSecure && Port == 80) || Port == 0 ? String.Empty : String.Format(":{0}", Port);
				
				string fullUrl = String.Format("{0}://{1}{2}{3}",
					scheme,
					Host,
					port,
					PathAndQuery);

				return fullUrl;
			}
		}

		/// <summary>
		/// The path + the query if applicable
		/// </summary>
		public string PathAndQuery
		{
			get
			{
				if (QueryString != String.Empty)
				{
					return String.Format("{0}?{1}", Path, QueryString);
				}
				else
				{
					return Path;
				}
			}
		}

		private HttpVariables _cookies = new HttpVariables();
		/// <summary>
		/// Returns the request cookies
		/// </summary>
		public HttpVariables Cookies
		{
			get { return _cookies; }
		}


		private HttpVariables _pathVariables = new HttpVariables();
		/// <summary>
		/// Returns custom parameters extracted from the path, according to definitions in the options
		/// </summary>
		public HttpVariables PathVariables
		{
			get { return _pathVariables; }
		}

		private HttpVariables _queryVariables = new HttpVariables();
		/// <summary>
		/// Returns parameters extracted from the query
		/// </summary>
		public HttpVariables QueryVariables
		{
			get { return _queryVariables; }

		}

		private HttpVariables _bodyVariables = new HttpVariables();
		/// <summary>
		/// Returns parameters extracted from the request body
		/// </summary>
		public HttpVariables BodyVariables
		{
			get { return _bodyVariables; }
			set { _bodyVariables = value; }
		}

		/// <summary>
		/// Removes the matching path parameters construction from the path
		/// </summary>
		/// <param name="path">Path string</param>
		/// <param name="replacementString">The string to normalize with</param>
		/// <returns></returns>
		private string NormalizePath(string path, string replacementString)
		{


			//last one has priority
			for (int i = _variableDefinitions.Count - 1; i > -1; i--)
			{
				if (_variableDefinitions[i].Location == RequestLocation.Path && Utils.IsMatch(path, _variableDefinitions[i].Regex))
				{
					path = Regex.Replace(path, _variableDefinitions[i].Regex, replacementString, RegexOptions.IgnoreCase);
					break;
				}
			}

			return path;
		}

		string _searchRegex = String.Empty;
		/// <summary>
		/// Returns a generic request line (without protocol host or http version), mainly for search purposes
		/// </summary>
		public string SearchRegex
		{
			get { return _searchRegex; }
		}

		private bool _isFullRequest;
		/// <summary>
		/// Returns wether all post data was read (post data matches content length)
		/// </summary>
		public bool IsFullRequest
		{
			get { return _isFullRequest; }
			set { _isFullRequest = value; }
		}

		#endregion

		/// <summary>
		/// Optimized method to obtain the request line from a raw request bytes
		/// </summary>
		/// <returns></returns>
		public static string GetRequestLine(byte[] rawRequest)
		{
			string reqLine = String.Empty;

			MemoryStream stream = new MemoryStream(rawRequest);

			byte [] reqLineBytes = Utils.ReadLine(stream, ESTIMATED_LINE_SIZE, LineEnding.Any);
            if (reqLineBytes != null)
            {
                reqLine = Constants.DefaultEncoding.GetString(reqLineBytes);
            }
			return reqLine;
		}

		/// <summary>
		/// Optimized method to obtain the request line from a raw request string
		/// </summary>
		/// <returns></returns>
		public static string GetRequestLine(string rawRequest)
		{ 
			string reqLine = String.Empty;
			if(rawRequest.Length > 0)
			{
				int indexOfNewLine = rawRequest.IndexOfAny(new char[2] {'\r','\n'});
				if (indexOfNewLine > 0)
				{
					reqLine = rawRequest.Substring(0, indexOfNewLine);
				}
				else
				{
					reqLine = rawRequest;
				}
			}

			return reqLine;
		}

		/// <summary>
		/// Parses the request
		/// </summary>
		/// <param name="requestBytes"></param>
        /// <param name="parseVariables">Whether to parse parameters for the request</param>
		private void ParseRequest(byte [] requestBytes, bool parseVariables)
		{

			MemoryStream ms = new MemoryStream(requestBytes);

			byte [] lineBytes = Utils.ReadLine(ms, ESTIMATED_LINE_SIZE, LineEnding.Any);

            string requestLine = Constants.DefaultEncoding.GetString(lineBytes);

			if (!InitMethod(requestLine))
			{ 
				//the request line is not long enough to read the method
				return;
			}

			//extract version
			int lastSpace = requestLine.LastIndexOf(' ');
			if (lastSpace > -1)
			{
				_httpVersion = requestLine.Substring(lastSpace + 1);
			}

			//extract query string


			int firstQuestionMark = requestLine.LastIndexOf('?');
			if (firstQuestionMark > -1)
			{
				_queryString = requestLine.Substring(firstQuestionMark + 1);
				int endOfQuery = _queryString.LastIndexOf(' ');
				if (endOfQuery > -1)
				{
					_queryString = _queryString.Substring(0, endOfQuery);
				}
			}

			//iterate through the lines as long as there is no empty line which would separate post data	
			string line;
			do
			{
				lineBytes = Utils.ReadLine(ms, ESTIMATED_LINE_SIZE, LineEnding.Any);
				if(lineBytes != null && lineBytes.Length > 0)
				{
                    line = Constants.DefaultEncoding.GetString(lineBytes);
					string [] nameAndValue = line.Split(new char[] { ':' }, 3);
                    if (nameAndValue.Length == 2)
                    {
                        _headers.Add(nameAndValue[0], nameAndValue[1].Trim());
                    }
                    else if (nameAndValue.Length == 3)
                    {
                        if (String.IsNullOrWhiteSpace(nameAndValue[0]))
                        {
                            _headers.Add(String.Join(":",nameAndValue[0], nameAndValue[1]), nameAndValue[2].Trim());
                        }
                        else
                        {
                            _headers.Add(nameAndValue[0], String.Join(":", nameAndValue[1], nameAndValue[2]).Trim());

                        }
                    }
				}
			}
			while (lineBytes != null && lineBytes.Length > 0);

			//initialize encoding
			string contentTypeValue = _headers["Content-Type"];
			if (contentTypeValue != null)
			{
				_contentEncoding = HttpUtil.GetEncoding(contentTypeValue);
			}

			if (lineBytes != null) //we read an empty line signaling that the headers are over
			{
				_isFullRequest = true;
				string contentLenHeaderString = _headers["Content-Length"];
				if (contentLenHeaderString != null || IsPost || IsPut)
				{
					int contentLenHeaderVal = 0;
					int.TryParse(contentLenHeaderString, out contentLenHeaderVal);

					long pos = ms.Position;

					int contentLength = (int)(ms.Length - pos);
					if (contentLenHeaderVal > contentLength)
					{
						_isFullRequest = false;
					}

					_contentData = new byte[contentLength];
					ms.Read(_contentData, 0, contentLength);
				}
			}
			else
			{
				_isFullRequest = false; //we didn't finish reading the headers
			}

            if (_headers["Host"] != null)
            {
                PopulateHostAndPort(_headers["Host"]);
            }
            else if (_headers[":authority"] != null)
            {
                PopulateHostAndPort(_headers[":authority"]);
            }


			int indexOfPath = _method.Length + 1;
			string temp = requestLine.Substring(indexOfPath);

			//remove the query and the http version
			if (_httpVersion != String.Empty)
			{
				temp = temp.Replace(_httpVersion, String.Empty);
			}

			if (_queryString != String.Empty)
			{
				temp = temp.Replace(String.Format("?{0} ",_queryString), String.Empty);
			}

			string protocol = String.Empty;

			//remove the protocol://host
			if (temp.IndexOf("https://", StringComparison.OrdinalIgnoreCase) == 0)
			{
				_isSecure = true;
				protocol = "https://";
			}
			else if(temp.IndexOf("http://", StringComparison.OrdinalIgnoreCase) == 0)
			{
				_isProxyHttp = true;
				_isSecure = false;
				protocol = "http://";
			}

			if (protocol != String.Empty) //this is a proxy request
			{
				// skip the protocol string and process whatever comes after that (hostAndPort, and then Path)
				// we already checked that temp starts with protocol and protocol is not empty
				temp = temp.Substring(protocol.Length);
				
				//next extract host and port
				//find the first fw slash
				int indexOfFws = temp.IndexOf('/');

				string hostAndPort = temp.Substring(0, indexOfFws);
				PopulateHostAndPort(hostAndPort);

				temp = temp.Substring(indexOfFws);

			}
			
			_path = temp.TrimEnd(' ');


			//construct a search regex
			string _regexPath = Regex.Escape(_path);


            if (parseVariables)
            {
                ParseVariables();
            }


            //in the case of custom parameters in the path make sure to remove from the regex
            if (_pathVariables.Count > 0)
            {
                _regexPath = NormalizePath(_regexPath, ".+");
            }

            _searchRegex = String.Format("{0} ({1}[^/]+)?{2}[\\s|\\?]", _method, _isSecure ? "https://" : "http://", _regexPath);

            //replace any dynamic elements
            _searchRegex = _searchRegex.Replace(Constants.DYNAMIC_ELEM_STRING, ".+");

            _path = _path.TrimEnd(' ', '?');
			
		}

		/// <summary>
		/// Initializes the method
		/// </summary>
		/// <param name="requestLine"></param>
		/// <returns>False if it fails</returns>
		private bool InitMethod(string requestLine)
		{
			bool success = true;

			int indexOfSpace = requestLine.IndexOf(' ');

			if(indexOfSpace == -1)
			{
				//we can't read the method yet, there's no space in the request line
				return false;
			}

			//handle the method
			_method = requestLine.Substring(0, indexOfSpace);

			_isPost = false;
			_isPut = false;
			_isConnect = false;

			if (String.Compare(_method, "post", true) == 0)
			{
				_isPost = true;
			}
			else if (String.Compare(_method, "put", true) == 0)
			{
				_isPut = true;
			}
			else if (String.Compare(_method, "connect", true) == 0)
			{
				_isConnect = true;
			}

			return success;
		}


		private void PopulateHostAndPort(string hostAndPort)
		{
			if (hostAndPort == null)
			{
				_host = String.Empty;
				return;
			}
			else 
			{
				int splitIndex = hostAndPort.LastIndexOf(':');
				if (splitIndex == -1 || hostAndPort.EndsWith("]")) //either this is an ipv6 address of there are no colons
				{
					_host = hostAndPort;
				}
				else
				{
					int.TryParse(hostAndPort.Substring(splitIndex + 1), out _port);
					_host = hostAndPort.Substring(0, splitIndex);
				}
			}

		}

        /// <summary>
        /// Parses the path, query, cookies and post data of the request if this has not been done in the constructor
        /// </summary>
        public void ParseVariables()
        {
            //process path variables
            _pathVariables = new HttpVariables(_path, RequestLocation.Path);

            _queryVariables = new HttpVariables(_queryString, RequestLocation.Query);

            List<HTTPHeader> cookieHeader = _headers.GetHeaders("Cookie");
            if (cookieHeader.Count > 0)
            {
                _cookies = new HttpVariables(cookieHeader[0].Values[0], RequestLocation.Cookies);
            }
            else
            {
                _cookies = new HttpVariables();
            }
            _bodyVariables = new HttpVariables(ContentDataString, RequestLocation.Body);
        }

		/// <summary>
		/// Parses request elements
		/// </summary>
		/// <param name="requestBytes"></param>
		public HttpRequestInfo(byte[] requestBytes) : this(requestBytes, true)
		{
		}

        /// <summary>
        /// Parses request elements
        /// </summary>
        /// <param name="requestBytes"></param>
        /// <param name="parseVariables"></param>
        public HttpRequestInfo(byte[] requestBytes, bool parseVariables)
        {
            if (requestBytes != null)
            {
				this.ParseRequest(requestBytes, parseVariables);
            }
        }

		/// <summary>
		/// Parses all the request elements
		/// </summary>
		/// <param name="request"></param>
		public HttpRequestInfo(string request)
		{
            this.ParseRequest(Constants.DefaultEncoding.GetBytes(request), true);
		}

        /// <summary>
        /// Parses all the request elements
        /// </summary>
        /// <param name="request"></param>
        /// <param name="parseVariables"></param>
        public HttpRequestInfo(string request, bool parseVariables)
        {
            this.ParseRequest(Constants.DefaultEncoding.GetBytes(request), parseVariables);
        }

		/// <summary>
		/// Used to modify cookie values
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetCookie(string name, string value)
		{

			Cookies[name] = value;

			Headers["Cookie"] = Cookies.ToString("=", "; ");
		}

		/// <summary>
		/// Returns the specified portion of the request
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public string GetRequestComponentString(RequestLocation location)
		{
			string component = String.Empty;

			switch (location)
			{
				case RequestLocation.Body: component = ContentDataString; break;
				case RequestLocation.Path: component = _path; break;
				case RequestLocation.Query: component = _queryString; break;
				case RequestLocation.Cookies: component = _headers["Cookie"]; break;
			}

			return component;
		}

		/// <summary>
		/// Calculate the request hash taking into consideration only
		/// the request line and post data
		/// </summary>
		/// <param name="mode">The traffic server mode</param>
		/// <returns></returns>
		public int GetHashCode(TrafficServerMode mode)
		{
			int result = 0;
			bool includeValues;

			if (mode == TrafficServerMode.BrowserFriendly)
			{
				includeValues = false;
			}
			else
			{
				includeValues = true;
			}

			int queryCode = 0;
			if (_queryVariables.Count == 0 && _queryString != String.Empty)
			{
				queryCode = _queryString.GetHashCode();
			}
			else
			{
				queryCode = _queryVariables.GetHashCode(includeValues);
			}


			int postCode = 0;
			if (_bodyVariables.Count == 0 && _contentData != null)
			{
				postCode = _contentData.GetHashCode();
			}
			else
			{
				postCode = _bodyVariables.GetHashCode(includeValues);
			}

			result = _method.GetHashCode() ^
					_isSecure.GetHashCode() ^
					NormalizePath(_path, String.Empty).GetHashCode() ^
					queryCode ^
					postCode;



			if (mode == TrafficServerMode.Strict)
			{
				result = result ^
						_cookies.GetHashCode(includeValues);
			}

			return result;
		}

		/// <summary>
		/// Overriden get hash code
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return this.GetHashCode(TrafficServerMode.Strict);
		}

		/// <summary>
		/// Overriden Equals
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			bool equals = false;
			HttpRequestInfo other = obj as HttpRequestInfo;
			if (other != null)
			{
				equals = GetHashCode() == other.GetHashCode();
			}
			return equals;
		}

		/// <summary>
		/// Overriden ToString method that obtains theraw request string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ToString(_isProxyHttp);
		}

		/// <summary>
		/// Gets raw request
		/// </summary>
		/// <returns></returns>
		public string ToString(bool isProxyHttp)
		{
			StringBuilder sb = new StringBuilder();
				
			string requestHead = GetRequestHead(isProxyHttp);

			sb.Append(requestHead);

			//post data
			string pdString = ContentDataString;
			if (!String.IsNullOrEmpty(pdString))
			{
				sb.Append(pdString);

			}

			return sb.ToString();
		}

		private string GetRequestHead(bool isProxyHttp)
		{
			//request line
			StringBuilder sb = new StringBuilder();

			string requestLine = GetRequestLine(isProxyHttp);

			sb.AppendLine(requestLine);

			//update cookie header
			if (_cookies.Count > 0)
			{
				_headers["Cookie"] = _cookies.ToString("=", "; ");
			}
			//update content length
			if (ContentData != null)
			{
				_headers["Content-Length"] = ContentData.Length.ToString();
			}
			//headers
			sb.AppendLine(_headers.ToString());
			return sb.ToString();
		}

		/// <summary>
		/// Gets the bytes of the request
		/// </summary>
		/// <param name="isProxyHttp">Whether this is a request to a proxy</param>
		/// <returns></returns>
		public byte[] ToArray(bool isProxyHttp = false)
		{
			string requestHead = GetRequestHead(isProxyHttp);
			ByteArrayBuilder arrayBuilder = new ByteArrayBuilder();
            byte[] requestHeadBytes = Constants.DefaultEncoding.GetBytes(requestHead);
			arrayBuilder.AddChunkReference(requestHeadBytes, requestHeadBytes.Length);
			if (ContentData != null)
			{
				arrayBuilder.AddChunkReference(ContentData, ContentData.Length);
			}
			return arrayBuilder.ToArray();
		}

		private string GetRequestLine(bool isProxyHttp)
		{
			string requestLine;

			if (!isProxyHttp)
			{
				requestLine = String.Format("{0} {1} {2}", Method, PathAndQuery, HttpVersion);
			}
			else
			{
				requestLine = String.Format("{0} http://{1}{2} {3}", Method, HostAndPort, PathAndQuery, HttpVersion);
			}
			return requestLine;
		}


	}
}
