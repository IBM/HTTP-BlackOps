using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK;
using TrafficViewerSDK.Options;
using System.Text.RegularExpressions;
using TrafficViewerSDK.Http;
using System.IO;
using TrafficViewerSDK.Properties;
using System.Diagnostics;

namespace TrafficViewerSDK.Importers
{
	/// <summary>
	/// Parses URIs from text files
	/// </summary>
	public class UriParser : ITrafficParser
	{
		private ParsingOptions _options = new ParsingOptions();
		private TrafficParserStatus _status = TrafficParserStatus.Stopped;
		private const string URI_REGEX = "https?://[^\\s\\r\\n\"']+";
		private HTTPHeader ADDITIONAL_HEADER = new HTTPHeader("Cookie", "JSESSIONID=38C9DE9EE94347C85D537954685EACE4; SEC=0b78ba9a-15dd-44e2-a229-b4f25e0a18e4;");
		private Dictionary<string, string> _importTypes = new Dictionary<string, string>();

		/// <summary>
		/// Constructor
		/// </summary>
		public UriParser()
		{
			_importTypes.Add("All Files", "*.*");
		}

		#region ITrafficParser Members
		/// <summary>
		/// Returns parser status
		/// </summary>
		public TrafficParserStatus ParserStatus
		{
			get { return _status; }
		}
		/// <summary>
		/// Gets the current options
		/// </summary>
		public ParsingOptions Options
		{
			get { return _options; }
		}
		/// <summary>
		/// Executes the parsing operation
		/// </summary>
		/// <param name="pathOfFileToImport"></param>
		/// <param name="currentFile"></param>
		/// <param name="options"></param>
		public void Parse(string pathOfFileToImport, ITrafficDataAccessor currentFile, ParsingOptions options)
		{
			_options = options;
			_status = TrafficParserStatus.Running;

			string text = File.ReadAllText(pathOfFileToImport);

			MatchCollection matches = Regex.Matches(text, URI_REGEX);

			Dictionary<string, List<Uri>> uniqueRequests = new Dictionary<string, List<Uri>>();

			foreach (Match m in matches)
			{
				try
				{
					Uri uri = new Uri(m.Value);
					string key = String.Format("{0}://{1}:{2}/{3}", uri.Scheme, uri.Host, uri.Port, uri.AbsolutePath);
					if (!uniqueRequests.ContainsKey(key))
					{
					
						uniqueRequests.Add(key,new List<Uri>());
					
					}
					uniqueRequests[key].Add(uri);

				}
				catch (Exception ex)
				{
					SdkSettings.Instance.Logger.Log(TraceLevel.Error, "URI Parser - Invalid uri: {0}", ex.Message);
				}
			}


			foreach (string key in uniqueRequests.Keys)
			{
				try
				{
					List<Uri> uriList = uniqueRequests[key];
					//construct querystring
					StringBuilder queryStringBuilder = new StringBuilder();
					foreach (Uri uri in uriList)
					{
						string query = uri.Query.Trim('?');
						if (!String.IsNullOrWhiteSpace(query))
						{
							queryStringBuilder.Append(query);
							queryStringBuilder.Append('&');
						}
					}
					//now get the full query
					string fullQuery = queryStringBuilder.ToString().Trim('&');
					Uri firstUri = uriList[0];

					AddRequest(currentFile, firstUri, fullQuery, Resources.UriParserGetRequest);
					AddRequest(currentFile, firstUri, fullQuery, Resources.UriParserPostRequest);
				}
				catch (Exception ex)
				{
					SdkSettings.Instance.Logger.Log(TraceLevel.Error, "URI Parser - Failed to add request: {0}", ex.Message);
				}
			}

			_status = TrafficParserStatus.Stopped;
		}

		private void AddRequest(ITrafficDataAccessor currentFile, Uri uri, string fullQuery, string format)
		{
			string request = String.Format(format, uri.AbsolutePath, fullQuery, uri.Host, uri.Port);
			HttpRequestInfo requestInfo = new HttpRequestInfo(request);
			requestInfo.Headers.Add(ADDITIONAL_HEADER);
			TVRequestInfo tvReqInfo = new TVRequestInfo();

			tvReqInfo.Description = Resources.UriParserDescription;
			tvReqInfo.RequestLine = HttpRequestInfo.GetRequestLine(request);
			tvReqInfo.ThreadId = "N/A";
			tvReqInfo.RequestTime = DateTime.Now;
			tvReqInfo.IsHttps = String.Compare(uri.Scheme, "https", true) == 0;
			currentFile.AddRequestInfo(tvReqInfo);
			currentFile.SaveRequest(tvReqInfo.Id, requestInfo.ToArray(false));
		}

		/// <summary>
		/// Executes a parsing operation from an object - not implemented
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="currentFile"></param>
		/// <param name="options"></param>
		public void Parse(object sender, ITrafficDataAccessor currentFile, ParsingOptions options)
		{
			throw new NotImplementedException("Not applicable for this import type");
		}


		/// <summary>
		/// Stop parsing
		/// </summary>
		public void Stop()
		{
			;
		}

		/// <summary>
		/// Resume parsing
		/// </summary>
		public void Resume()
		{
			;
		}

		/// <summary>
		/// Not implemented
		/// </summary>
		/// <param name="tailChunk"></param>
		public void Tail(int tailChunk)
		{
			;
		}

		/// <summary>
		/// Not applicable
		/// </summary>
		public void ClearSource()
		{
			throw new NotImplementedException("Not applicable for this import type");
		}


		/// <summary>
		/// Type of files supported
		/// </summary>
		public Dictionary<string, string> ImportTypes
		{
			get
			{
				return _importTypes;
			}
		}


		/// <summary>
		/// The name of the parser
		/// </summary>
		public string Name
		{
			get { return Resources.UriParserName; }
		}


		/// <summary>
		/// The type of imports that it supports
		/// </summary>
		public ImportMode ImportSupport
		{
			get { return ImportMode.Tail; }
		}

		#endregion
	}
}
