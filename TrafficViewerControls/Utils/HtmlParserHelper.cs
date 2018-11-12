using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TVHtmlParser;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;
using System.Diagnostics;

namespace TrafficViewerControls
{
	public class HtmlParserHelper
	{
		/// <summary>
		/// Parses Html into XmlDocument
		/// </summary>
		/// <param name="responseInfo"></param>
		/// <param name="doc"></param>
		/// <returns></returns>
		public static void Parse(HttpResponseInfo responseInfo, out XmlDocument doc)
		{
			doc = null;
			try
			{
				
				string html = responseInfo.ResponseBody.ToString(responseInfo.Headers["Content-Type"]);
				HtmlParser parser = new HtmlParser();

				parser.Parse(html, out doc);
			}
			catch (Exception ex)
			{
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "HtmlParser: Error parsing html response: {0}", ex.Message);
			}
		}
	}
}
