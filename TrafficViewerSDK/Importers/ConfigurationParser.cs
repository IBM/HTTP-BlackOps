using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK;
using TrafficViewerSDK.Options;
using System.Xml;
using TrafficViewerSDK.Http;
using TrafficViewerSDK.Properties;

namespace TrafficViewerSDK.Importers
{
	/// <summary>
	/// Parses scant or job configuration files for traffic data
	/// </summary>
	public class ConfigurationParser : ITrafficParser
	{
		private XmlDocument _configFile;
		private ITrafficDataAccessor _tvFile;
		private SortedDictionary<int, XmlNode> _aseRequests;

		private Dictionary<string, string> _importTypes = new Dictionary<string, string>();

		/// <summary>
		/// Gets a parameter string, POST data or QUERY
		/// </summary>
		/// <param name="request"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private string GetParametersString(XmlNode request, string type)
		{
			//find all query parameters
			StringBuilder parametersString = new StringBuilder();
			XmlNodeList queryParameters = request.SelectNodes("parameter[@type='" + type + "']");

			for (int i = 0; i < queryParameters.Count; i++)
			{
				string name = String.Empty;
				if (queryParameters[i].Attributes["name"] != null)
				{
					name = queryParameters[i].Attributes["name"].Value;
				}

				if (name.IndexOf("patternParameter") == -1)
				{
					string value = String.Empty;
					if (queryParameters[i].Attributes["value"] != null)
					{
						value = queryParameters[i].Attributes["value"].Value;
					}

					if (!String.IsNullOrEmpty(value))
					{
						parametersString.Append(name);
						parametersString.Append("=");
						parametersString.Append(value);
						if (i < queryParameters.Count - 1)
						{
							parametersString.Append("&");
						}
					}
				}
			}

			return parametersString.ToString();
		}


		/// <summary>
		/// Adds a request to the current Traffic Viewer File
		/// </summary>
		/// <param name="request"></param>
		/// <param name="description"></param>
		private void AddAppScanRequest(XmlNode request, string description)
		{
			TVRequestInfo reqInfo = new TVRequestInfo();
			reqInfo.Description = description;
            reqInfo.IsHttps = request.Attributes["scheme"] != null && request.Attributes["scheme"].Equals("https");
			reqInfo.ThreadId = Properties.Resources.Settings;

            XmlNode rawRequestNode = request.SelectSingleNode("raw");
            byte[] rawRequestBytes = new byte[0];
            if (rawRequestNode.Attributes["encoding"] != null && rawRequestNode.Attributes["encoding"].Value.Equals("none"))
            {
                string rawRequest = String.Empty;
                rawRequest = rawRequestNode.InnerText;
                rawRequestBytes = Constants.DefaultEncoding.GetBytes(rawRequest);   
            }
            reqInfo.RequestLine = HttpRequestInfo.GetRequestLine(rawRequestBytes);
            reqInfo.Id = _tvFile.AddRequestInfo(reqInfo);
            _tvFile.SaveRequest(reqInfo.Id, rawRequestBytes);

			XmlNode response = request.SelectSingleNode("response");
			
			//put together the response			

			if (response != null)
			{
				
                ByteArrayBuilder builder = new ByteArrayBuilder();
                XmlNode headersNode = response.SelectSingleNode("headers");
                if (headersNode != null && headersNode.Attributes["value"] != null)
                {
                    builder.AddChunkReference(Constants.DefaultEncoding.GetBytes(headersNode.Attributes["value"].Value));
                }

                XmlNode bodyNode = response.SelectSingleNode("body");
				if (bodyNode != null)
				{
                    bool isCompressed = bodyNode.Attributes["compressedBinaryValue"] != null && bodyNode.Attributes["compressedBinaryValue"].Value == "true";


					string body = bodyNode.Attributes["value"].Value;
                    byte[] bodyBytes = new byte[0];
					if (isCompressed)
					{
						bodyBytes = Utils.DecompressBytesFromBase64String(body);
					}
					else
					{
						body = Utils.Base64Decode(body);
                        bodyBytes = Constants.DefaultEncoding.GetBytes(body);
					}
                    builder.AddChunkReference(bodyBytes);
				}
                _tvFile.SaveResponse(reqInfo.Id, builder.ToArray());

			}
		}

		private void AddHeaders(XmlNode node, StringBuilder rawRequest)
		{
			XmlNodeList headers = node.SelectNodes("header");

			foreach (XmlNode header in headers)
			{
				rawRequest.Append(header.Attributes["name"].Value);
				rawRequest.Append(": ");
				rawRequest.AppendLine(header.Attributes["value"].Value);
			}
		}

		#region ITrafficParser Members

		private TrafficParserStatus _status = TrafficParserStatus.Stopped;
		/// <summary>
		/// Gets the status of the current parser      
		/// </summary>
		public TrafficParserStatus ParserStatus
		{
			get { return _status; }
		}

		private ParsingOptions _profile;
		/// <summary>
		/// Gets the current parsing options
		/// </summary>
		public ParsingOptions Options
		{
			get { return _profile; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public ConfigurationParser()
		{
			_importTypes.Add("AppScan Manual Explore Files", "*.exd");
			_importTypes.Add("AppScan Scan Template", "*.scant");
			_importTypes.Add("AppScan Job Configuration", "*.xml");
		}


		/// <summary>
		/// Starts the parsing operation
		/// </summary>
		/// <param name="scantPath"></param>
		/// <param name="currentFile"></param>
		/// <param name="profile"></param>
		public void Parse(string scantPath, ITrafficDataAccessor currentFile, ParsingOptions profile)
		{
			_profile = profile;
			_configFile = new XmlDocument();
            _configFile.XmlResolver = null;
			_configFile.Load(scantPath);
			_tvFile = currentFile;

			HandleAppScanSettings();

			HandleASESettings();

		}

		/// <summary>
		/// Parses data from an object, for example AppScan - not available for this import type
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="currentFile"></param>
		/// <param name="options"></param>
		public void Parse(object sender, ITrafficDataAccessor currentFile, ParsingOptions options)
		{
			throw new NotImplementedException("Not available for this import type");
		}
		private void HandleASESettings()
		{
			XmlNodeList exploreUrls = _configFile.SelectNodes("//option[@enum='epcsCOTExploreUrls']");
			_aseRequests = new SortedDictionary<int, XmlNode>();
			foreach (XmlNode url in exploreUrls)
			{
				XmlNode requestOrdinal = url.SelectSingleNode("option[@enum='elCOTExploreOrdinal']");
				int ordinal = int.Parse(requestOrdinal.Attributes["value"].Value);
				_aseRequests.Add(ordinal, url);
			}

			foreach (int ordinal in _aseRequests.Keys)
			{
				AddASERequest(_aseRequests[ordinal]);
			}

		}

		private void AddASERequest(XmlNode url)
		{
			XmlNode requestNode = url.SelectSingleNode("option[@enum='esCOTExploreRequest']");
			XmlNode responseNode = url.SelectSingleNode("option[@enum='esCOTExploreResponse']");
			XmlNode requestTypeNode = url.SelectSingleNode("option[@enum='elCOTExploreType']");
			XmlNode responseHeaderNode = url.SelectSingleNode("option[@enum='esCOTExploreResponseHeader']");

			ExploreType requestType = (ExploreType)int.Parse(requestTypeNode.Attributes["value"].Value);
			string description = String.Empty;

			switch (requestType)
			{
				case ExploreType.eETLogin: description = Resources.RecordedLoginPage; break;
				case ExploreType.eETRegularInSession: description = Resources.InSessionPage; break;
				case ExploreType.eETRegular: description = Resources.ManualExplore; break;
			}

			string request = requestNode.Attributes["value"].Value;
			request = Utils.Base64Decode(request);
			HttpRequestInfo reqInfo = new HttpRequestInfo(request);
			TVRequestInfo tvInfo = new TVRequestInfo();
			tvInfo.RequestLine = reqInfo.RequestLine;
			tvInfo.ThreadId = Resources.Settings;
			tvInfo.Description = description;

			RequestResponseBytes data = new RequestResponseBytes();
			data.AddToRequest(request);

			if (responseHeaderNode != null && responseHeaderNode.Attributes["value"]!=null)
			{
				data.AddToResponse(responseHeaderNode.Attributes["value"].Value + Environment.NewLine);
			}

			byte[] response = Convert.FromBase64String(responseNode.Attributes["value"].Value);

			response = Utils.DecompressData(response);

            response = Encoding.Convert(Encoding.Unicode, Constants.DefaultEncoding, response);

			data.AddToResponse(response);

			//attempt to parse the response
			try
			{
				HttpResponseInfo respInfo = new HttpResponseInfo();
				respInfo.ProcessResponse(data.RawResponse);
				tvInfo.ResponseStatus = respInfo.Status.ToString();
			}
			catch 
			{
				tvInfo.ResponseStatus = "???";
			}

			_tvFile.AddRequestInfo(tvInfo);
			_tvFile.SaveRequest(tvInfo.Id, data);
			_tvFile.SaveResponse(tvInfo.Id, data);
		}

		private void HandleAppScanSettings()
		{
			//select all the request nodes in the login sequence
			XmlNode recordedSessionRequestsNode = _configFile.SelectSingleNode("//RecordedSessionRequests");
			if (recordedSessionRequestsNode != null)
			{
				//select the login requests
				XmlNodeList requests = recordedSessionRequestsNode.SelectNodes("request[@SessionRequestType='Login']");
				if (requests != null)
				{
					//save the recorded login data in the session variable
					//set its ordinal based on its place in the list
					foreach (XmlNode request in requests)
					{
						AddAppScanRequest(request, Properties.Resources.RecordedLoginPage);
					}
				}

				XmlNode inSessionRequest = recordedSessionRequestsNode.SelectSingleNode("request[@IsSessionVerifier='True']");
				if (inSessionRequest != null)
				{
					AddAppScanRequest(inSessionRequest, Properties.Resources.InSessionPage);
				}
			}

			//parse multistep operations
			XmlNode sequencesNode = _configFile.SelectSingleNode("//StateInducer/Sequences");
			if (sequencesNode != null)
			{
				XmlNodeList sequences = sequencesNode.SelectNodes("Sequence");
				foreach (XmlNode sequence in sequences)
				{
					XmlNode requestsNode = sequence.SelectSingleNode("requests");
					XmlNodeList requests = requestsNode.SelectNodes("request");
					foreach (XmlNode request in requests)
					{
						AddAppScanRequest(request, Properties.Resources.MultiStep);
					}
				}
			}

			//parse manual explore data (EXD)
			XmlNode exdRequestsNode = _configFile.SelectSingleNode("requests");
			if (exdRequestsNode != null)
			{
				XmlNodeList exdRequests = exdRequestsNode.SelectNodes("request");
				foreach (XmlNode request in exdRequests)
				{
					AddAppScanRequest(request, Properties.Resources.ManualExplore);
				}
			}
		}



		/// <summary>
		/// Doesn't do anything, the parsing stops when all the requests in the file were extracted
		/// </summary>
		public void Stop()
		{

		}

		/// <summary>
		/// Doesn't do anything, the parsing stops when all the requests in the file were extracted
		/// </summary>
		public void Resume()
		{
		}

		/// <summary>
		/// Tail doesn't do anything
		/// </summary>
		/// <param name="tailChunk"></param>
		public void Tail(int tailChunk)
		{

		}

		/// <summary>
		/// Clearing a scant file is not supported, this will only reset the position of the current stream
		/// </summary>
		public void ClearSource()
		{
			throw new NotImplementedException("Clearing scan files is not necessary.");
		}

		/// <summary>
		/// File types that can be imported
		/// </summary>
		public Dictionary<string, string> ImportTypes
		{
			get { return _importTypes; }
		}

		/// <summary>
		/// Name to be displayed to the user
		/// </summary>
		public string Name
		{
			get { return Resources.ConfigurationParserName; }
		}

		/// <summary>
		/// What types of import modes it supports
		/// </summary>
		public ImportMode ImportSupport
		{
			get { return ImportMode.Files; }
		}

		#endregion
	}
}
