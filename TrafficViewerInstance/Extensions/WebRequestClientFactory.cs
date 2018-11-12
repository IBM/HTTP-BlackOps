using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerInstance.Properties;
using TrafficViewerSDK.Http;

namespace TrafficViewerInstance.Extensions
{
	class WebRequestClientFactory:IHttpClientFactory
	{
		public string ClientType
		{
			get { return Resources.WebRequestClientName; }
		}

		public IHttpClient MakeClient()
		{
			return new WebRequestClient();
		}

		public void OnLoad()
		{
			//do nothing
		}
	}
}
