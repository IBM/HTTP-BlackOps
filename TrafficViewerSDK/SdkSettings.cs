using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViewerSDK.Http;

namespace TrafficViewerSDK
{
	/// <summary>
	/// Used to pass settings to the SDK
	/// </summary>
	public class SdkSettings
	{
		private static readonly object _lock = new object();

		private static SdkSettings _instance = null;
		/// <summary>
		/// Gets the SDK Settings instance
		/// </summary>
		public static SdkSettings Instance
		{
			get 
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = new SdkSettings();
					}
				}
				return _instance; 
			}			
		}

        private string _caPrivateKey = null;
        /// <summary>
        /// Gets/Sets the Blackops CA private key
        /// </summary>
        public string CAPrivateKey
        {
            get { return _caPrivateKey; }
            set { _caPrivateKey = value; }
        }


		private List<string> _responsePatterns;
		/// <summary>
		/// Gets/sets a list of session id response patterns
		/// </summary>
		public List<string> ResponsePatterns
		{
			get { return _responsePatterns; }
			set { _responsePatterns = value; }
		}

		private List<string> _sessionIdPatterns;
		/// <summary>
		/// Gets/sets a list of session id patterns
		/// </summary>
		public List<string> SessionIdPatterns
		{
			get { return _sessionIdPatterns; }
			set { _sessionIdPatterns = value; }
		}

		private List<HttpVariableDefinition> _variableDefinitions;
		/// <summary>
		/// Gets/sets a list of http variable definitions
		/// </summary>
		public List<HttpVariableDefinition> VariableDefinitions
		{
			get { return _variableDefinitions; }
			set { _variableDefinitions = value; }
		}


		private int _memoryBufferSize;
		/// <summary>
		/// Number of objects stored in memory by various caches
		/// </summary>
		public int MemoryBufferSize
		{
			get { return _memoryBufferSize; }
			set { _memoryBufferSize = value; }
		}


		Dictionary<string, TrafficViewerExtensionFunction> _extensionInfoList;
		/// <summary>
		/// Contains a list of available extensions
		/// </summary>
		public Dictionary<string, TrafficViewerExtensionFunction> ExtensionInfoList
		{
			get { return _extensionInfoList; }
			set { _extensionInfoList = value; }
		}


		int _httpRequestTimeout;
		/// <summary>
		/// Gets/Sets the timeout for an HTTP request in seconds
		/// </summary>
		public int HttpRequestTimeout
		{
			get { return _httpRequestTimeout; }
			set { _httpRequestTimeout = value; }
		}



		private int _requestDelay;
		/// <summary>
		/// Gets/Sets the request delay used for Stealth Mode
		/// </summary>
		public int RequestDelay
		{
			get { return _requestDelay; }
			set { _requestDelay = value; }
		}

		private string _requestDelayFilter = ".*";
		/// <summary>
		/// Gets/sets request delay filter
		/// </summary>
		public string RequestDelayFilter
		{
			get { return _requestDelayFilter; }
			set { _requestDelayFilter = value; }
		}

        private ILogWriter _logger;
        
        /// <summary>
        /// Logs messages
        /// </summary>
        public ILogWriter Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }


		private SdkSettings()
		{
			_extensionInfoList = new Dictionary<string, TrafficViewerExtensionFunction>();
			//no extensions loaded by default

			_httpRequestTimeout = 15;

			_memoryBufferSize = 1;

            _logger = new DefaultLogWriter();

			//initialize default values
			_responsePatterns = new List<string>();
			_responsePatterns.Add("(?:href|action|src)=[^>]+{0}=([^&\"'\\s>]+)");
			_responsePatterns.Add("name=[\"']?{0}[^>]value=[\"']?([^&\"'\\s>]+)");
			_responsePatterns.Add("var\\s+\\w+\\s?=\\s?[\"'][^\"']+{0}=([^&\"'\\s;]+)");
			_responsePatterns.Add("\"{0}\"\\s*:\\s*([^\"']+)");

			_sessionIdPatterns = new List<string>();
			_sessionIdPatterns.Add("ses|auth|token|key|sid");

			_variableDefinitions = new List<HttpVariableDefinition>();
			_variableDefinitions.Add(new HttpVariableDefinition("PathVariables", RequestLocation.Path, "([^&=;/?]{1,255})=([^&=;/?]*)"));
			_variableDefinitions.Add(new HttpVariableDefinition(HttpVariableDefinition.REGULAR_TYPE, RequestLocation.Query, "([^&=]{1,255})(?:=([^&=]*))?"));
			_variableDefinitions.Add(new HttpVariableDefinition(HttpVariableDefinition.REGULAR_TYPE, RequestLocation.Body, "([^&=]{1,255})=([^&=]*)"));
			_variableDefinitions.Add(new HttpVariableDefinition("JSON", RequestLocation.Body, "([^\"{}:,]{1,255})\"?\\s?:\\s?\"?([^\"{}:,]+)"));
			_variableDefinitions.Add(new HttpVariableDefinition("SOAP", RequestLocation.Body, @"<\s?([^<>\s/]{1,255})[^>]*>\s*([^<]*)</"));
			_variableDefinitions.Add(new HttpVariableDefinition(HttpVariableDefinition.REGULAR_TYPE, RequestLocation.Cookies, "([^&;=]{1,255})=([^&;]*)"));
            _variableDefinitions.Add(new HttpVariableDefinition("Echo", RequestLocation.Body, "<p[^>]*n=\"([^\"]+)\"[^>]*>\\s*([^<]*)</"));
            _variableDefinitions.Add(new HttpVariableDefinition("GWT", RequestLocation.Body, @"(?i)([a-z]+)\|([^|]+)"));
            _variableDefinitions.Add(new HttpVariableDefinition("encJSON", RequestLocation.Body, @"(?:%2C)?([\w\._\-]+)%3A[%2-7A-D""]*([^%]+)"));
            
		}


	
	}
}
