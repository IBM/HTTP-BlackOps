using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;
using System.Xml;

namespace TrafficViewerControls
{
	public class RequestDomView : BrowserView
	{
		protected override string ExtensionOverride
		{
			get
			{
				return ".xml";
			}
		}

		protected override byte[] GetResponseBody(HttpResponseInfo respInfo)
		{
			XmlDocument doc;
			HtmlParserHelper.Parse(respInfo, out doc);
			byte[] xmlBytes = Constants.DefaultEncoding.GetBytes(doc.InnerXml);
			return xmlBytes;
		}

	}
}
