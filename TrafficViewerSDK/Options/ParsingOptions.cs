/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace TrafficViewerSDK.Options
{
	/// <summary>
	/// Used to access and save parsing settings
	/// </summary>
	public class ParsingOptions : OptionsManager
	{
		

		/// <summary>
		/// Regular expression identifying a sending request line
		/// </summary>
		public string SendingRequestRegex
		{
			get
			{
				string value = (string)GetOption("SendingRequestRegex");
				if (value == null) return @"Sending request.*?length.*?(\d+)";
				return value;
			}
			set
			{
				SetSingleValueOption("SendingRequestRegex", value);
			}
		}


		/// <summary>
		/// Regular expression identifying a sending request line
		/// </summary>
		public string ReceivingResponseRegex
		{
			get
			{
				string value = (string)GetOption("ReceivingResponseRegex");
				if (value == null) return @"Receiving the response.*?length.*?(\d+)";
				return value;
			}
			set
			{
				SetSingleValueOption("ReceivingResponseRegex", value);
			}
		}

		/// <summary>
		/// Regular expression identifying the thread id, must contain a grouping ()
		/// Example regex: Thread Id:([\d+])
		/// If non-existent the log will be parsed by a single thread
		/// </summary>
		public string ThreadIdRegex
		{
			get
			{
				string value = (string)GetOption("ThreadIdRegex");
				if (value == null) return String.Empty;
				return value;
			}
			set
			{
				SetSingleValueOption("ThreadIdRegex", value);
			}
		}

		/// <summary>
		/// Regular expression identifying the line that starts a thread
		/// Example regex: Begin Thread
		/// If non-existent the log will be parsed by a single thread
		/// </summary>
		public string BeginThreadRegex
		{
			get
			{
				string value = (string)GetOption("BeginThreadRegex");
				if (value == null) return String.Empty;
				return value;
			}
			set
			{
				SetSingleValueOption("BeginThreadRegex", value);
			}
		}

		/// <summary>
		/// Regular expression identifying the line that ends a thread
		/// Example regex: End Thread
		/// If non-existent the threads will keep parsing the file until a BeginThread will be found
		/// </summary>
		public string EndThreadRegex
		{
			get
			{
				string value = (string)GetOption("EndThreadRegex");
				if (value == null) return String.Empty;
				return value;
			}
			set
			{
				SetSingleValueOption("EndThreadRegex", value);
			}
		}

		/// <summary>
		/// Regular expression identifying the description of the thread
		/// </summary>
		public string DescriptionRegex
		{
			get
			{
				string value = (string)GetOption("DescriptionRegex");
				if (value == null) return String.Empty;
				return value;
			}
			set
			{
				SetSingleValueOption("DescriptionRegex", value);
			}
		}

		/// <summary>
		/// Defines a request line
		/// </summary>
		public string RequestLineRegex
		{
			get
			{
				string value = (string)GetOption("RequestLineRegex");
				if (value == null) return @"^(GET|POST|PUT|TRACE|TRACK|DEBUG|HEAD|POLL|CONNECT|PROPFIND|PROPPATCH|MKCOL|DELETE|COPY|MOVE|LOCK|UNLOCK|OPTIONS|SEARCH).*HTTP/";
				return value;
			}
			set
			{
				SetSingleValueOption("RequestLineRegex", value);
			}
		}

		/// <summary>
		/// Defines a response line regex
		/// </summary>
		public string ResponseLineRegex
		{
			get
			{
				string value = (string)GetOption("ResponseLineRegex");
				if (value == null) return @"^HTTP\/1\.\d\s";
				return value;
			}
			set
			{
				SetSingleValueOption("ResponseLineRegex", value);
			}
		}

		/// <summary>
		/// Defines the http status code pattern
		/// </summary>
		public string ResponseStatusRegex
		{
			get
			{
				string value = (string)GetOption("ResponseStatusRegex");
				if (value == null) return @"^HTTP.*?\b(\d{3})\b";
				return value;
			}
			set
			{
				SetSingleValueOption("ResponseStatusRegex", value);
			}
		}

		/// <summary>
		/// When the traffic is multithreaded if a thread contains only one request it is difficult to detect the end of the response
		/// unless we read a message from the log indicating the response was received
		/// </summary>
		public string ResponseReceivedMessageRegex
		{
			get
			{
				string value = (string)GetOption("ResponseReceivedMessageRegex");
				if (value == null) return @"====>.*finished getting the response";
				return value;
			}
			set
			{
				SetSingleValueOption("ResponseReceivedMessageRegex", value);
			}
		}

		/// <summary>
		/// The connection to site string indicates sets of duplicate request
		/// response pairs in the same thread in the case of AppScan manual explore
		/// </summary>
		public string ProxyConnectionToSiteRegex
		{
			get
			{
				string value = (string)GetOption("ProxyConnectionToSiteRegex");
				if (value == null) return @"connection\s?to\s?site";
				return value;
			}
			set
			{
				SetSingleValueOption("ProxyConnectionToSiteRegex", value);
			}
		}

		/// <summary>
		/// Identifies lines that are not part of the HTTP traffic, for example messaging
		/// Example ====>
		/// </summary>
		public string NonHttpTrafficRegex
		{
			get
			{
				string value = (string)GetOption("NonHttpTrafficRegex");
				if (value == null) return String.Empty;
				return value;
			}
			set
			{
				SetSingleValueOption("NonHttpTrafficRegex", value);
			}
		}

		/// <summary>
		/// Extracts timestamps from the traffic log
		/// </summary>
		public string TimeFormat
		{
			get
			{
				string value = (string)GetOption("TimeFormat");
				if (value == null) return @"ddd, dd MMM yy, HH:mm:ss:fff";
				return value;
			}
			set
			{
				SetSingleValueOption("TimeFormat", value);
			}
		}

		/// <summary>
		/// Master switch for the exclusions
		/// </summary>
		public bool UseExclusions
		{
			get
			{
				string value = (string)GetOption("UseExclusions");
				if (value == null) return true;
				return Convert.ToBoolean(value);
			}
			set
			{
				SetSingleValueOption("UseExclusions", value);
			}
		}


		/// <summary>
		/// Defines a List of request exclusions specified by regular expressions. The exclusions apply to the request line
		/// </summary>
		public IEnumerable<string> GetExclusions()
		{
			object value = GetOption("Exclusions");
			IEnumerable<string> exclusions = value as IEnumerable<string>;
			if (exclusions == null)
			{
				exclusions = new List<string>();
			}
			return exclusions;
		}

		/// <summary>
		/// Stores the exclusions values
		/// </summary>
		/// <param name="exclusions"></param>
		public void SetExclusions(IEnumerable<string> exclusions)
		{
			SetMultiValueOption("Exclusions", exclusions);
		}

		/// <summary>
		/// Defines a list of custom fields that can be extracted from anywhere in the traffic
		/// </summary>
		/// <returns>A list of name/regex custom fields definitions</returns>
		public Dictionary<string, string> GetCustomFields()
		{
			List<string> values = (List<string>)GetOption("CustomFields");
			Dictionary<string, string> result = new Dictionary<string, string>();
			string[] pair;
			if (values != null)
			{
				foreach (string v in values)
				{
					pair = v.Split(Constants.VALUES_SEPARATOR.ToCharArray());
					if (pair.Length > 1)
					{
						result.Add(pair[0], pair[1]);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Stores custom defined field definitions
		/// </summary>
		/// <param name="customFields">Collection of name/regex pairs</param>
		public void SetCustomFields(Dictionary<string, string> customFields)
		{
			List<string> values = new List<string>();
			foreach (KeyValuePair<string, string> pair in customFields)
			{
				values.Add(pair.Key + Constants.VALUES_SEPARATOR + pair.Value);
			}
			SetMultiValueOption("CustomFields", values);
		}




        /// <summary>
        /// Defines a list of pattern replacements used to track dynamic values
        /// </summary>
        /// <returns>A list of name/regex custom fields definitions</returns>
        public Dictionary<string, TrackingPattern> GetTrackingPatterns()
        {
            List<string> values = (List<string>)GetOption("TrackingPatterns");
            Dictionary<string, TrackingPattern> result = new Dictionary<string, TrackingPattern>();
            string[] pair;
            if (values != null)
            {
                foreach (string v in values)
                {
                    pair = v.Split(Constants.VALUES_SEPARATOR.ToCharArray());
                    if (pair.Length == 4)
                    {
                        result.Add(pair[0], new TrackingPattern(pair[0], pair[1], 
							(TrackingType)Enum.Parse(typeof(TrackingType),pair[2]) , pair[3]));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Stores a list of pattern replacements used to track dynamic values
        /// </summary>
        /// <param name="trackingPatterns">Collection of name/regex pairs</param>
        public void SetTrackingPatterns(Dictionary<string, TrackingPattern> trackingPatterns)
        {
            List<string> values = new List<string>();
            foreach (TrackingPattern def in trackingPatterns.Values)
            {
                values.Add(def.ToString());
            }
            SetMultiValueOption("TrackingPatterns", values);
        }



		/// <summary>
		/// Defines a list of custom fields that can be extracted from anywhere in the traffic
		/// </summary>
		/// <returns>A list of name/regex custom fields definitions</returns>
		public Dictionary<string, string> GetRequestReplacements()
		{
			List<string> values = (List<string>)GetOption("RequestReplacements");
			Dictionary<string, string> result = new Dictionary<string, string>();
			string[] pair;
			if (values != null)
			{
				foreach (string v in values)
				{
					pair = v.Split(Constants.VALUES_SEPARATOR.ToCharArray());
					if (pair.Length > 1)
					{
						result.Add(pair[0], pair[1]);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Stores custom defined field definitions
		/// </summary>
		/// <param name="Replacements">Collection of name/regex pairs</param>
		public void SetRequestReplacements(Dictionary<string, string> Replacements)
		{
			List<string> values = new List<string>();
			foreach (KeyValuePair<string, string> pair in Replacements)
			{
				values.Add(pair.Key + Constants.VALUES_SEPARATOR + pair.Value);
			}
            SetMultiValueOption("RequestReplacements", values);
		}


        /// <summary>
        /// Defines a list of custom fields that can be extracted from anywhere in the traffic
        /// </summary>
        /// <returns>A list of name/regex custom fields definitions</returns>
        public Dictionary<string, string> GetResponseReplacements()
        {
            List<string> values = (List<string>)GetOption("ResponseReplacements");
            Dictionary<string, string> result = new Dictionary<string, string>();
            string[] pair;
            if (values != null)
            {
                foreach (string v in values)
                {
                    pair = v.Split(Constants.VALUES_SEPARATOR.ToCharArray());
                    if (pair.Length > 1)
                    {
                        result.Add(pair[0], pair[1]);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Stores custom defined field definitions
        /// </summary>
        /// <param name="replacements">Collection of name/regex pairs</param>
        public void SetResponseReplacements(Dictionary<string, string> replacements)
        {
            List<string> values = new List<string>();
            foreach (KeyValuePair<string, string> pair in replacements)
            {
                values.Add(pair.Key + Constants.VALUES_SEPARATOR + pair.Value);
            }
            SetMultiValueOption("ResponseReplacements", values);
        }


		/// <summary>
		/// Gets a list of request highlighting definitions
		/// </summary>
		public Dictionary<string, string> GetHighlightingDefinitions()
		{

			List<string> values = (List<string>)GetOption("HighlightingDefinitions");
			Dictionary<string, string> result = new Dictionary<string, string>();
			string[] pair;
			if (values != null)
			{
				foreach (string v in values)
				{
					pair = v.Split(Constants.VALUES_SEPARATOR.ToCharArray());
					if (pair.Length > 1)
					{
						result.Add(pair[0], pair[1]);
					}
				}
			}
			else
			{
				result.Add("description=[^=]*login.*\\s", "MidnightBlue");
                result.Add("description=[^=]*session.*\\s", "DimGray");
                result.Add("description=[^=]*test.*\\s", "DarkRed");
                result.Add("description=[^=]*automatic\\s?explore.*\\s", "Crimson");
                result.Add("description=[^=]*cached.*\\s", "DarkGreen");
                result.Add("description=[^=]*to\\s?proxy.*\\s", "DarkPurple");
                result.Add("description=[^=]*vulnerability.*\\s", "DarkMagenta");
                result.Add("scheme=\\bhttp\\s", "DarkSlateGray"); 

			}
			return result;
		}

		/// <summary>
		/// Stores the current highlighting options
		/// </summary>
		/// <param name="hDefs">Collection of description partial match/color</param>
		public void SetHighlightingDefinitions(Dictionary<string, string> hDefs)
		{
			List<string> values = new List<string>();
			foreach (KeyValuePair<string, string> pair in hDefs)
			{
				values.Add(pair.Key + Constants.VALUES_SEPARATOR + pair.Value);
			}
			SetMultiValueOption("HighlightingDefinitions", values);
		}

		/// <summary>
		/// Stores the current version of the HTD file
		/// </summary>
		public Version FileVersion
		{
			get
			{
				string value = (string)GetOption("FileVersion");
				if (value == null) return TrafficViewerFile.FileVersion;
				Version fileVersion = new Version(value);
				return fileVersion;
			}
			set
			{
				SetSingleValueOption("FileVersion", value);
			}
		}

        /// <summary>
        /// Specifies the current profile versions
        /// </summary>
        public Version ProfileVersion
        {
            get
            {
                string value = (string)GetOption("ProfileVersion");
                if (value == null) return new Version("1.0.0");
                Version fileVersion = new Version(value);
                return fileVersion;
            }
            set
            {
                SetSingleValueOption("ProfileVersion", value);
            }
        }


		/// <summary>
		/// Gets ASE default parsing profile
		/// </summary>
		/// <returns></returns>
		public static ParsingOptions GetDefaultProfile()
		{
			ParsingOptions profile = new ParsingOptions();
			profile.BeginThreadRegex = "--- Begin Thread";
			profile.EndThreadRegex = "--- End Thread";
			profile.ThreadIdRegex = @"Begin Thread\s+((?:\d+)?\[\w+\])";
			profile.DescriptionRegex = @"(?:Mode:| ----) ([^>]+)";
			profile.NonHttpTrafficRegex = "^(====>|----)";
			profile.TimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
			profile.FileVersion = TrafficViewerFile.FileVersion;

			List<string> exclusions = new List<string>();
			exclusions.Add("\\.(zip|gz|Z|tar|tgz|sit|cab)\\b");
			exclusions.Add("\\.(css|pdf|ps|doc|ppt|xls|rtf|dot|mpp|mpt|mpd|mdb|csv|pps|ppa|wmf|xlw|xla|dbf|slk|prn|dif)\\b");
			exclusions.Add("\\.(avi|mpg|mpeg|mov|qt|movie|moov|rm|asf|asx|mpe|m1v|mpa|wmv)\\b");
			exclusions.Add("\\.(wav|mp3|ra|au|aiff|mpga|wma|mid|midi|rmi|m3u)\\b");
			exclusions.Add("\\.(gif|jpg|jpeg|bmp|png|tif|tiff|ico|pcx|wmf)\\b");
			exclusions.Add("\\.(js)\\b");
			exclusions.Add("/demo/[^/]+/(data|components|viewer)");
			profile.SetExclusions(exclusions);

            var replacements = profile.GetRequestReplacements();
            replacements.Add(@"(Accept-Encoding:[^\r\n]+\n)", "");
            replacements.Add(@"(If-Modified-Since:[^\r\n]+\n)", "");
            replacements.Add(@"(If-None-Match:[^\r\n]+\n)", "");
            profile.SetRequestReplacements(replacements);

            profile.ProfileVersion = new Version("1.0.3");//update the default profile version

			return profile;
		}

		/// <summary>
		/// Gets AppScan default parsing profile
		/// </summary>
		/// <returns></returns>
		public static ParsingOptions GetLegacyAppScanProfile()
		{
			ParsingOptions profile = new ParsingOptions();
			profile.BeginThreadRegex = "--- Begin Thread";
			profile.EndThreadRegex = "--- End Thread";
			profile.ThreadIdRegex = @"Begin Thread\s+((?:\d+)?\[\w+\])";
			profile.DescriptionRegex = @"(?:Mode:| ----) ([^>]+)\s+--";
			profile.NonHttpTrafficRegex = "^(====>|----)";
			profile.TimeFormat = "ddd, dd MMM yy, HH:mm:ss:fff";
			profile.FileVersion = TrafficViewerFile.FileVersion;

			List<string> exclusions = new List<string>();
			exclusions.Add("\\.(js|zip|gz|Z|tar|tgz|sit|cab)\\b");
			exclusions.Add("\\.(css|pdf|ps|doc|ppt|xls|rtf|dot|mpp|mpt|mpd|mdb|csv|pps|ppa|wmf|xlw|xla|dbf|slk|prn|dif)\\b");
			exclusions.Add("\\.(avi|mpg|mpeg|mov|qt|movie|moov|rm|asf|asx|mpe|m1v|mpa|wmv)\\b");
			exclusions.Add("\\.(wav|mp3|ra|au|aiff|mpga|wma|mid|midi|rmi|m3u)\\b");
			exclusions.Add("\\.(gif|jpg|jpeg|bmp|png|tif|tiff|ico|pcx|wmf)\\b");
			exclusions.Add("\\.(js)\\b");
			exclusions.Add("/demo/[^/]+/(data|components|viewer)");
			profile.SetExclusions(exclusions);

                        

			return profile;
		}

		/// <summary>
		/// Gets a parsing profile that works for non multi-thread logs
		/// </summary>
		/// <returns></returns>
		public static ParsingOptions GetRawProfile()
		{
			ParsingOptions profile = new ParsingOptions();

			List<string> exclusions = new List<string>();
			exclusions.Add("\\.(zip|gz|Z|tar|tgz|sit|cab)\\b");
			exclusions.Add("\\.(css|pdf|ps|doc|ppt|xls|rtf|dot|mpp|mpt|mpd|mdb|csv|pps|ppa|wmf|xlw|xla|dbf|slk|prn|dif)\\b");
			exclusions.Add("\\.(avi|mpg|mpeg|mov|qt|movie|moov|rm|asf|asx|mpe|m1v|mpa|wmv)\\b");
			exclusions.Add("\\.(wav|mp3|ra|au|aiff|mpga|wma|mid|midi|rmi|m3u)\\b");
			exclusions.Add("\\.(gif|jpg|jpeg|bmp|png|tif|tiff|ico|pcx|wmf)\\b");
			exclusions.Add("\\.(js)\\b");
			exclusions.Add("/demo/[^/]+/(data|components|viewer)");
			profile.SetExclusions(exclusions);

            


			return profile;
		}


	}
}
