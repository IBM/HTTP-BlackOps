using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Represents a headers collection
	/// </summary>
	public class HTTPHeaders : IEnumerable<HTTPHeader>
	{
		private List<HTTPHeader> _headers =
			new List<HTTPHeader>();

		/// <summary>
		/// Gets/sets the first value of the first header that matches the name.
		/// If the header does not exist the setter automatically adds the header
		/// </summary>
		public string this[string name]
		{
			get
			{
				List<HTTPHeader> matches = GetHeaders(name);
				string result = null;
				if (matches.Count > 0)
				{
					result = matches[0].Values[0];
				}
				return result;
			}
			set
			{
				List<HTTPHeader> matches = GetHeaders(name);
				if (matches.Count > 0)
				{
					matches[0].Values[0] = value;
				}
				else
				{
					Add(name, value);
				}
			}
		}

		/// <summary>
		/// Returns the list of headers with that name
		/// </summary>
		/// <param name="name">The name of the header</param>
		/// <returns></returns>
		public List<HTTPHeader> GetHeaders(string name)
		{
			List<HTTPHeader> result = new List<HTTPHeader>();
			foreach (HTTPHeader header in _headers)
			{
				if (String.Compare(header.Name, name, true) == 0)
				{
					result.Add(header);
				}
			}
			return result;
		}


		/// <summary>
		/// Adds a new header to the list
		/// </summary>
		/// <param name="name"></param>
		/// <param name="values"></param>
		public void Add(string name, params string[] values)
		{
			_headers.Add(new HTTPHeader(name, values));
		}

        /// <summary>
        /// Adds a new header to the list
        /// </summary>
        /// <param name="header"></param>
        public void Add(HTTPHeader header)
        {
            _headers.Add(header);
        }

		/// <summary>
		/// Removes all headers matching this name
		/// </summary>
		/// <param name="name"></param>
		public void Remove(string name)
		{
			for (int i = _headers.Count - 1; i > -1; i--)
			{
				if (_headers[i].Name == name)
				{
					_headers.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Overriden to string method
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string s = String.Empty;
			foreach (HTTPHeader header in _headers)
			{
				s += header.ToString() + Environment.NewLine;
			}
			return s;
		}

		#region IEnumerable<HTTPHeader> Members

		/// <summary>
		/// Gets an enumerator
		/// </summary>
		/// <returns></returns>
		public IEnumerator<HTTPHeader> GetEnumerator()
		{
			return _headers.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _headers.GetEnumerator();
		}

		#endregion
	}

	/// <summary>
	/// Represents a HTTP header
	/// </summary>
	public class HTTPHeader
	{
		private string _name;
		/// <summary>
		/// the header name
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private List<string> _values;
		/// <summary>
		/// values for that header
		/// </summary>
		public List<string> Values
		{
			get { return _values; }
			set { _values = value; }
		}

		/// <summary>
		/// The value of the header
		/// </summary>
		public string Value
		{
			get
			{
				StringBuilder value = new StringBuilder();
				int n = _values.Count;
				for (int i = 0; i < n; i++)
				{
					value.Append(_values[i]);
					if (i < -1)
					{
						value.Append(";");
					}
				}
				return value.ToString();
			}
		}

		/// <summary>
		/// Overriden ToString method
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("{0}: {1}",_name, Value);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="values"></param>
		public HTTPHeader(string name, params string[] values)
		{
			_name = name;
			_values = new List<string>();
			foreach (string v in values)
			{
				_values.Add(v.Trim());
			}
		}

	}

}
