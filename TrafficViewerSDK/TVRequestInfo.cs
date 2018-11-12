using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;

namespace TrafficViewerSDK
{
	/// <summary>
	/// Stores information about the request and its location in the traffic file
	/// </summary>
	public class TVRequestInfo
	{
		/// <summary>
		/// Contains culture information for time conversion
		/// </summary>
		private System.Globalization.DateTimeFormatInfo _dateTimeFormatInfo =
				new System.Globalization.DateTimeFormatInfo();

		private const int MAX_CUSTOM_FIELDS = 3;
		private readonly string[] NULL_STRING_ARRAY = new string[MAX_CUSTOM_FIELDS]
													{
														String.Empty,
														String.Empty,
														String.Empty
													};


		private int _id = -1;
		/// <summary>
		/// The unique index of the request header entry in the requests list
		/// </summary>
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		private string _requestLine;
		/// <summary>
		/// The HTTP Request line
		/// </summary>
		public string RequestLine
		{
			get
			{
				return _requestLine;
			}
			set
			{
				_requestLine = value;
			}
		}

		private DateTime _requestTime;
		/// <summary>
		/// The time of the request
		/// </summary>
		public DateTime RequestTime
		{
			get
			{
				return _requestTime;
			}
			set
			{
				_requestTime = value;
			}
		}

		private DateTime _responseTime;
		/// <summary>
		/// Time of the response
		/// </summary>
		public DateTime ResponseTime
		{
			get
			{
				return _responseTime;
			}
			set
			{
				_responseTime = value;
			}
		}

		private Dictionary<string, string> _customFields = null;
		/// <summary>
		/// List of custom fields to add to the request info
		/// </summary>
		public Dictionary<string, string> CustomFields
		{
			get
			{
				return _customFields;
			}
			set
			{
				_customFields = value;
			}
		}

		private string _responseStatus = null;
		/// <summary>
		/// Response status
		/// </summary>
		public string ResponseStatus
		{
			get
			{
				return _responseStatus;
			}
			set
			{
				_responseStatus = value;
			}
		}

		private string _threadId;
		/// <summary>
		/// The thread id
		/// </summary>
		public string ThreadId
		{
			get
			{
				return _threadId;
			}
			set
			{
				_threadId = value;
			}
		}

		private string _description;
		/// <summary>
		/// The description of the request info
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		/// The positon in the traffic data file where this request starts
		/// </summary>
		private long _requestStartPosition = 0;
		/// <summary>
		/// The request start position
		/// </summary>
		public long RequestStartPosition
		{
			get
			{
				return _requestStartPosition;
			}
			set
			{
				_requestStartPosition = value;
			}
		}

		private bool _isHttps = true;
		/// <summary>
		/// Whether the request was sent using https
		/// </summary>
		public bool IsHttps
		{
			get { return _isHttps; }
			set { _isHttps = value; }
		}


		private int _requestLength = 0;
		/// <summary>
		/// NUmber of bytes in the request
		/// </summary>
		public int RequestLength
		{
			get { return _requestLength; }
			set { _requestLength = value; }
		}

		/// <summary>
		/// The positon in the traffic data file where this response starts
		/// </summary>
		private long _responseStartPosition = 0;
		/// <summary>
		/// The positon in the traffic data file where this response starts
		/// </summary>
		public long ResponseStartPosition
		{
			get
			{
				return _responseStartPosition;
			}
			set
			{
				_responseStartPosition = value;
			}
		}

		private int _responseLength = 0;
		/// <summary>
		/// Number of bytes in the response
		/// </summary>
		public int ResponseLength
		{
			get { return _responseLength; }
			set { _responseLength = value; }
		}

		private string _domUniquenessId = String.Empty;
		/// <summary>
		/// Gets/sets value of dom uniqueness id, if calculated
		/// </summary>
		public string DomUniquenessId
		{
			get { return _domUniquenessId; }
			set { _domUniquenessId = value; }
		}

		private string _duration = String.Empty;
		/// <summary>
		/// How long was the request
		/// </summary>
		public string Duration
		{
			get
			{ 
				const string UNKNOWN_DURATION = "   ?";
				if (_duration == String.Empty || _duration == UNKNOWN_DURATION)
				{
					if (_responseTime == default(DateTime) || _requestTime == default(DateTime))
					{
						_duration = UNKNOWN_DURATION;
					}
					else
					{
						TimeSpan ts = _responseTime.Subtract(_requestTime);
						_duration = String.Format("{0,6:F2}s", ts.TotalSeconds);
					}
				}
				return _duration;
			}
			set { _duration = value; }
		}

		private int _refererId = -1;
		/// <summary>
		/// The traffic viewer id of the referer request
		/// </summary>
		public int RefererId
		{
			get { return _refererId; }
			set { _refererId = value; }
		}

		private string _updatedPath = String.Empty;
		/// <summary>
		/// Stores the last good value of the path
		/// </summary>
		public string UpdatedPath
		{
			get { return _updatedPath; }
			set { _updatedPath = value; }
		}
		
		

		private string _requestContext = String.Empty;
		/// <summary>
		/// Regular expression identifying the request in its context
		/// </summary>
		public string RequestContext
		{
			get { return _requestContext; }
			set { _requestContext = value; }
		}


		private string _trackingPattern = String.Empty;
		/// <summary>
		/// Gets the current tracking pattern
		/// </summary>
		public string TrackingPattern
		{
			get { return _trackingPattern; }
			set { _trackingPattern = value; }
		}

        private string _host;
        /// <summary>
        /// Gets the host
        /// </summary>
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        private string _validation;
        /// <summary>
        /// Gets the validation
        /// </summary>
        public string Validation
        {
            get { return _validation; }
            set { _validation = value; }
        }

        private bool _isEncrypted = false;
        /// <summary>
        /// Whether the request should be encrypted/decrypted on save
        /// </summary>
        public bool IsEncrypted
        {
            get { return _isEncrypted; }
            set { _isEncrypted = value; }
        }


		/// <summary>
		/// Parses an index line and reads all the values
		/// </summary>
		/// <param name="indexLine"></param>
		public void ReadValues(string indexLine)
		{
			string[] chunks = indexLine.Split(Constants.VALUES_SEPARATOR.ToCharArray());
			for (int i = 0; i < chunks.Length; i++)
			{
				switch (i)
				{
					case 0: _id = Convert.ToInt32(chunks[i]); break;
					case 1: _requestLine = chunks[i]; break;
					case 2: _responseStatus = chunks[i]; break;
					case 3: _threadId = chunks[i]; break;
					case 4: _description = chunks[i]; break;
					case 5: _requestTime = ReadDate(chunks[i]); break;
					case 6: _responseTime = ReadDate(chunks[i]); break;
					case 7: _requestStartPosition = Convert.ToInt64(chunks[i]); break;
					case 8: _requestLength = Convert.ToInt32(chunks[i]); break;
					case 9: _responseStartPosition = Convert.ToInt64(chunks[i]); break;
					case 10: _responseLength = Convert.ToInt32(chunks[i]); break;
					case 11: _domUniquenessId = chunks[i]; break;
					case 12: _duration = chunks[i]; break;
                    case 13: _isHttps = chunks[i].Equals("https") || chunks[i].Equals("true",StringComparison.OrdinalIgnoreCase); break;
					case 14: _refererId = Convert.ToInt32(chunks[i]); break;
					case 15: _requestContext = Utils.DecompressFromBase64String(chunks[i]); break;
					case 16: _updatedPath = chunks[i]; break;
					case 17: _trackingPattern = chunks[i]; break;
                    case 18: _host = chunks[i]; break;
                    case 19: _validation = chunks[i]; break;
                    case 20: _isEncrypted = Convert.ToBoolean(chunks[i]); break;
				}
			}
		}

		/// <summary>
		/// Convert string date time to traffic viewer format
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		private DateTime ReadDate(string time)
		{
			DateTime result = new DateTime();
			
			try
			{
				result = DateTime.ParseExact(time, Constants.COMMON_TIME_FORMAT, _dateTimeFormatInfo);
			}
			catch { }

			return result;
		}

		/// <summary>
		/// Converts the header to a string that can be written in an index file
		/// </summary>
		/// <param name="separator"></param>
        /// <param name="useForMatching">Whether ToString is called to match something like request highlighting. Includes attribute names (false by default)</param>
		/// <returns></returns>
		public string ToString(string separator, bool useForMatching = false)
		{
			StringBuilder sb = new StringBuilder(255);
            if (useForMatching) sb.Append("id=");
			sb.Append(_id);
			sb.Append(separator);
            if (useForMatching) sb.Append("request=");
			sb.Append(_requestLine);
			sb.Append(separator);
            if (useForMatching) sb.Append("status=");
			sb.Append(_responseStatus);
			sb.Append(separator);
            if (useForMatching) sb.Append("thread=");
			sb.Append(_threadId);
			sb.Append(separator);
            if (useForMatching) sb.Append("description=");
			sb.Append(_description);
			sb.Append(separator);
            if (useForMatching) sb.Append("req. time=");
			sb.Append(_requestTime.ToString(Constants.COMMON_TIME_FORMAT));
			sb.Append(separator);
            if (useForMatching) sb.Append("resp. time=");
            sb.Append(_responseTime.ToString(Constants.COMMON_TIME_FORMAT));
			
            if (!useForMatching)
            {
                sb.Append(separator);
                sb.Append(_requestStartPosition);
            }
            sb.Append(separator);
			sb.Append(_requestLength);
            if (!useForMatching)
            {
                sb.Append(separator);
                sb.Append(_responseStartPosition);
            }
			sb.Append(separator);
			sb.Append(_responseLength);
			sb.Append(separator);
			sb.Append(_domUniquenessId);
			sb.Append(separator);
			sb.Append(_duration);
			sb.Append(separator);
            if (useForMatching)
            {
                sb.Append("scheme=");
                sb.Append(_isHttps ? "https" : "http");
            }
            else
            {
                sb.Append(_isHttps);//need backward compatibility with ASE
            }
			sb.Append(separator);
			sb.Append(_refererId);
			sb.Append(separator);
            if (String.IsNullOrWhiteSpace(_requestContext))
            {
                sb.Append(_requestContext);
            }
            else
            {
                sb.Append(Utils.CompressToBase64String(_requestContext));
            }
			sb.Append(separator);
			sb.Append(_updatedPath);
			sb.Append(separator);
			sb.Append(_trackingPattern);
            sb.Append(separator);
            if (useForMatching) sb.Append("host=");
            sb.Append(_host);
            sb.Append(separator);
            sb.Append(_validation);
            sb.Append(separator);
            sb.Append(_isEncrypted);
			return sb.ToString();
		}

		/// <summary>
		/// Overriden ToString obtains tab separated values
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ToString(Constants.VALUES_SEPARATOR);
		}

		/// <summary>
		/// Gets the XML file for the custom fields
		/// </summary>
		/// <returns></returns>
		public XmlNode GetCustomFieldsXml()
		{
			XmlNode requestNode = null;
			if (_customFields != null)
			{
				XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.XmlResolver = null;
				requestNode = xmlDoc.CreateElement("Request");
				XmlNode fieldNode;
				foreach (KeyValuePair<string, string> field in _customFields)
				{
					fieldNode = xmlDoc.CreateElement("Field");
					fieldNode.Attributes.Append(xmlDoc.CreateAttribute("name"));
					fieldNode.Attributes["name"].Value = field.Key;
					fieldNode.InnerText = field.Value;
					requestNode.AppendChild(fieldNode);
				}
			}
			return requestNode;
		}

		/// <summary>
		/// Returns the custom fields value in the form of an array
		/// The values will be returned in the order of their definition and detection
		/// </summary>
		/// <param name="maxLength">The maximum size of the array</param>
		/// <returns></returns>
		public string[] GetCustomFieldsArray(int maxLength)
		{
			int i = 0;
			string[] result = NULL_STRING_ARRAY;
			if (_customFields != null)
			{
				foreach (string c in _customFields.Values)
				{
					result[i] = c;
					i++;
				}
			}
			return result;
		}
	}
}
