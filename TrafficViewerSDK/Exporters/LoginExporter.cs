using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Properties;
using System.Xml;
using TrafficViewerSDK.Http;

namespace TrafficViewerSDK.Exporters
{
	class LoginExporter : ManualExploreExporter
	{

        private Dictionary<int, HttpVariableInfo> _variableInfoCollection = new Dictionary<int,HttpVariableInfo>();

        private void AddToVariableInfoCollection(IEnumerable<HttpVariableInfo> variableInfoList)
        {
            foreach (HttpVariableInfo varInfo in variableInfoList)
            {
                if (!_variableInfoCollection.ContainsKey(varInfo.GetHashCode()))
                {
                    _variableInfoCollection.Add(varInfo.GetHashCode(), varInfo);
                }
            }
        }
        

		#region ITrafficExporter Members

		public override string Caption
		{
			get { return Resources.LoginExporter; }
		}

		public override string Extension
		{
			get { return "login"; }
		}

		protected override void Export(ITrafficDataAccessor source, System.IO.Stream stream, bool overwriteScheme ,bool isSSL, string newHost, int newPort)
		{
			TVRequestInfo info, inSessionRequestInfo = null;

			int i=-1,count = 0;

			string overridenScheme = isSSL ? "https" : "http";
			//using an xml writer for memory concerns
			XmlWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
			writer.WriteStartDocument();
			writer.WriteStartElement("SessionManagement");
			writer.WriteAttributeString("Version","1.2");
			writer.WriteElementString("SessionManagementMode", "Manual");
			writer.WriteElementString("AllowConcurrentLogins", "True");
			writer.WriteElementString("EnableJSXInLoginReplay", "False");

			writer.WriteStartElement("RecordedSessionRequests");

			bool loginFound = false;

			
			while ((info = source.GetNext(ref i)) != null)
			{

				string scheme = info.IsHttps ? "https" : "http";
				scheme = overwriteScheme ? overridenScheme : scheme;
                //get the request information
                byte[] reqData = source.LoadRequestData(info.Id);
                byte[] respData = source.LoadResponseData(info.Id);
			
                if (reqData != null)
                {
                    HttpRequestInfo reqInfo = new HttpRequestInfo(reqData);
                    reqInfo.IsSecure = scheme.Equals("https");
                    HttpResponseInfo respInfo = new HttpResponseInfo(respData);
                    //add to the list of variables
                    AddToVariableInfoCollection(reqInfo.Cookies.GetVariableInfoCollection());
                    AddToVariableInfoCollection(reqInfo.QueryVariables.GetVariableInfoCollection());
                    AddToVariableInfoCollection(reqInfo.BodyVariables.GetVariableInfoCollection());
                    AddToVariableInfoCollection(reqInfo.PathVariables.GetVariableInfoCollection());

                    if (info.Description.IndexOf(Resources.Login, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        WriteRequest(writer, newHost, newPort, scheme, reqInfo, respInfo, new KeyValuePair<string, string>("SessionRequestType", "Login"));
                        loginFound = true;
                        count++;
                    }
                    else if (info.Description.IndexOf(Resources.Session, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        WriteRequest(writer, newHost, newPort, scheme, reqInfo, respInfo, new KeyValuePair<string, string>("IsSessionVerifier", "True"));
                        inSessionRequestInfo = info;
                        break;
                    }
                    else if (loginFound)
                    {
                        break;
                    }
                }

			}


			writer.WriteEndElement();

			if (inSessionRequestInfo != null)
			{
				writer.WriteStartElement("SessionVerifier");
				writer.WriteElementString("Enable", "True");
				writer.WriteElementString("Pattern", @"(?i)((log|sign)\s?(out|off)|exit|quit)");
				writer.WriteElementString("PatternType", "RegularExpression");
				byte[] reqData = source.LoadRequestData(inSessionRequestInfo.Id);
                byte[] respData = source.LoadResponseData(inSessionRequestInfo.Id);


				string scheme = inSessionRequestInfo.IsHttps ? "https" : "http";
				scheme = overwriteScheme ? overridenScheme : scheme;
                
				WriteRequest(writer, newHost, newPort, scheme, new HttpRequestInfo(reqData), new HttpResponseInfo(respData),new KeyValuePair<string, string>("SessionRequestType", "Regular"));			
				writer.WriteEndElement();

				writer.WriteElementString("InSessionRequestIndex", count.ToString());
			}

			//WriteVariableDefinitions(writer);

			writer.WriteEndElement();
			writer.WriteEndDocument();
			
			writer.Close();

			if (!loginFound)
			{ 
				//warn the user
				throw new Exception(Resources.NoLoginRequestsFound);
			}
		}

		protected void WriteVariableDefinitions(XmlWriter writer)
		{
			//now write variable definitions
			writer.WriteStartElement("VariablesDefinitions");
			foreach (HttpVariableInfo varInfo in _variableInfoCollection.Values)
			{
				string variableType = "Parameter";
				if (String.Compare(varInfo.Type, "Regular", true) != 0)
				{
					variableType = "Custom";
				}
				else if (varInfo.Location == RequestLocation.Cookies)
				{
					variableType = "Cookie";
				}


				string requestIgnoreStatus = varInfo.IsTracked ? "Value" : "None";
				string entityIgnoreStatus = "Value";


				writer.WriteStartElement("VariableDefinition");
				writer.WriteAttributeString("IsRegularExpression", "False");
				writer.WriteAttributeString("Name", varInfo.Name);
				writer.WriteElementString("VariableType", variableType);
				writer.WriteElementString("Hosts", String.Empty);
				writer.WriteElementString("Comments", String.Empty);
				writer.WriteElementString("RequestIgnoreStatus", requestIgnoreStatus);
				writer.WriteElementString("EntityIgnoreStatus", entityIgnoreStatus);
				writer.WriteElementString("ExcludeFromTest", "False");
				writer.WriteElementString("SessionIDEnabled", varInfo.IsTracked.ToString());
				if (varInfo.IsTracked)
				{
					writer.WriteStartElement("SessionID");
					writer.WriteAttributeString("TrackingMethod", "ExploreAndLogin");
					writer.WriteValue(varInfo.Value);
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		#endregion
	}
}
