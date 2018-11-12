using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Properties;
using System.Xml;
using TrafficViewerSDK.Http;

namespace TrafficViewerSDK.Exporters
{
	class SequenceExporter : LoginExporter
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
			get { return Resources.SequenceExporter; }
		}

		public override string Extension
		{
			get { return "seq"; }
		}

		protected override void Export(ITrafficDataAccessor source, System.IO.Stream stream, bool overwriteScheme ,bool isSSL, string newHost, int newPort)
		{
			TVRequestInfo info;

			int i=-1;

			string overridenScheme = isSSL ? "https" : "http";
			//using an xml writer for memory concerns
			XmlWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
			writer.WriteStartDocument();
			writer.WriteStartElement("StateInducer");
			writer.WriteAttributeString("Version","1.1");
			

			writer.WriteStartElement("Sequences");
			writer.WriteStartElement("Sequence");
			
			writer.WriteAttributeString("Name", "Exported from X-Force Black Ops");
			writer.WriteAttributeString("Enabled", "True");
			writer.WriteAttributeString("ReplayOptimizationsEnabled", "False");
			writer.WriteAttributeString("TestInIsolation", "True");
			writer.WriteAttributeString("TestSingleThreadedly", "True");
			writer.WriteAttributeString("ManualExploreEnabled", "False");
			writer.WriteAttributeString("LoginRequired", "True");


			writer.WriteStartElement("requests");
			
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
                    HttpResponseInfo respInfo = new HttpResponseInfo(respData);
                    //add to the list of variables
                    AddToVariableInfoCollection(reqInfo.Cookies.GetVariableInfoCollection());
                    AddToVariableInfoCollection(reqInfo.QueryVariables.GetVariableInfoCollection());
                    AddToVariableInfoCollection(reqInfo.BodyVariables.GetVariableInfoCollection());
                    AddToVariableInfoCollection(reqInfo.PathVariables.GetVariableInfoCollection());

					WriteRequest(writer, newHost, newPort, scheme, reqInfo,respInfo);
                }

			}

			writer.WriteEndElement();

            //writer.WriteRaw(Resources.VariableDefinitions);
			
			writer.WriteEndElement();
			writer.WriteEndElement();

			writer.WriteEndElement();
			writer.WriteEndDocument();

			writer.Flush();
			writer.Close();

			
		}

		#endregion
	}
}
