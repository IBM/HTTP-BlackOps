using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK.Http
{
	//Contains 3 classes: HttpVariableDefinition & HttpVariableDefinitionSet & HttpVariableDefinitions

	/// <summary>
	/// Defines what should be considered a variable in the request using regular expressions
	/// </summary>
	public class HttpVariableDefinition
	{
		/// <summary>
		/// String indicating the regular type
		/// </summary>
		public const string REGULAR_TYPE = "Regular";

		private string _regex = String.Empty;
		/// <summary>
		/// Uses groups to identify parameter name value pairs
		/// </summary>
		public string Regex
		{
			get { return _regex; }
			set { _regex = value; }
		}

		/// <summary>
		/// Whether this is a normal type of variable Parameter or Cookie or if it is custom
		/// </summary>
		public bool IsRegular
		{
			get
			{
				return String.Compare(REGULAR_TYPE, Name) == 0;
			}
		}

		private string _name = String.Empty;
		/// <summary>
		/// The variable definition name
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private RequestLocation _location;
		/// <summary>
		/// Which request component do we match against
		/// </summary>
		public RequestLocation Location
		{
			get { return _location; }
			set { _location = value; }
		}

		/// <summary>
		/// Overriden to string method for this definition
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("{0}{1}{2}{1}{3}",Name,Constants.VALUES_SEPARATOR,Location,Regex);
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="location"></param>
		/// <param name="regex"></param>
		public HttpVariableDefinition(string name, RequestLocation location, string regex)
		{
			Name = name;
			Location = location;
			Regex = regex;
		}

		/// <summary>
		/// Constructs definition from tab separated values
		/// </summary>
		/// <param name="tabSeparatedValues"></param>
		public HttpVariableDefinition(string tabSeparatedValues)
		{
			string[] values = tabSeparatedValues.Split(new string [1] {Constants.VALUES_SEPARATOR},StringSplitOptions.RemoveEmptyEntries);
			if (values.Length != 3)
			{
				throw new Exception("Improper parameter definition");
			}

			Name = values[0];
			Location = (RequestLocation)Enum.Parse(typeof(RequestLocation), values[1]);
			Regex = values[2];
		}
	}


	
}
