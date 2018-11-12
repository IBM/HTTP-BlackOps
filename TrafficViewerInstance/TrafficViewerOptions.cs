using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using TrafficViewerSDK.Http;
using TrafficViewerSDK.Properties;
using System.Reflection;
using TrafficViewerSDK;
using TrafficViewerSDK.Options;
using TrafficViewerSDK.Search;
using TrafficViewerInstance.Properties;
using System.Diagnostics;

namespace TrafficViewerInstance
{
	/// <summary>
	/// Retrieves the traffic viewer options
	/// </summary>
	public class TrafficViewerOptions : OptionsManager
	{

		/// <summary>
		/// Retrieves Application Data directory where the TrafficViewer options will be placed
		/// </summary>
		public static string TrafficViewerAppDataDir
		{
			get
			{
				string dir = System.Environment.GetEnvironmentVariable("userprofile");
				dir += "\\Application Data\\" + TrafficViewerConstants.APP_NAME;
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
				return dir;
			}
		}

		/// <summary>
		/// The action to be executed automatically on TVF close
		/// </summary>
		public int ActionOnClose
		{
			get
			{
				object value = GetOption("ActionOnClose");
				if (value == null) return 0;
				return Convert.ToInt32(value);
			}
			set
			{
				SetSingleValueOption("ActionOnClose", value);
			}
		}


		/// <summary>
		/// Amount of time to delay a http request, used for Stealth Mode
		/// </summary>
		public int RequestDelay
		{
			get
			{
				object value = GetOption("RequestDelay");
				if (value == null) return 0;
				return Convert.ToInt32(value);
			}
			set
			{
				SetSingleValueOption("RequestDelay", value);
			}
		}


		/// <summary>
		/// Http timeout
		/// </summary>
		public int HttpRequestTimeout
		{
			get
			{
				object value = GetOption("HttpRequestTimeout");
				if (value == null) return 15;
				return Convert.ToInt32(value);
			}
			set
			{
				SetSingleValueOption("HttpRequestTimeout", value);
			}
		}

		/// <summary>
		/// If a proxy should be used for sending http requests
		/// </summary>
		public bool UseProxy
		{
			get
			{
				object value = GetOption("UseProxy");
				if (value == null) return false;
				return Convert.ToBoolean(value);
			}
			set
			{
				SetSingleValueOption("UseProxy", value);
			}
		}

		

        /// <summary>
        /// Whether to ignore invalid SSL certificates
        /// </summary>
        public bool IgnoreInvalidSslCert
        {
            get
            {
                object value = GetOption("IgnoreInvalidSslCert");
                if (value == null) return true;
                return Convert.ToBoolean(value);
            }
            set
            {
                SetSingleValueOption("IgnoreInvalidSslCert", value);
            }
        }



		/// <summary>
		/// Http proxy server address
		/// </summary>
		public string HttpProxyServer
		{
			get
			{
				object value = GetOption("HttpProxyServer");
				if (value == null) return String.Empty;
				return value as string;
			}
			set
			{
				SetSingleValueOption("HttpProxyServer", value);
			}
		}


        /// <summary>
        /// Private key used to sign certificates
        /// </summary>
        public string CACertPrivKey
        {
            get
            {
                object value = GetOption("CACertPrivKey");
                if (value == null) return String.Empty;
                return value as string;
            }
            set
            {
                SetSingleValueOption("CACertPrivKey", value);
            }
        }

        /// <summary>
        /// The base64 encoded CER
        /// </summary>
        public string CACertCer
        {
            get
            {
                object value = GetOption("CACertCer");
                if (value == null) return String.Empty;
                return value as string;
            }
            set
            {
                SetSingleValueOption("CACertCer", value);
            }
        }


		/// <summary>
		/// Request Delay Filter
		/// </summary>
		public string RequestDelayFilter
		{
			get
			{
				object value = GetOption("RequestDelayFilter");
				if (value == null) return ".*";
				return value as string;
			}
			set
			{
				SetSingleValueOption("RequestDelayFilter", value);
			}
		}

		/// <summary>
		/// Http proxy server address
		/// </summary>
		public string ProxyCert
		{
			get
			{
				object value = GetOption("ProxyCert");
				if (value == null) return String.Empty;
				return value as string;
			}
			set
			{
				SetSingleValueOption("ProxyCert", value);
			}
		}

		/// <summary>
		/// Http proxy server address
		/// </summary>
		public string ProxyCertPass
		{
			get
			{
				object value = GetOption("ProxyCertPass");
				string pass = value as string;
				if (pass == null) return String.Empty;

				pass = Encryptor.DecryptToString(pass);
				return pass;
			}
			set
			{
				SetSingleValueOption("ProxyCertPass", Encryptor.EncryptToString(value));
			}
		}


		/// <summary>
		/// Http proxy port
		/// </summary>
		public int HttpProxyPort
		{
			get
			{
				object value = GetOption("HttpProxyPort");
				if (value == null) return 0;
				return Convert.ToInt32(value);
			}
			set
			{
				SetSingleValueOption("HttpProxyPort", value);
			}
		}

		/// <summary>
		/// Textbox background
		/// </summary>
		public string ColorTextboxBackground
		{
			get
			{
				object value = GetOption("ColorTextboxBackground");
				if (value == null) return "Black";
				return (string)value;
			}
			set
			{
				SetSingleValueOption("ColorTextboxBackground", value);
			}
		}


		/// <summary>
		/// Textbox text color
		/// </summary>
		public string ColorTextboxText
		{
			get
			{
				object value = GetOption("ColorTextboxText");
				if (value == null) return "Lime";
				return (string)value;
			}
			set
			{
				SetSingleValueOption("ColorTextboxText", value);
			}
		}

		/// <summary>
		/// Diff text color
		/// </summary>
		public string ColorDiffText
		{
			get
			{
				object value = GetOption("ColorDiffText");
				if (value == null) return "Red";
				return (string)value;
			}
			set
			{
				SetSingleValueOption("ColorDiffText", value);
			}
		}

		/// <summary>
		/// Highlight color
		/// </summary>
		public string ColorHighlight
		{
			get
			{
				object value = GetOption("ColorHighlight");
				if (value == null) return "Red";
				return (string)value;
			}
			set
			{
				SetSingleValueOption("ColorHighlight", value);
			}
		}


        /// <summary>
        /// Gets a list of traps
        /// </summary>
        public List<HttpTrapDef> GetTraps()
        {

            List<string> values = (List<string>)GetOption("Traps");
            List<HttpTrapDef> traps = new List<HttpTrapDef>();

            string[] vals;
            if (values != null)
            {
                foreach (string v in values)
                {
                    vals = v.Split(Constants.VALUES_SEPARATOR.ToCharArray());
                    if (vals.Length == 3)
                    {
                        traps.Add(new HttpTrapDef(vals[0], vals[1], vals[2]));
                    }
                }
            }
            else
            {
                traps.Add(new HttpTrapDef("Enabled", "Request", "."));
                traps.Add(new HttpTrapDef("Enabled", "Response", "."));
                traps.Add(new HttpTrapDef("Disabled", "Request", "^(POST|PUT)"));

            }
            return traps;
        }

        /// <summary>
        /// Stores the current trap options
        /// </summary>
        /// <param name="hDefs">Collection of description partial match/color</param>
        public void SetTraps(IEnumerable<HttpTrapDef> defs)
        {
            List<string> values = new List<string>();
            foreach (HttpTrapDef def in defs)
            {
                values.Add(def.ToString());
            }
            SetMultiValueOption("Traps", values);
        }


		/// <summary>
		/// Installation directory
		/// </summary>
		public string InstallDir
		{
			get
			{
				object value = GetOption("InstallDir");
				if (value == null) return Directory.GetCurrentDirectory() + "\\";
				return (string)value;
			}
			set
			{
				SetSingleValueOption("InstallDir", value);
			}
		}

		/// <summary>
		/// Gets or sets the path of th file tpo be automatically imported on startup
		/// </summary>
		public string StartupImport
		{
			get
			{
				object value = GetOption("DefaultImport");
				const string APPSCAN_TRAFFIC_LOG = @"C:\Program Files\IBM\Rational AppScan\Logs\AppScanTraffic.log";
				if (value == null)
				{
					if (File.Exists(APPSCAN_TRAFFIC_LOG))
					{
						value = APPSCAN_TRAFFIC_LOG;
					}
					else
					{
						value = String.Empty;
					}
				}
				return value as string;
			}
			set
			{
				SetSingleValueOption("DefaultImport", value);
			}
		}

		/// <summary>
		/// Gets or sets the profile for the startup import
		/// </summary>
		public string StartupImportProfile
		{
			get
			{
				object value = GetOption("StartupImportProfile");
				if (value == null) return String.Empty;
				return (string)value;
			}
			set
			{
				SetSingleValueOption("StartupImportProfile", value);
			}
		}


		/// <summary>
		/// Gets or sets the last profile selected by the user
		/// </summary>
		public string SelectedProfile
		{
			get
			{
				object value = GetOption("SelectedProfile");
				if (value == null) return "AppScanAndPolicyTester.xml";
				return (string)value;
			}
			set
			{
				SetSingleValueOption("SelectedProfile", value);
			}
		}

		/// <summary>
		/// Gets or sets the last parser selected by the user
		/// </summary>
		public string SelectedParser
		{
			get
			{
				string value = GetOption("SelectedParser") as string;

				return value;
			}
			set
			{
				SetSingleValueOption("SelectedParser", value);
			}
		}


		/// <summary>
		/// Gets or sets the parser that should be used at startup 
		/// </summary>
		public string StartupParser
		{
			get
			{
				string value = GetOption("StartupParser") as string;

				return value;
			}
			set
			{
				SetSingleValueOption("StartupParser", value);
			}
		}

		/// <summary>
		/// Gets or sets the http client name
		/// </summary>
		public string HttpClientName
		{
			get
			{
				string value = GetOption("HttpClientName") as string;
                if (String.IsNullOrEmpty(value))
                {
                    value = Resources.WebRequestClientName;
                }
				return value;
			}
			set
			{
				SetSingleValueOption("HttpClientName", value);
			}
		}

		/// <summary>
		/// Gets or sets the search type selected by the user
		/// </summary>
		public int LastSearchType
		{
			get
			{
				int value;

				if (!int.TryParse(GetOption("LastSearchType") as string, out value))
				{
					value = (int)SearchContext.Full;
				}
				return value;
			}
			set
			{
				SetSingleValueOption("LastSearchType", (int)value);
			}
		}

		/// <summary>
		/// This how long a line usually is. It is the amount of bytes allocated initially for a line during read.
		/// </summary>
		public int EstimatedLineSize
		{
			get
			{
				object value = GetOption("EstimatedLineSize");
				if (value == null)
				{
					return 1024;
					//the statistical mode averages between 52 and 90 although lines
					//containing more than 1000 chars are also found often in binary content
				}
				return Convert.ToInt32(value);
			}
			set
			{
				SetSingleValueOption("EstimatedLineSize", value);
			}
		}

		/// <summary>
		/// The number of events to be cached before being dislayed to the user
		/// </summary>
		public int RequestListEventsQueueSize
		{
			get
			{
				object value = GetOption("RequestListEventsQueueSize");
				if (value == null)
				{
					return 1000;
				}
				return Convert.ToInt32(value);
			}
			set
			{
				SetSingleValueOption("RequestListEventsQueueSize", value);
			}
		}

		/// <summary>
		/// Whether to defrag the raw log on save
		/// </summary>
		public bool DefragOnSave
		{
			get
			{
				object value = GetOption("DefragOnSave");
				if (value == null)
				{
					return false;
				}
				return Convert.ToBoolean(value);
			}
			set
			{
				SetSingleValueOption("DefragOnSave", value);
			}
		}


		object _loadFirebug = null;
		/// <summary>
		/// Whether to load firebug lite for in browser friendly mode of Traffic Server
		/// </summary>
		public bool LoadFirebugInBrowserFriendlyMode
		{
			get
			{
				if (_loadFirebug == null)
				{
					_loadFirebug = GetOption("LoadFirebugInBrowserFriendlyMode");
				}

				if (_loadFirebug == null)
				{
					_loadFirebug = false;
				}
				return Convert.ToBoolean(_loadFirebug);
			}
			set
			{
				SetSingleValueOption("LoadFirebugInBrowserFriendlyMode", value);
				_loadFirebug = value;
			}
		}

		private List<string> _proxyBypasses;
		/// <summary>
		/// Gets a list of hosts that the proxy will bypass
		/// </summary>
		public List<string> ProxyBypassPrefixes
		{
			get
			{
				if (_proxyBypasses == null)
				{
					object value = GetOption("ProxyBypassPrefixes");
					_proxyBypasses = value as List<string>;
					if (_proxyBypasses == null)
					{
						_proxyBypasses = new List<string>();
						_proxyBypasses.Add("https://getfirebug.com");
					}
				}
				return _proxyBypasses;
			}
			set
			{
				_proxyBypasses = value;
				SetMultiValueOption("ProxyBypassPrefixes", _proxyBypasses);
			}
		}


		/// <summary>
		/// The maximum number of requests to be loaded to the GUI at a time during a load
		/// </summary>
		public int LoadMaxAdvanceSize
		{
			get
			{
				object value = GetOption("LoadMaxAdvanceSize");
				if (value == null)
				{
					return 500;
				}
				return Convert.ToInt32(value);
			}
			set
			{
				SetSingleValueOption("LoadMaxAdvanceSize", value);
			}
		}

		/// <summary>
		/// Value used by various buffers to limit the amount of objects that are stored in the memory
		/// </summary>
		public int MemoryBufferSize
		{
			get
			{
				object value = GetOption("RequestsBufferSize");
				if (value == null) return 1;
				return Convert.ToInt32(value);
			}
			set
			{
				SetSingleValueOption("RequestsBufferSize", value);
			}
		}

		/// <summary>
		/// How many requests should be loaded at a time during a tail operation
		/// </summary>
		public int TailChunk
		{
			get
			{
				object value = GetOption("TailChunk");
				if (value == null) return 1;
				return Convert.ToInt32(value);
			}
			set
			{
				SetSingleValueOption("TailChunk", value);
			}
		}

		/// <summary>
		/// The number requests to be loaded to the GUI at a time during a tail
		/// </summary>
		public int TailAdvanceSize
		{
			get
			{
				object value = GetOption("TailAdvanceSize");
				if (value == null)
				{
					return 3;
				}
				return Convert.ToInt32(value);
			}
			set
			{
				SetSingleValueOption("TailAdvanceSize", value);
			}
		}

		/// <summary>
		/// Gets or sets the ip of the traffic server proxy
		/// </summary>
		public string TrafficServerIp
		{
			get
			{
                object value = GetOption("TrafficServerIp");
				if (value == null)
				{
					return "127.0.0.1";
				}
				return (string)value;
			}
			set
			{
                SetSingleValueOption("TrafficServerIp", value);
			}
		}

        /// <summary>
        /// Gets or sets a host to forward requests to if they are not found in the traffic log
        /// </summary>
        public string ForwardingHost
        {
            get
            {
                object value = GetOption("ForwardingHost");
                if (value == null)
                {
                    return String.Empty;
                }
                return (string)value;
            }
            set
            {
                SetSingleValueOption("ForwardingHost", value);
            }
        }

		/// <summary>
		/// Gets/sets the port that the traffic server listens to
		/// </summary>
		public int TrafficServerPort
		{
			get
			{
                object value = GetOption("TrafficServerPort");
				if (value == null) return 9999;
				return Convert.ToInt32(value);
			}
			set
			{
                SetSingleValueOption("TrafficServerPort", value);
			}
		}

        /// <summary>
        /// Gets/sets the proxy port for secure connections
        /// </summary>
        public int TrafficServerPortSecure
        {
            get
            {
                object value = GetOption("TrafficServerPortSecure");
                if (value == null) return 19999;
                return Convert.ToInt32(value);
            }
            set
            {
                SetSingleValueOption("TrafficServerPortSecure", value);
            }
        }


        /// <summary>
        /// Gets or sets the port of a host to forward requests to if they are not found in the traffic log
        /// </summary>
        public int ForwardingPort
        {
            get
            {
                object value = GetOption("ForwardingPort");
                if (value == null) return 0;
                return Convert.ToInt32(value);
            }
            set
            {
                SetSingleValueOption("ForwardingPort", value);
            }
        }

		

		
		/// <summary>
		/// Gets the list of dynamic elements that should be added to request normalization
		/// </summary>
		/// <returns>Path to dll</returns>
		public List<string> GetDynamicElements()
		{
			List<string> values = (List<string>)GetOption("DynamicElements");
			if (values == null)
			{
				values = new List<string>();
				values.Add(@"alert[^/]*?(\d+)");
				values.Add(@"MAC_ADDRESS_([\dA-F]+)");
				values.Add(@"(?:ses|ra?nd|id)[\w]+=([^&?=;\n]{10,64})");
			}

			return values;
		}

		/// <summary>
		/// Stores the list of dynamic elements
		/// </summary>
		/// <param name="parserPaths">List of dynamic elements</param>
		public void SetDynamicElements(IEnumerable<string> dynamicElements)
		{
			SetMultiValueOption("DynamicElements", dynamicElements);
		}

		/// <summary>
		/// Retrieves the list of custom analysis modules
		/// </summary>
		public Dictionary<string,TrafficViewerExtensionFunction> GetExtensionInfo()
		{
			List<string> extensionDefs = new List<string>();
			object value = GetOption("Extensions");

			Dictionary<string,TrafficViewerExtensionFunction> extensions = new Dictionary<string,TrafficViewerExtensionFunction>();

			if (value != null)
			{
				extensionDefs = value as List<string>;
				foreach (string def in extensionDefs)
				{
					string[] defParts = def.Split(TrafficViewerSDK.Constants.VALUES_SEPARATOR.ToCharArray());
					if (defParts.Length == 2)
					{
						extensions.Add(defParts[1],(TrafficViewerExtensionFunction)Enum.Parse(typeof(TrafficViewerExtensionFunction),
							defParts[0]));
					}

				}
			}
			else
			{
				Dictionary<string, TrafficViewerExtensionFunction> defaultExtensions = new Dictionary<string, TrafficViewerExtensionFunction>();

				//defaultExtensions.Add("ScanFileParser.dll", TrafficViewerExtensionFunction.TrafficParser);

				foreach (string dllName in defaultExtensions.Keys)
				{
					string installDir = InstallDir;
					string dllFullPath = String.Format("{0}\\{1}", installDir, dllName);
					if (File.Exists(dllFullPath))
					{
						extensionDefs.Add(String.Format("{0}\t{1}", defaultExtensions[dllName] ,dllFullPath));
						extensions.Add(dllFullPath, defaultExtensions[dllName]);
					}
					else
					{
                        SdkSettings.Instance.Logger.Log(TraceLevel.Warning, "TrafficViewerOptions - Cannot find extension: {0}", dllFullPath);
					}
				}

				this.SetExtensions(extensionDefs);
				this.Save();
			}
			return extensions;
		}



		/// <summary>
		/// Stores the list of available extensions
		/// </summary>
		/// <param name="extensions">List of analysis modules</param>
		public void SetExtensions(IEnumerable<string> extensions)
		{
			SetMultiValueOption("Extensions", extensions);
		}



		/// <summary>
		/// Gets the state of the request list columns
		/// </summary>
		public Dictionary<string, bool> GetRequestListColumns()
		{

			List<string> values = (List<string>)GetOption("RequestListColumns");
			Dictionary<string, bool> result = new Dictionary<string, bool>();
			string[] pair;
			if (values != null)
			{
				foreach (string v in values)
				{
					pair = v.Split(TrafficViewerSDK.Constants.VALUES_SEPARATOR.ToCharArray());
					if (pair.Length > 1)
					{
						result.Add(pair[0], Convert.ToBoolean(pair[1]));
					}
				}
			}
			else
			{
				result.Add("_id", true);
				result.Add("_scheme", true);
				result.Add("_requestLine", true);
				result.Add("_status", true);
				
				result.Add("_description", true);
				result.Add("_reqTime", true);
				result.Add("_duration", true);
				result.Add("_respSize", true);
				result.Add("_thread", false);
			}

			return result;
		}

		/// <summary>
		/// Stores the state of the request list columns
		/// </summary>
		/// <param name="columnsState">Dictionary of columnId/visibility</param>
		public void SetRequestListColumns(Dictionary<string, bool> columnsState)
		{
			List<string> values = new List<string>();
			foreach (KeyValuePair<string, bool> pair in columnsState)
			{
				values.Add(pair.Key + TrafficViewerSDK.Constants.VALUES_SEPARATOR + pair.Value.ToString());
			}
			SetMultiValueOption("RequestListColumns", values);
		}

		/// <summary>
		/// Gets the default profile
		/// </summary>
		/// <returns></returns>
		public ParsingOptions GetDefaultProfile()
		{
		    string profilePath = TrafficViewerAppDataDir + "\\Profiles\\Default.xml";
            ParsingOptions newDefaultProfile = ParsingOptions.GetDefaultProfile();

			if (!File.Exists(profilePath))
			{
				return ParsingOptions.GetDefaultProfile();
			}
			else
			{
				                
                //check the version on the default profile
                ParsingOptions existingDefaultProfile = new ParsingOptions();
                existingDefaultProfile.Load(profilePath);
                

                if (newDefaultProfile.ProfileVersion > existingDefaultProfile.ProfileVersion)
                {
                    //there is a newwer default profile overwrite the old one
                    newDefaultProfile.SaveAs(profilePath);
                    return newDefaultProfile;
                }

                return existingDefaultProfile;
			}
		}

		/// <summary>
		/// Gets the list of available parsing profiles
		/// </summary>
		/// <returns></returns>
		public List<string> GetProfilePaths()
		{
			List<string> values = (List<string>)GetOption("Profiles");
            string profileDir = TrafficViewerAppDataDir + "\\Profiles";
            string defaultProfilePath = profileDir + "\\Default.xml";
            if (values == null)
            {
                values = new List<string>();

                //save the ASE, AppScan and the raw log profiles to the default profiles directory

                

                // first create the profiles directory if it doesn't exist
                if (!Directory.Exists(profileDir))
                {
                    Directory.CreateDirectory(profileDir);
                }

                //ASE profile
                ParsingOptions profile = ParsingOptions.GetDefaultProfile();
                string profilePath = defaultProfilePath;
                profile.SaveAs(profilePath);
                values.Add(profilePath);

                //appscan profile
                profile = ParsingOptions.GetLegacyAppScanProfile();
                profilePath = profileDir + "\\AppScan8.5AndEarlierProfile.xml";
                profile.SaveAs(profilePath);
                values.Add(profilePath);


                //raw profile
                profile = ParsingOptions.GetRawProfile();
                profilePath = profileDir + "\\RawProfile.xml";
                profile.SaveAs(profilePath);
                values.Add(profilePath);

                //save these options so we don't go through this again
                this.SetProfilePaths(values);
                this.Save();
            }
           
			return values;
		}

		/// <summary>
		/// Stores the list of available profiles
		/// </summary>
		/// <param name="parserPaths">List of profile paths</param>
		public void SetProfilePaths(IEnumerable<string> profilePaths)
		{
			SetMultiValueOption("Profiles", profilePaths);
		}


		/// <summary>
		/// Gets the list of session id regular expressions
		/// </summary>
		/// <returns></returns>
		public List<string> GetSessionIdRegexes()
		{
			List<string> values = (List<string>)GetOption("SessionIdRegexes");
			if (values == null)
			{
				values = new List<string>();
				values.Add("ses|auth|token|key|sid");
				
			}
			return values;
		}

		/// <summary>
		/// Stores the list of regular expression definitions
		/// </summary>
		/// <param name="values"></param>
		public void SetSessionIdRegexes(IEnumerable<string> values)
		{
			SetMultiValueOption("SessionIdRegexes", values);
		}

		/// <summary>
		/// Gets the list of parameter response patterns
		/// </summary>
		/// <returns></returns>
		public List<string> GetResponsePatterns()
		{
			List<string> values = (List<string>)GetOption("ResponsePatterns");
			if (values == null)
			{
				values = new List<string>();
				values.Add("(?:href|action|src)=[^>]+{0}=([^&\"'\\s>]+)");
				values.Add("name=[\"']?{0}[^>]value=[\"']?([^&\"'\\s>]+)");
				values.Add("var\\s+\\w+\\s?=\\s?[\"'][^\"']+{0}=([^&\"'\\s;]+)");
				values.Add("\"{0}\"\\s*:\\s*([^\"']+)");
				
			}
			return values;
		}

		/// <summary>
		/// Stores the list of parameter response patterns
		/// </summary>
		/// <param name="values"></param>
		public void SetResponsePatterns(IEnumerable<string> values)
		{
			SetMultiValueOption("ResponsePatterns", values);
		}


		private List<HttpVariableDefinition> _variableDefinitions = null;
		/// <summary>
		/// Gets/sets the variable definitions 
		/// </summary>
		public List<HttpVariableDefinition> VariableDefinitions
		{
			get
			{
				if (_variableDefinitions == null)
				{
					//retrieves a list of location string tab regex
					List<string> values = (List<string>)GetOption("VariableDefinitions");
					_variableDefinitions = new List<HttpVariableDefinition>();
					if (values != null)
					{
						foreach (string v in values)
						{
							_variableDefinitions.Add(new HttpVariableDefinition(v));
						}
					}
					else
					{
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
				return _variableDefinitions;
			}
			set
			{

				_variableDefinitions = value;

				List<string> values = new List<string>();

				if (_variableDefinitions != null)
				{
					foreach (HttpVariableDefinition def in _variableDefinitions)
					{
						values.Add(def.ToString());
					}

					SetMultiValueOption("VariableDefinitions", values);
				}
			}
		}




		public override void Save()
		{
			//save the variable definitions, the following commits any direct operations on the
			//variable definitions collection to the settings XML
			this.VariableDefinitions = _variableDefinitions;
			base.Save();
		}

		#region Instance creation and destruction

		/// <summary>
		/// Lock to prevent multiple threads from getting the instance at the same time
		/// </summary>
		private static object _instanceLock = new object();

		/// <summary>
		/// The instance
		/// </summary>
		private static TrafficViewerOptions __instance = null;

		/// <summary>
		/// Exposes the singleton
		/// </summary>
		public static TrafficViewerOptions Instance
		{
			get
			{
				lock (_instanceLock)
				{
					if (__instance == null)
					{
						__instance = new TrafficViewerOptions();
					}
				}
				return __instance;
			}
		}

		private TrafficViewerOptions()
		{
			_optionsDocPath = TrafficViewerAppDataDir + "\\" + TrafficViewerConstants.OPTIONS_FILE_NAME;
			Load(_optionsDocPath);
		}

		~TrafficViewerOptions()
		{
			Save();
		}

		#endregion
	}

}
