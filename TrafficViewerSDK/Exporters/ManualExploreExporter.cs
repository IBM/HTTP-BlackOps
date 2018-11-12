using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TrafficViewerSDK.Http;
using System.Xml;
using TrafficViewerSDK.Properties;
using System.Text.RegularExpressions;

namespace TrafficViewerSDK.Exporters
{
	/// <summary>
	/// Exports the current traffic information to EXD format
	/// </summary>
	public class ManualExploreExporter:ITrafficExporter
	{
		/// <summary>
		/// If specified this string will be used to skip specific parameters
		/// </summary>
		protected virtual string ParameterExclusion
		{
			get
			{
				return null;
			}
		}
		
		private void WriteParameters(XmlWriter writer, HttpVariables variables)
		{
			if (variables.Count > 0)
			{
				string linkParamType;
				string tvCustomParamName = String.Empty;
				string parameterType = Enum.GetName(typeof(RequestLocation), variables.MatchingDefinition.Location).ToUpper();
				bool isRegular;
				//hack for the unusual handling of custom parameters in AppScan
				if (String.Compare(variables.MatchingDefinition.Name, HttpVariableDefinition.REGULAR_TYPE, true) != 0)
				{
					isRegular = false;
					linkParamType = "invalid";
					parameterType = "QUERY";

				}
				else
				{
					isRegular = true;
					linkParamType = "simplelink";
				}

				int count = 0;

				foreach (KeyValuePair<string, string> pair in variables)
				{
					string name = isRegular ? pair.Key : String.Format("__patternParameter__{0}__{1}__{2}", variables.MatchingDefinition.Name, pair.Key, count);
					if (String.IsNullOrEmpty(ParameterExclusion) || !Utils.IsMatch(name, ParameterExclusion))
					{
						writer.WriteStartElement("parameter");
						writer.WriteAttributeString("name", name);
						writer.WriteAttributeString("value", pair.Value);
						writer.WriteAttributeString("type", parameterType);
						writer.WriteAttributeString("linkParamType", linkParamType);
						writer.WriteAttributeString("separator", "&");
						writer.WriteAttributeString("operator", "=");
						writer.WriteEndElement();
					}
					count++;
				}
			}
		}

		/// <summary>
		/// Writes a request using the specified XMLWriter
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="newHost"></param>
		/// <param name="newPort"></param>
		/// <param name="scheme"></param>
		/// <param name="reqInfo"></param>
        protected void WriteRequest(XmlWriter writer, string newHost, int newPort, string scheme, HttpRequestInfo reqInfo)
        {
            this.WriteRequest(writer, newHost, newPort, scheme, reqInfo, null);
        }

		/// <summary>
		/// Constructs an AppScan request format
		/// </summary>
		/// <param name="writer">The writer to write the request to</param>
		/// <param name="newHost"></param>
		/// <param name="newPort"></param>
		/// <param name="scheme"></param>
		/// <param name="reqInfo">Http request information</param>
		/// <param name="respInfo">Http response information</param>
		/// <param name="additionalAttributes">Additional attributes to be added to the request</param>
		protected void WriteRequest(XmlWriter writer, string newHost, int newPort, string scheme, HttpRequestInfo reqInfo, HttpResponseInfo respInfo, params KeyValuePair<string, string>[] additionalAttributes)
		{
			
                string host;
                int port;
                //check if the host needs to be changed

				if (String.IsNullOrEmpty(newHost))
				{
                    host = reqInfo.Host;		
				}
				else
				{
                    host = newHost;
				}


				if (newPort == 0)
				{
                    port = reqInfo.Port;
				}
				else
				{
                    port = newPort;

				}
                writer.WriteStartElement("request");
             
                writer.WriteAttributeString("method", reqInfo.Method);
                writer.WriteAttributeString("scheme", scheme);
                writer.WriteAttributeString("httpVersion", reqInfo.HttpVersion);
                writer.WriteAttributeString("host", host);
                writer.WriteAttributeString("port", port.ToString());
            
				writer.WriteAttributeString("path", reqInfo.Path);
				writer.WriteAttributeString("contentType", "OTHER");
				writer.WriteAttributeString("boundary", "");
				writer.WriteAttributeString("pathQuerySeparator", "?");
				writer.WriteAttributeString("japEncoding", "0");

				foreach (KeyValuePair<string, string> attribute in additionalAttributes)
				{
					writer.WriteAttributeString(attribute.Key, attribute.Value);
				}

				//write request body if there is one
				string postData = reqInfo.ContentDataString;


				int numberOfPatternParams = 0;

				if (reqInfo.QueryVariables.Count > 0 && !reqInfo.QueryVariables.MatchingDefinition.IsRegular)
				{
					numberOfPatternParams += reqInfo.QueryVariables.Count;
				}

				if (reqInfo.BodyVariables.Count > 0 && !reqInfo.BodyVariables.MatchingDefinition.IsRegular)
				{
					numberOfPatternParams += reqInfo.BodyVariables.Count;
				}

				writer.WriteAttributeString("numberOfPatternParameters", numberOfPatternParams.ToString());

				writer.WriteStartElement("raw");
				writer.WriteAttributeString("encoding", "none");
				writer.WriteString(reqInfo.ToString());
				writer.WriteEndElement();

				if (!String.IsNullOrEmpty(postData))
				{
					writer.WriteStartElement("body");
					writer.WriteAttributeString("encodeValue", "true");
					writer.WriteAttributeString("value", Utils.Base64Encode(postData));
					writer.WriteEndElement();
				}

				WriteParameters(writer, reqInfo.QueryVariables);
				WriteParameters(writer, reqInfo.BodyVariables);

                //writes the headers
                WriteHeaders(writer, host, port, reqInfo.Headers);

				//write cookies
				foreach (KeyValuePair<string, string> pair in reqInfo.Cookies)
				{
					writer.WriteStartElement("cookie");

					writer.WriteAttributeString("name", pair.Key);
					writer.WriteAttributeString("value", pair.Value);

					writer.WriteEndElement();
				}

                /*if(!String.IsNullOrWhiteSpace(reqInfo.ContentDataString) && Utils.IsMatch(reqInfo.ContentDataString,"W([\\w]+)_treeSel"))
                {
                    writer.WriteRaw(Resources.ParametersDefinitions);
                }*/

                //write response
                if (respInfo != null)
                {
                    WriteResponse(writer, respInfo);
                }
				writer.WriteEndElement();
		}

        /// <summary>
        /// Write headers
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
		/// <param name="headers"></param>
        private static void WriteHeaders(XmlWriter writer, string host, int port, HTTPHeaders headers)
        {
            //write headers
            foreach (HTTPHeader header in headers)
            {
                writer.WriteStartElement("header");
                writer.WriteAttributeString("name", header.Name);

                if (String.Compare(header.Name, "host", true) == 0)
                {

                    if (port == 443 || port == 80)
                    {
                        writer.WriteAttributeString("value", host);
                    }
                    else
                    {
                        writer.WriteAttributeString("value", String.Format("{0}:{1}", host, port));
                    }
                }
                else
                {
                    writer.WriteAttributeString("value", header.Value);
                }
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Writes a XML BLOB similar to
        ///  <response status="200" isFrameset="false" isAttack="false" analyzed="false" parsed="true" errorText="" contentType="HTML" headersBuildable="true" bodyBuildable="false" xmlType="XML" japEncoding="2">
        ///  <body encodeValue="true" value="DQoNCjwhRE9DVFlQRSBodG1sIFBVQkxJQyAiLS8vVzNDLy9EVEQgWEhUTUwgMS4wIFRyYW5zaXRpb25hbC8vRU4iICJodHRwOi8vd3d3LnczLm9yZy9UUi94aHRtbDEvRFREL3hodG1sMS10cmFuc2l0aW9uYWwuZHRkIj4NCg0KPGh0bWwgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkveGh0bWwiIHhtbDpsYW5nPSJlbiIgPg0KPGhlYWQgaWQ9Il9jdGwwX19jdGwwX2hlYWQiPjx0aXRsZT4NCglBbHRvcm8gTXV0dWFsDQo8L3RpdGxlPjxtZXRhIGh0dHAtZXF1aXY9IkNvbnRlbnQtVHlwZSIgY29udGVudD0idGV4dC9odG1sOyBjaGFyc2V0PWlzby04ODU5LTEiPjxsaW5rIGhyZWY9InN0eWxlLmNzcyIgcmVsPSJzdHlsZXNoZWV0IiB0eXBlPSJ0ZXh0L2NzcyIgLz48bWV0YSBuYW1lPSJkZXNjcmlwdGlvbiIgY29udGVudD0iQWx0b3JvIE11dHVhbCBvZmZlcnMgYSBicm9hZCByYW5nZSBvZiBjb21tZXJjaWFsLCBwcml2YXRlLCByZXRhaWwgYW5kIG1vcnRnYWdlIGJhbmtpbmcgc2VydmljZXMgdG8gc21hbGwgYW5kIG1pZGRsZS1tYXJrZXQgYnVzaW5lc3NlcyBhbmQgaW5kaXZpZHVhbHMuIj48L2hlYWQ+DQo8Ym9keSBzdHlsZT0ibWFyZ2luLXRvcDo1cHg7Ij4NCg0KPGRpdiBpZD0iaGVhZGVyIiBzdHlsZT0ibWFyZ2luLWJvdHRvbTo1cHg7IHdpZHRoOiA5OSU7Ij4NCiAgPGZvcm0gaWQ9ImZybVNlYXJjaCIgbWV0aG9kPSJnZXQiIGFjdGlvbj0iL3NlYXJjaC5hc3B4Ij4NCgkgIDx0YWJsZSB3aWR0aD0iMTAwJSIgYm9yZGVyPSIwIiBjZWxscGFkZGluZz0iMCIgY2VsbHNwYWNpbmc9IjAiPg0KCQkgIDx0cj4NCgkJICAgICAgPHRkIHJvd3NwYW49IjIiPjxhIGlkPSJfY3RsMF9fY3RsMF9IeXBlckxpbmsxIiBocmVmPSJkZWZhdWx0LmFzcHgiIHN0eWxlPSJoZWlnaHQ6ODBweDt3aWR0aDoxODNweDsiPjxpbWcgc3JjPSJpbWFnZXMvbG9nby5naWYiIGJvcmRlcj0iMCIgLz48L2E+PC90ZD4NCgkJCSAgPHRkIGFsaWduPSJyaWdodCIgdmFsaWduPSJ0b3AiPg0KICAJCQkgIDxhIGlkPSJfY3RsMF9fY3RsMF9Mb2dpbkxpbmsiIHRpdGxlPSJJdCBkb2VzIG5vdCBhcHBlYXIgdGhhdCB5b3UgaGF2ZSBwcm9wZXJseSBhdXRoZW50aWNhdGVkIHlvdXJzZWxmLiAgUGxlYXNlIGNsaWNrIGhlcmUgdG8gc2lnbiBpbi4iIGhyZWY9ImJhbmsvbG9naW4uYXNweCIgc3R5bGU9ImNvbG9yOlJlZDtmb250LXdlaWdodDpib2xkOyI+U2lnbiBJbjwvYT4gfCA8YSBpZD0iX2N0bDBfX2N0bDBfSHlwZXJMaW5rMyIgaHJlZj0iZGVmYXVsdC5hc3B4P2NvbnRlbnQ9aW5zaWRlX2NvbnRhY3QuaHRtIj5Db250YWN0IFVzPC9hPiB8IDxhIGlkPSJfY3RsMF9fY3RsMF9IeXBlckxpbms0IiBocmVmPSJmZWVkYmFjay5hc3B4Ij5GZWVkYmFjazwvYT4gfCA8bGFiZWwgZm9yPSJ0eHRTZWFyY2giPlNlYXJjaDwvbGFiZWw+DQogICAgICAgICAgPGlucHV0IHR5cGU9InRleHQiIG5hbWU9InR4dFNlYXJjaCIgaWQ9InR4dFNlYXJjaCIgYWNjZXNza2V5PSJTIiAvPg0KICAgICAgICAgIDxpbnB1dCB0eXBlPSJzdWJtaXQiIHZhbHVlPSJHbyIgLz4NCgkJCSAgPC90ZD4NCgkJICA8L3RyPg0KCQkgIDx0cj4NCgkJCSAgPHRkIGFsaWduPSJyaWdodCIgc3R5bGU9ImJhY2tncm91bmQtaW1hZ2U6dXJsKC9pbWFnZXMvZ3JhZGllbnQuanBnKTtwYWRkaW5nOjBweDttYXJnaW46MHB4OyI+PGltZyBpZD0iX2N0bDBfX2N0bDBfSW1hZ2UxIiBzcmM9ImltYWdlcy9oZWFkZXJfcGljLmpwZyIgYm9yZGVyPSIwIiBzdHlsZT0iaGVpZ2h0OjYwcHg7d2lkdGg6MzU0cHg7IiAvPjwvdGQ+DQoJCSAgPC90cj4NCgkgIDwvdGFibGU+DQoJPC9mb3JtPg0KPC9kaXY+DQoNCjxkaXYgaWQ9IndyYXBwZXIiIHN0eWxlPSJ3aWR0aDogOTklOyI+DQogICAgDQoNCjx0YWJsZSBjZWxsc3BhY2luZz0iMCIgd2lkdGg9IjEwMCUiPg0KICA8dHI+DQogICAgPHRkIHdpZHRoPSIyNSUiIGNsYXNzPSJidCBiciBiYiI+PGRpdiBpZD0iSGVhZGVyMSI+PGltZyBpZD0iX2N0bDBfX2N0bDBfQ29udGVudF9JbWFnZTEiIHNyYz0iaW1hZ2VzL3BmX2xvY2suZ2lmIiBhbHQ9IlNlY3VyZSBMb2dpbiIgYWxpZ249ImFic2JvdHRvbSIgYm9yZGVyPSIwIiBzdHlsZT0iaGVpZ2h0OjE0cHg7d2lkdGg6MTJweDsiIC8+ICZuYnNwOyA8YSBpZD0iX2N0bDBfX2N0bDBfQ29udGVudF9BY2NvdW50TGluayIgdGl0bGU9IllvdSBkbyBub3QgYXBwZWFyIHRvIGhhdmUgYXV0aGVudGljYXRlZCB5b3Vyc2VsZiB3aXRoIHRoZSBhcHBsaWNhdGlvbi4gIENsaWNrIGhlcmUgdG8gZW50ZXIgeW91ciB2YWxpZCB1c2VybmFtZSBhbmQgcGFzc3dvcmQuIiBjbGFzcz0iZm9jdXMiIGhyZWY9ImJhbmsvbG9naW4uYXNweCI+T05MSU5FIEJBTktJTkcgTE9HSU48L2E+PC9kaXY+PC90ZD4NCiAgICA8dGQgd2lkdGg9IjI1JSIgY2xhc3M9ImNjIGJ0IGJyIGJiIj48ZGl2IGlkPSJIZWFkZXIyIj48YSBpZD0iX2N0bDBfX2N0bDBfQ29udGVudF9MaW5rSGVhZGVyMiIgY2xhc3M9ImZvY3VzIiBocmVmPSJkZWZhdWx0LmFzcHg/Y29udGVudD1wZXJzb25hbC5odG0iPlBFUlNPTkFMPC9hPjwvZGl2PjwvdGQ+DQogICAgPHRkIHdpZHRoPSIyNSUiIGNsYXNzPSJjYyBidCBiciBiYiI+PGRpdiBpZD0iSGVhZGVyMyI+PGEgaWQ9Il9jdGwwX19jdGwwX0NvbnRlbnRfTGlua0hlYWRlcjMiIGNsYXNzPSJmb2N1cyIgaHJlZj0iZGVmYXVsdC5hc3B4P2NvbnRlbnQ9YnVzaW5lc3MuaHRtIj5TTUFMTCBCVVNJTkVTUzwvYT48L2Rpdj48L3RkPg0KICAgIDx0ZCB3aWR0aD0iMjUlIiBjbGFzcz0iY2MgYnQgYmIiPjxkaXYgaWQ9IkhlYWRlcjQiPjxhIGlkPSJfY3RsMF9fY3RsMF9Db250ZW50X0xpbmtIZWFkZXI0IiBjbGFzcz0iZm9jdXMiIGhyZWY9ImRlZmF1bHQuYXNweD9jb250ZW50PWluc2lkZS5odG0iPklOU0lERSBBTFRPUk8gTVVUVUFMPC9hPjwvZGl2PjwvdGQ+DQogIDwvdHI+DQogIDx0cj4NCiAgICA8dGQgdmFsaWduPSJ0b3AiIGNsYXNzPSJjYyBiciBiYiI+DQogICAgICAgIDxiciBzdHlsZT0ibGluZS1oZWlnaHQ6IDEwcHg7Ii8+DQogICAgICAgIDxhIGlkPSJfY3RsMF9fY3RsMF9Db250ZW50X0NhdExpbmsxIiBjbGFzcz0ic3ViaGVhZGVyIiBocmVmPSJkZWZhdWx0LmFzcHg/Y29udGVudD1wZXJzb25hbC5odG0iPlBFUlNPTkFMPC9hPg0KICAgICAgICA8dWwgY2xhc3M9InNpZGViYXIiPg0KICAgICAgICAgICAgPGxpPjxhIGlkPSJfY3RsMF9fY3RsMF9Db250ZW50X01lbnVIeXBlckxpbmsxIiBocmVmPSJkZWZhdWx0LmFzcHg/Y29udGVudD1wZXJzb25hbF9kZXBvc2l0Lmh0bSI+RGVwb3NpdCBQcm9kdWN0PC9hPjwvbGk+DQogICAgICAgICAgICA8bGk+PGEgaWQ9Il9jdGwwX19jdGwwX0NvbnRlbnRfTWVudUh5cGVyTGluazIiIGhyZWY9ImRlZmF1bHQuYXNweD9jb250ZW50PXBlcnNvbmFsX2NoZWNraW5nLmh0bSI+Q2hlY2tpbmc8L2E+PC9saT4NCiAgICAgICAgICAgIDxsaT48YSBpZD0iX2N0bDBfX2N0bDBfQ29udGVudF9NZW51SHlwZXJMaW5rMyIgaHJlZj0iZGVmYXVsdC5hc3B4P2NvbnRlbnQ9cGVyc29uYWxfbG9hbnMuaHRtIj5Mb2FuIFByb2R1Y3RzPC9hPjwvbGk+DQogICAgICAgICAgICA8bGk+PGEgaWQ9Il9jdGwwX19jdGwwX0NvbnRlbnRfTWVudUh5cGVyTGluazQiIGhyZWY9ImRlZmF1bHQuYXNweD9jb250ZW50PXBlcnNvbmFsX2NhcmRzLmh0bSI+Q2FyZHM8L2E+PC9saT4NCiAgICAgICAgICAgIDxsaT48YSBpZD0iX2N0bDBfX2N0bDBfQ29udGVudF9NZW51SHlwZXJMaW5rNSIgaHJlZj0iZGVmYXVsdC5hc3B4P2NvbnRlbnQ9cGVyc29uYWxfaW52ZXN0bWVudHMuaHRtIj5JbnZlc3RtZW50cyAmYW1wOyBJbnN1cmFuY2U8L2E+PC9saT4NCiAgICAgICAgICAgIDxsaT48YSBpZD0iX2N0bDBfX2N0bDBfQ29udGVudF9NZW51SHlwZXJMaW5rNiIgaHJlZj0iZGVmYXVsdC5hc3B4P2NvbnRlbnQ9cGVyc29uYWxfb3RoZXIuaHRtIj5PdGhlciBTZXJ2aWNlczwvYT48L2xpPg0KICAgICAgICA8L3VsPg0KDQogICAgICAgIDxhIGlkPSJfY3RsMF9fY3RsMF9Db250ZW50X0NhdExpbmsyIiBjbGFzcz0ic3ViaGVhZGVyIiBocmVmPSJkZWZhdWx0LmFzcHg/Y29udGVudD1idXNpbmVzcy5odG0iPlNNQUxMIEJVU0lORVNTPC9hPg0KICAgICAgICA8dWwgY2xhc3M9InNpZGViYXIiPg0KICAgICAgICAgICAgPGxpPjxhIGlkPSJfY3RsMF9fY3RsMF9Db250ZW50X01lbnVIeXBlckxpbms3IiBocmVmPSJkZWZhdWx0LmFzcHg/Y29udGVudD1idXNpbmVzc19kZXBvc2l0Lmh0bSI+RGVwb3NpdCBQcm9kdWN0czwvYT48L2xpPg0KICAgICAgICAgICAgPGxpPjxhIGlkPSJfY3RsMF9fY3RsMF9Db250ZW50X01lbnVIeXBlckxpbms4IiBocmVmPSJkZWZhdWx0LmFzcHg/Y29udGVudD1idXNpbmVzc19sZW5kaW5nLmh0bSI+TGVuZGluZyBTZXJ2aWNlczwvYT48L2xpPg0KICAgICAgICAgICAgPGxpPjxhIGlkPSJfY3RsMF9fY3RsMF9Db250ZW50X01lbnVIeXBlckxpbms5IiBocmVmPSJkZWZhdWx0LmFzcHg/Y29udGVudD1idXNpbmVzc19jYXJkcy5odG0iPkNhcmRzPC9hPjwvbGk+DQogICAgICAgICAgICA8bGk+PGEgaWQ9Il9jdGwwX19jdGwwX0NvbnRlbnRfTWVudUh5cGVyTGluazEwIiBocmVmPSJkZWZhdWx0LmFzcHg/Y29udGVudD1idXNpbmVzc19pbnN1cmFuY2UuaHRtIj5JbnN1cmFuY2U8L2E+PC9saT4NCiAgICAgICAgICAgIDxsaT48YSBpZD0iX2N0bDBfX2N0bDBfQ29udGVudF9NZW51SHlwZXJMaW5rMTEiIGhyZWY9ImRlZmF1bHQuYXNweD9jb250ZW50PWJ1c2luZXNzX3JldGlyZW1lbnQuaHRtIj5SZXRpcmVtZW50PC9hPjwvbGk+DQogICAgICAgICAgICA8bGk+PGEgaWQ9Il9jdGwwX19jdGwwX0NvbnRlbnRfTWVudUh5cGVyTGluazEyIiBocmVmPSJkZWZhdWx0LmFzcHg/Y29udGVudD1idXNpbmVzc19vdGhlci5odG0iPk90aGVyIFNlcnZpY2VzPC9hPjwvbGk+DQogICAgICAgIDwvdWw+DQoNCiAgICAgICAgPGEgaWQ9Il9jdGwwX19jdGwwX0NvbnRlbnRfQ2F0TGluazMiIGNsYXNzPSJzdWJoZWFkZXIiIGhyZWY9ImRlZmF1bHQuYXNweD9jb250ZW50PWluc2lkZS5odG0iPklOU0lERSBBTFRPUk8gTVVUVUFMPC9hPg0KICAgICAgICA8dWwgY2xhc3M9InNpZGViYXIiPg0KICAgICAgICAgICAgPGxpPjxhIGlkPSJfY3RsMF9fY3RsMF9Db250ZW50X01lbnVIeXBlckxpbmsxMyIgaHJlZj0iZGVmYXVsdC5hc3B4P2NvbnRlbnQ9aW5zaWRlX2Fib3V0Lmh0bSI+QWJvdXQgVXM8L2E+PC9saT4NCiAgICAgICAgICAgIDxsaT48YSBpZD0iX2N0bDBfX2N0bDBfQ29udGVudF9NZW51SHlwZXJMaW5rMTQiIGhyZWY9ImRlZmF1bHQuYXNweD9jb250ZW50PWluc2lkZV9jb250YWN0Lmh0bSI+Q29udGFjdCBVczwvYT48L2xpPg0KICAgICAgICAgICAgPGxpPjxhIGlkPSJfY3RsMF9fY3RsMF9Db250ZW50X01lbnVIeXBlckxpbmsxNSIgaHJlZj0iY2dpLmV4ZSI+TG9jYXRpb25zPC9hPjwvbGk+DQogICAgICAgICAgICA8bGk+PGEgaWQ9Il9jdGwwX19jdGwwX0NvbnRlbnRfTWVudUh5cGVyTGluazE2IiBocmVmPSJkZWZhdWx0LmFzcHg/Y29udGVudD1pbnNpZGVfaW52ZXN0b3IuaHRtIj5JbnZlc3RvciBSZWxhdGlvbnM8L2E+PC9saT4NCiAgICAgICAgICAgIDxsaT48YSBpZD0iX2N0bDBfX2N0bDBfQ29udGVudF9NZW51SHlwZXJMaW5rMTciIGhyZWY9ImRlZmF1bHQuYXNweD9jb250ZW50PWluc2lkZV9wcmVzcy5odG0iPlByZXNzIFJvb208L2E+PC9saT4NCiAgICAgICAgICAgIDxsaT48YSBpZD0iX2N0bDBfX2N0bDBfQ29udGVudF9NZW51SHlwZXJMaW5rMTgiIGhyZWY9ImRlZmF1bHQuYXNweD9jb250ZW50PWluc2lkZV9jYXJlZXJzLmh0bSI+Q2FyZWVyczwvYT48L2xpPg0KICAgICAgICA8L3VsPg0KICAgIDwvdGQ+DQogICAgPHRkIHZhbGlnbj0idG9wIiBjb2xzcGFuPSIzIiBjbGFzcz0iYmIiPg0KDQogICAgPHNwYW4gaWQ9Il9jdGwwX19jdGwwX0NvbnRlbnRfTWFpbl9sYmxDb250ZW50Ij48YnIgLz48dGFibGUgYm9yZGVyPTAgY2VsbHNwYWNpbmc9MCB3aWR0aD0iMTAwJSI+ICA8dHI+ICAgIDx0ZCB3aWR0aD0iMzMlIiB2YWxpZ249InRvcCI+ICAgICAgICA8Yj48YSBocmVmPSJkZWZhdWx0LmFzcHg/Y29udGVudD1wZXJzb25hbF9zYXZpbmdzLmh0bSI+T25saW5lIEJhbmtpbmcgd2l0aCBGUkVFIE9ubGluZSBCaWxsIFBheSA8L2E+PC9iPjxiciAvPiAgICAgICAgTm8gc3RhbXBzLCBlbnZlbG9wZXMsIG9yIGNoZWNrcyB0byB3cml0ZSBnaXZlIHlvdSBtb3JlIHRpbWUgdG8gc3BlbmQgb24gdGhlIHRoaW5ncyB5b3UgZW5qb3kuIDxiciAvPiAgICAgICAgPGJyIC8+ICAgICAgICA8Y2VudGVyPjxpbWcgc3JjPSJpbWFnZXMvaG9tZTEuanBnIiB3aWR0aD0iMTcwIiBoZWlnaHQ9IjExNCIgLz48L2NlbnRlcj4gICAgICAgIDxiciAvPiAgICAgICAgPGI+PGEgaHJlZj0iZGVmYXVsdC5hc3B4P2NvbnRlbnQ9cGVyc29uYWxfbG9hbnMuaHRtIj5SZWFsIEVzdGF0ZSBGaW5hbmNpbmc8L2E+PC9iPjxiciAvPiAgICAgICAgRmFzdC4gU2ltcGxlLiBQcm9mZXNzaW9uYWwuIFdoZXRoZXIgeW91IGFyZSBwcmVwYXJpbmcgdG8gYnV5LCBidWlsZCwgcHVyY2hhc2UgbGFuZCwgb3IgY29uc3RydWN0IG5ldyBzcGFjZSwgbGV0IEFsdG9ybyBNdXR1YWwncyBwcmVtaWVyIHJlYWwgZXN0YXRlIGxlbmRlcnMgaGVscCB3aXRoIGZpbmFuY2luZy4gQXMgYSByZWdpb25hbCBsZWFkZXIsIHdlIGtub3cgdGhlIG1hcmtldCwgd2UgdW5kZXJzdGFuZCB0aGUgYnVzaW5lc3MsIGFuZCB3ZSBoYXZlIHRoZSB0cmFjayByZWNvcmQgdG8gcHJvdmUgaXQgICAgPC90ZD4gICAgPHRkIHdpZHRoPSIzMyUiIHZhbGlnbj0idG9wIj4gICAgICAgIDxjZW50ZXI+PGltZyBzcmM9ImltYWdlcy9ob21lMi5qcGciIHdpZHRoPSIxNzAiIGhlaWdodD0iMTI4IiAvPjwvY2VudGVyPiAgICAgICAgPGJyIC8+PGJyLz4gICAgICAgIDxiPjxhIGhyZWY9ImRlZmF1bHQuYXNweD9jb250ZW50PWJ1c2luZXNzX2NhcmRzLmh0bSI+QnVzaW5lc3MgQ3JlZGl0IENhcmRzPC9hPjwvYj48YnIgLz4gICAgICAgIFlvdSdyZSBhbHdheXMgbG9va2luZyBmb3Igd2F5cyB0byBpbXByb3ZlIHlvdXIgY29tcGFueSdzIGJvdHRvbSBsaW5lLiBZb3Ugd2FudCB0byBiZSBpbmZvcm1lZCwgaW1wcm92ZSBlZmZpY2llbmN5IGFuZCBjb250cm9sIGV4cGVuc2VzLiBOb3csIHlvdSBjYW4gZG8gaXQgYWxsIC0gd2l0aCBhIGJ1c2luZXNzIGNyZWRpdCBjYXJkIGFjY291bnQgZnJvbSBBbHRvcm8gTXV0dWFsLiAgICAgICAgPGJyIC8+ICAgICAgICA8YnIgLz4gICAgICAgIDxiPjxhIGhyZWY9ImRlZmF1bHQuYXNweD9jb250ZW50PWJ1c2luZXNzX3JldGlyZW1lbnQuaHRtIj5SZXRpcmVtZW50IFNvbHV0aW9uczwvYT48L2I+PGJyIC8+ICAgICAgICBSZXRhaW5pbmcgZ29vZCBlbXBsb3llZXMgaXMgYSB0b3VnaCB0YXNrLiBTZWUgaG93IEFsdG9ybyBNdXR1YWwgY2FuIGFzc2lzdCB5b3UgaW4gYWNjb21wbGlzaGluZyB0aGlzIGZlYXQgdGhyb3VnaCBlZmZlY3RpdmUgUmV0aXJlbWVudCBTb2x1dGlvbnMuICAgIDwvdGQ+ICAgIDx0ZCB3aWR0aD0iMzMlIiB2YWxpZ249InRvcCI+ICAgICAgICA8Yj5Qcml2YWN5IGFuZCBTZWN1cml0eSA8L2I+PGJyIC8+ICAgICAgICBUaGUgMjAwMCBlbXBsb3llZXMgb2YgQWx0b3JvIE11dHVhbCBhcmUgZGVkaWNhdGVkIHRvIHByb3RlY3RpbmcgeW91ciA8YSBocmVmPSJkZWZhdWx0LmFzcHg/Y29udGVudD1wcml2YWN5Lmh0bSI+cHJpdmFjeTwvYT4gYW5kIDxhIGhyZWY9ImRlZmF1bHQuYXNweD9jb250ZW50PXNlY3VyaXR5Lmh0bSI+c2VjdXJpdHk8L2E+LiBXZSBwbGVkZ2UgdG8gcHJvdmlkZSB5b3Ugd2l0aCB0aGUgaW5mb3JtYXRpb24gYW5kIHJlc291cmNlcyB0aGF0IHlvdSBuZWVkIHRvIGhlbHAgc2VjdXJlIHlvdXIgaW5mb3JtYXRpb24gYW5kIGtlZXAgaXQgY29uZmlkZW50aWFsLiAgVGhpcyBpcyBvdXIgcHJvbWlzZS4gICAgICAgIDxiciAvPjxiciAvPiAgICAgICAgPGNlbnRlcj48aW1nIHNyYz0iaW1hZ2VzL2hvbWUzLmpwZyIgd2lkdGg9IjE3MCIgaGVpZ2h0PSIxMTMiIC8+PC9jZW50ZXI+PGJyIC8+PGJyIC8+ICAgICAgICA8Yj48YSBocmVmPSJzdXJ2ZXlfcXVlc3Rpb25zLmFzcHgiPldpbiBhbiA4R0IgaVBvZCBOYW5vPC9hPjwvYj4gICAgICAgIDxiciAvPiAgICAgICAgQ29tcGxldGluZyB0aGlzIHNob3J0IHN1cnZleSB3aWxsIGVudGVyIHlvdSBpbiBhIGRyYXcgZm9yIDEgb2YgNTAgaVBvZCBOYW5vcy4gIFdlIGxvb2sgZm9yd2FyZCB0byBoZWFyaW5nIHlvdXIgaW1wb3J0YW50IGZlZWRiYWNrLiAgICAgICAgPGJyIC8+PGJyIC8+ICAgIDwvdGQ+ICA8L3RyPjwvdGFibGU+PC9zcGFuPg0KDQogICAgPC90ZD4NCiAgPC90cj4NCjwvdGFibGU+DQoNCg0KPC9kaXY+DQoNCjxkaXYgaWQ9ImZvb3RlciIgc3R5bGU9IndpZHRoOiA5OSU7Ij4NCiAgICA8YSBpZD0iX2N0bDBfX2N0bDBfSHlwZXJMaW5rNSIgaHJlZj0iZGVmYXVsdC5hc3B4P2NvbnRlbnQ9cHJpdmFjeS5odG0iPlByaXZhY3kgUG9saWN5PC9hPg0KICAgICZuYnNwOyZuYnNwO3wmbmJzcDsmbmJzcDsNCiAgICA8YSBpZD0iX2N0bDBfX2N0bDBfSHlwZXJMaW5rNiIgaHJlZj0iZGVmYXVsdC5hc3B4P2NvbnRlbnQ9c2VjdXJpdHkuaHRtIj5TZWN1cml0eSBTdGF0ZW1lbnQ8L2E+DQogICAgJm5ic3A7Jm5ic3A7fCZuYnNwOyZuYnNwOw0KICAgICZjb3B5OyAyMDA5IEFsdG9ybyBNdXR1YWwsIEluYy4NCg0KICAgIDxkaXYgY2xhc3M9ImRpc2NsYWltZXIiPg0KICAgICAgICBUaGUgQWx0b3JvIE11dHVhbCB3ZWJzaXRlIGlzIHB1Ymxpc2hlZCBieSBXYXRjaGZpcmUsIEluYy4gZm9yIHRoZSBzb2xlIHB1cnBvc2Ugb2YNCiAgICAgICAgZGVtb25zdHJhdGluZyB0aGUgZWZmZWN0aXZlbmVzcyBvZiBXYXRjaGZpcmUgcHJvZHVjdHMgaW4gZGV0ZWN0aW5nIHdlYiBhcHBsaWNhdGlvbg0KICAgICAgICB2dWxuZXJhYmlsaXRpZXMgYW5kIHdlYnNpdGUgZGVmZWN0cy4gVGhpcyBzaXRlIGlzIG5vdCBhIHJlYWwgYmFua2luZyBzaXRlLiBTaW1pbGFyaXRpZXMsDQogICAgICAgIGlmIGFueSwgdG8gdGhpcmQgcGFydHkgcHJvZHVjdHMgYW5kL29yIHdlYnNpdGVzIGFyZSBwdXJlbHkgY29pbmNpZGVudGFsLiBUaGlzIHNpdGUgaXMNCiAgICAgICAgcHJvdmlkZWQgImFzIGlzIiB3aXRob3V0IHdhcnJhbnR5IG9mIGFueSBraW5kLCBlaXRoZXIgZXhwcmVzcyBvciBpbXBsaWVkLiBXYXRjaGZpcmUgZG9lcw0KICAgICAgICBub3QgYXNzdW1lIGFueSByaXNrIGluIHJlbGF0aW9uIHRvIHlvdXIgdXNlIG9mIHRoaXMgd2Vic2l0ZS4gRm9yIGFkZGl0aW9uYWwgVGVybXMgb2YgVXNlLA0KICAgICAgICBwbGVhc2UgZ28gdG8gPGEgaWQ9Il9jdGwwX19jdGwwX0h5cGVyTGluazciIGhyZWY9Imh0dHA6Ly93d3cud2F0Y2hmaXJlLmNvbS9zdGF0ZW1lbnRzL3Rlcm1zLmFzcHgiPmh0dHA6Ly93d3cud2F0Y2hmaXJlLmNvbS9zdGF0ZW1lbnRzL3Rlcm1zLmFzcHg8L2E+LjxiciAvPjxiciAvPg0KDQogICAgICAgIENvcHlyaWdodCAmY29weTsgMjAwOSwgV2F0Y2hmaXJlIENvcnBvcmF0aW9uLCBBbGwgcmlnaHRzIHJlc2VydmVkLg0KICAgIDwvZGl2Pg0KPC9kaXY+DQoNCjwvYm9keT4NCjwvaHRtbD4=" />
        ///  <header name="Date" value="Thu, 28 May 2009 19:36:29 GMT" />
        ///  <header name="Server" value="Microsoft-IIS/6.0" />
        ///  <header name="X-Powered-By" value="ASP.NET" />
        ///  <header name="X-AspNet-Version" value="2.0.50727" />
        ///  <header name="Set-Cookie" value="ASP.NET_SessionId=n0gzurjs2dtehryqs20ypb55; path=/; HttpOnly" />
        ///  <header name="Set-Cookie" value="amSessionId=143629170095; path=/" />
        ///  <header name="Cache-Control" value="no-cache" />
        ///  <header name="Pragma" value="no-cache" />
        ///  <header name="Expires" value="-1" />
        ///  <header name="Content-Type" value="text/html; charset=utf-8" />
        ///  <header name="Content-Length" value="9605" />
        ///  <cookie name="ASP.NET_SessionId" value="n0gzurjs2dtehryqs20ypb55" />
        ///  <cookie name="amSessionId" value="143629170095" />
        /// </response>
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="respInfo"></param>
        protected virtual void WriteResponse(XmlWriter writer, HttpResponseInfo respInfo)
        {

            writer.WriteStartElement("response");
            writer.WriteAttributeString("status", respInfo.Status.ToString());
            writer.WriteAttributeString("isFrameset", "false");
            writer.WriteAttributeString("isAttack", "false");
            writer.WriteAttributeString("analyzed", "false");
            writer.WriteAttributeString("parsed", "true");
            writer.WriteAttributeString("errorText", "");
            writer.WriteAttributeString("contentType", respInfo.Headers["Content-Type"]);
            writer.WriteAttributeString("headersBuildable", "true");
            writer.WriteAttributeString("bodyBuildable", "false");
            writer.WriteAttributeString("xmlType", "XML");
            writer.WriteAttributeString("japEncoding", "2");
            
            //write body
            writer.WriteStartElement("body");
            //writer.WriteAttributeString("encodeValue", "true");
            writer.WriteAttributeString(" compressedBinaryValue", "true");
			writer.WriteAttributeString(" compressedBinary", "true");

			byte[] respBodyBytes = respInfo.ResponseBody.ToArray();




            writer.WriteAttributeString("value", Utils.CompressToBase64String(respBodyBytes??new byte[0]));
            writer.WriteEndElement();

            //write headers
            WriteHeaders(writer, "", -1, respInfo.Headers);

            //end response
            writer.WriteEndElement();
        }

		#region ITrafficExporter Members

		/// <summary>
		/// Gets the display name of the exporter
		/// </summary>
		public virtual string Caption
		{
			get
			{
				return Resources.EXDCaption;
			}
		}

		/// <summary>
		/// Gets the extension of the exporter
		/// </summary>
		public virtual string Extension
		{
			get
			{
				return "exd";
			}
		}

		/// <summary>
		/// Exports data from the specified source to the specifies strems
		/// </summary>
		/// <param name="source"></param>
		/// <param name="stream"></param>
		/// <param name="overwriteScheme">Wether to overrite existing http scheme</param>
		/// <param name="isSSL">Wether the request is secure or not. This will overrite the http scheme of the request if applicable</param>
		/// <param name="newHost">New host for the request</param>
		/// <param name="newPort">New port for the request</param>
		protected virtual void Export(ITrafficDataAccessor source, Stream stream, bool overwriteScheme, bool isSSL, string newHost, int newPort)
		{
			TVRequestInfo info;
			int i = -1;

			string overridenScheme = isSSL ? "https" : "http";
			//using an xml writer for memory concerns
			XmlWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
			writer.WriteStartDocument();
			writer.WriteComment(String.Format("Automatically created by Traffic Viewer SDK at {0}", DateTime.Now));
			writer.WriteComment(String.Format("Number of requests in file {0}", source.RequestCount));
			writer.WriteStartElement("requests");

			while ((info = source.GetNext(ref i)) != null)
			{
				string scheme = info.IsHttps ? "https" : "http";
				scheme = overwriteScheme ? overridenScheme : scheme;

				//get the request information
				byte[] data = source.LoadRequestData(info.Id);
                if (data != null)
                {
					HttpRequestInfo reqInfo = new HttpRequestInfo(data);
					reqInfo.IsSecure = scheme == "https";
                    WriteRequest(writer, newHost, newPort, scheme, reqInfo);
                }
			}

			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Flush();
		}

		/// <summary>
		/// Executes the export operation
		/// </summary>
		/// <param name="source"></param>
		/// <param name="stream"></param>
		/// <param name="newHost">Replacement host</param>
		/// <param name="newPort">Replacement port</param>
		public void Export(ITrafficDataAccessor source, Stream stream, string newHost, int newPort)
		{
			Export(source, stream, false, false, newHost, newPort);
		}


		/// <summary>
		/// Executes the export operation
		/// </summary>
		/// <param name="source"></param>
		/// <param name="stream"></param>
		public void Export(ITrafficDataAccessor source, Stream stream)
		{
			Export(source, stream, false, false, null, 0);
		}

				

		
		#endregion

	}
}
