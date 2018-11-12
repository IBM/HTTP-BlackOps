using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TrafficViewerSDK.Search;
using TrafficViewerSDK.Options;
using System.Diagnostics;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Stores key value pairs for the variables in the request
	/// </summary>
	public class HttpVariables : Dictionary<string, string>
	{
		private const string GROUP_REGEX = @"\([^?].*?\)";
		private List<HttpVariableDefinition> _variablesDefinitions =
			SdkSettings.Instance.VariableDefinitions;

        private List<string> _sessionIdNames = SdkSettings.Instance.SessionIdPatterns;

		private RequestLocation _location;

		private string _originalString = String.Empty;

		private HttpVariableDefinition _matchingDefinition;
		/// <summary>
		/// The regular expression that matched the current variable definition collection
		/// </summary>
		public HttpVariableDefinition MatchingDefinition
		{
			get { return _matchingDefinition; }
		}

		/// <summary>
		/// Gets a unique code for this collection including the values
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		/// <summary>
		/// Gets a unique code for this collection according to the includeValue parameter
		/// </summary>
		/// <param name="includeValues">True to include the variables values</param>
		/// <returns></returns>
		public int GetHashCode(bool includeValues)
		{
			int result = 0;
			foreach (string key in this.Keys)
			{
				result = result ^ key.GetHashCode();

				if (includeValues)
				{
					result = result ^ this[key].GetHashCode();
				}
			}

			return result;
		}

		/// <summary>
		/// Compares the current collection to the given parameter
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return this.Equals(obj, true);
		}

		/// <summary>
		/// Compares the current collection to the given parameter
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="includeValues">True to include the variables values</param>
		/// <returns></returns>
		public bool Equals(object obj, bool includeValues)
		{
			try
			{
				if (this.GetHashCode(includeValues) == (obj as HttpVariables).GetHashCode(includeValues))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Overriden ToString method
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ToString(true, String.Empty);
		}

		/// <summary>
		/// Overriden ToStringMethod
		/// </summary>
		/// <param name="nameValueSeparator">String to be used to when concatenating names and values</param>
		/// <returns></returns>
		public string ToString(string nameValueSeparator)
		{
			return ToString(true, nameValueSeparator);
		}

		/// <summary>
		/// Constructs variable string when the operator and separator are known
		/// </summary>
		/// <param name="nameValueSeparator"></param>
		/// <param name="parameterSeparator"></param>
		/// <returns></returns>
		public string ToString(string nameValueSeparator, string parameterSeparator)
		{
			StringBuilder sb = new StringBuilder();
			int count = 0;
			foreach (string key in this.Keys)
			{
				string name = key;
				string value = this[key];

				sb.Append(key);
				if (value != null)
				{
					sb.Append(nameValueSeparator);
					sb.Append(value);
				}
				count++;

				if (count < this.Keys.Count)
				{
					sb.Append(parameterSeparator);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Gets a string of the http variables
		/// </summary>
		/// <param name="includeValues"></param>
		/// <param name="nameValueSeparator"></param>
		/// <returns></returns>
		public string ToString(bool includeValues, string nameValueSeparator)
		{
			string componentString = _originalString;

			if (_matchingDefinition == null)
			{
                return _originalString;
			}


			foreach (string key in this.Keys)
			{
				string name = key;
				string value = includeValues ? this[key] : String.Empty;
				//create a regex that will match this variable				
				//replace the first group in the definition
				string nameSpecificDefinition = String.Empty;

				Match m = Regex.Match(_matchingDefinition.Regex, GROUP_REGEX);

				if (m.Success)
				{

					nameSpecificDefinition =
                        (m.Index == 0 ? "\\b" : _matchingDefinition.Regex.Substring(0, m.Index)) +
						Regex.Escape(name) +
						_matchingDefinition.Regex.Substring(m.Index + m.Length);



					//check if the definition exists in the original string
					if (Regex.IsMatch(_originalString, nameSpecificDefinition, RegexOptions.IgnoreCase))
					{
						componentString = Utils.ReplaceGroups(componentString, nameSpecificDefinition, value);
					}
					else //this is a new parameter
					{
                        if (_matchingDefinition.IsRegular)
                        {
                            if (String.IsNullOrWhiteSpace(nameValueSeparator))
                            {
                                if (_matchingDefinition.Location == RequestLocation.Cookies)
                                {
                                    nameValueSeparator = ";";
                                }
                                else
                                {
                                    nameValueSeparator = "&";
                                }
                            }

                            if (String.IsNullOrWhiteSpace(componentString))
                            {
                                componentString += String.Format("{0}={1}", name, value);
                            }
                            else
                            {
                                componentString = String.Format("{0}{1}{2}={3}", componentString, nameValueSeparator, name, value);
                            }
                        }
                            /*
                        else
                        {
                            string nameValueString = Regex.Replace(nameSpecificDefinition, GROUP_REGEX, value);
                            componentString = String.Format("{0}{1}{2}", componentString, nameValueSeparator, nameValueString);

                            if (nameValueSeparator == String.Empty)
                            {
                                SdkSettings.Instance.Logger.Log(TraceLevel.Warning, "HttpVariables: Adding new variables without knowing the name value separator might result in malformed requests: {0}", _originalString);
                            }
                        }*/

					}


				}
			}

			return componentString;
		}

        /// <summary>
        /// Sets a variable in this collection
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Set(string name, string value)
        {
            if (ContainsKey(name))
            {
                this[name] = value;
            }
            else
            { 
                this.Add(name,value);
            }
        }

		/// <summary>
		/// Gets a simple text search criteria for these variables
		/// </summary>
		/// <param name="includeValues">Whether to include values in the criteria</param>
		/// <returns>A non regex criteria</returns>
		public SearchCriteria GetSearchCriteria(bool includeValues)
		{
			SearchContext context;
			if (_location == RequestLocation.Body)
			{
				context = SearchContext.RequestBody;
			}
			else if (_location == RequestLocation.Path || _location == RequestLocation.Query)
			{
				context = SearchContext.RequestLine;
			}
			else
			{
				context = SearchContext.Request;
			}

			SearchCriteria result = new SearchCriteria(context, false);

			foreach (string key in this.Keys)
			{
				string name = key;
				//normalize name
				result.Texts.Add(name);

				string value = this[key];

				if (includeValues && !String.IsNullOrEmpty(value))
				{
					int indexOfDynEl = value.IndexOf(Constants.DYNAMIC_ELEM_STRING);
					if (indexOfDynEl > -1)
					{
						value = value.Substring(0, indexOfDynEl);
					}
					result.Texts.Add(value);
				}
			}
			return result;
		}

        /// <summary>
        /// Exports the current collection to a variable info collection
        /// </summary>
        /// <returns></returns>
        public List<HttpVariableInfo> GetVariableInfoCollection()
        {
            List<HttpVariableInfo> variableInfoCollection = new List<HttpVariableInfo>();

            foreach (string key in this.Keys)
            {
                HttpVariableInfo varInfo = new HttpVariableInfo();
                varInfo.Name = key;
                varInfo.Value = this[key];
                varInfo.Type = _matchingDefinition.Name;
                varInfo.Location = _matchingDefinition.Location;
                varInfo.IsTracked = Utils.IsMatchInList(varInfo.Name, _sessionIdNames);

                variableInfoCollection.Add(varInfo);
               
            }

            return variableInfoCollection;
            
        }

		/// <summary>
		/// Constructor for a HttpVariables collection
		/// </summary>
		public HttpVariables()
			: this(String.Empty, RequestLocation.Path)
		{ }

		/// <summary>
		/// Constructor for a HttpVariables collection
		/// </summary>
		/// <param name="source"></param>
		/// <param name="location"></param>
		public HttpVariables(string source, RequestLocation location) : base(StringComparer.OrdinalIgnoreCase)
		{
			
			MatchCollection matches;

			_location = location;

			_originalString = source;


			int n = _variablesDefinitions.Count - 1, i;
			for (i = n; i > -1; i--)
			{
				if (_variablesDefinitions[i].Location == location && Utils.IsMatch(_originalString, _variablesDefinitions[i].Regex))
				{
					_matchingDefinition = _variablesDefinitions[i];

					break;
				}
			}

			if (_matchingDefinition != null)
			{
				matches = Regex.Matches(_originalString, _matchingDefinition.Regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);


				foreach (Match m in matches)
				{
					if (m.Groups.Count > 2)
					{
						string name = m.Groups[1].Value.Trim();
						if (!String.IsNullOrWhiteSpace(name) && !this.ContainsKey(name))
						{
							this.Add(name, m.Groups[2].Value.Trim());
						}
					}
				}

			}

		}




    }
}
