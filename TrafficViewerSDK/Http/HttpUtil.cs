using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Common code used for Http request/response processing
	/// </summary>
	public class HttpUtil
	{
		private const string CHARSET_ATTRIBUTE = "charset=";

		/// <summary>
		/// Parses the range header string
		/// </summary>
		/// <param name="rangeHeaderValue">Value of range header</param>
		/// <param name="rangeSpecifier">bytes for example</param>
		/// <param name="from">from value</param>
		/// <param name="to">to value</param>
		public static void ParseRange(string rangeHeaderValue, out string rangeSpecifier, out int from, out int to)
		{ 
			rangeSpecifier = "bytes";
			from = 0;
			to = 0;
			
			int indexOfEqual = rangeHeaderValue.IndexOf('=');
			if(indexOfEqual > -1)
			{
				rangeSpecifier = rangeHeaderValue.Substring(0, indexOfEqual).Trim();
				//fremove the range specifier from the header value
				rangeHeaderValue = rangeHeaderValue.Substring(indexOfEqual + 1).Trim();
			}

			int indexOfDash = rangeHeaderValue.IndexOf('-');
			if (indexOfDash > -1)
			{
				string fromValue = rangeHeaderValue.Substring(0, indexOfDash).Trim();
				if (!String.IsNullOrWhiteSpace(fromValue))
				{
					int.TryParse(fromValue, out from);
				}
				rangeHeaderValue = rangeHeaderValue.Substring(indexOfDash + 1).Trim();
			}

			//lastly try parsing the remaining string into the to value
			if (!String.IsNullOrWhiteSpace(rangeHeaderValue))
			{
				int.TryParse(rangeHeaderValue, out to);
			}
		}

		/// <summary>
		/// Gets an encoding according to the content type header charset value
		/// </summary>
		/// <param name="contentTypeHeader"></param>
		/// <returns></returns>
		public static Encoding GetEncoding(string contentTypeHeader)
		{
			Encoding encoding;
            encoding = Constants.DefaultEncoding;

			if (contentTypeHeader != null)
			{
				int valueStart = contentTypeHeader.IndexOf(CHARSET_ATTRIBUTE);
				string charset = null;

				if (valueStart > -1)
				{
					//get the charset attribute Content-Type:text-html;charset=utf8
					charset = contentTypeHeader.Substring(valueStart + CHARSET_ATTRIBUTE.Length).Trim().ToLowerInvariant();
					Encoding enc = null;
					try
					{
						enc = Encoding.GetEncoding(charset);
					}
					catch 
					{
						//if an encoding cannot be obtained from the request default to UTF8
                        SdkSettings.Instance.Logger.Log(TraceLevel.Warning, "Cannot find encoding {0}. Using UTF8", charset);

					}
					if (enc != null)
					{
						encoding = enc;
					}
				}
			}
			return encoding;
		}
	}
}
