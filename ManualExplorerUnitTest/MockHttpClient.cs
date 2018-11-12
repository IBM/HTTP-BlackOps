using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;
using System.Net;

namespace ManualExplorerUnitTest
{
	public class MockHttpClient:IHttpClient
	{

		ITrafficDataAccessor _mockSite;

		public string Name
		{
			get { return "Mock Http Client"; }
		}


		public MockHttpClient(ITrafficDataAccessor mockSite)
		{
			_mockSite = mockSite;
		}

		public HttpResponseInfo SendRequest(HttpRequestInfo requestInfo)
		{
			int idx = -1;
			TVRequestInfo currDataSourceInfo = null;
			byte[] response = null;

			while((currDataSourceInfo = _mockSite.GetNext(ref idx))!=null)
			{
				if (String.Compare(currDataSourceInfo.RequestLine, requestInfo.RequestLine) == 0)
				{ 
					response = _mockSite.LoadResponseData(currDataSourceInfo.Id);
					break;
				}
			}
			HttpResponseInfo respInfo = null;
			if (response != null)
			{
				respInfo = new HttpResponseInfo(response);
			}
			return respInfo;
		}

	

		public void SetCredentials(NetworkCredential credentials)
		{
			;
		}


		public void SetNetworkSettings(INetworkSettings networkSettings)
		{
			throw new NotImplementedException();
		}
	}
}
