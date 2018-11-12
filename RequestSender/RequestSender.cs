using RequestSender.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;


namespace RequestSender
{

	/// <summary>
	/// Sends a series of HTTP requests
	/// </summary>
	public class RequestSender
	{
		private WebRequestClient _httpClient;
		private HttpRequestInfo _prevRequest;
		private HttpResponseInfo _prevResponse;
		private SessionIdHelper _sessionIdHelper = new SessionIdHelper();
		private RemoteCertificateValidationCallback _certificateValidationCallback;
        private static string _communicationError = Resources.CommunicationError;
       
        private object _lock = new object();

        public static string CommunicationError
        {
            get { return RequestSender._communicationError; }
          
        }

		

		
		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="netSettings"></param>
		public RequestSender() : this(null) { }


		/// <summary>
		/// Network settings
		/// </summary>
		/// <param name="netSettings"></param>
		public RequestSender(INetworkSettings netSettings)
		{
			_httpClient = new WebRequestClient();
			if (netSettings == null)
			{
				netSettings = new DefaultNetworkSettings();
				netSettings.CertificateValidationCallback = _certificateValidationCallback;
			}
			_httpClient.SetNetworkSettings(netSettings);
            _httpClient.ShouldHandleCookies = true;

		}
		/// <summary>
		/// Sends all the requests in the attached file
		/// </summary>
		/// <param name="source"></param>
		public void Send(ITrafficDataAccessor source)
		{
			int id = -1;
			TVRequestInfo info = null;
            PatternTracker.Instance.PatternsToTrack = source.Profile.GetTrackingPatterns();

			while ((info = source.GetNext(ref id)) != null)
			{
             	SendRequest(source, info);
			}

		}

		/// <summary>
		/// Sends only the requests specified by id
		/// </summary>
		/// <param name="source"></param>
		/// <param name="idsToSend"></param>
		public void Send(ITrafficDataAccessor source, IEnumerable<int> idsToSend, int numberOfThreads = 1)
		{
            PatternTracker.Instance.PatternsToTrack = source.Profile.GetTrackingPatterns();
            Queue<TVRequestInfo> requestsToSend = new Queue<TVRequestInfo>();
            if (numberOfThreads == 1)
            {
                foreach (int id in idsToSend)
                {
                    TVRequestInfo info = source.GetRequestInfo(id);
                    if (info != null)
                    {
                        requestsToSend.Enqueue(info);
                        SendRequest(source, info);
                    }
                }
            }
            else
            {
                for (int idx = 0; idx < numberOfThreads; idx++)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(SendAsync));
                    t.Start(new object[2] { source, requestsToSend });
                }
                do
                {
                    Thread.Sleep(1000);
                    lock (_lock)
                    {
                        if (requestsToSend.Count == 0) return;
                    }
                }
                while (true);
            }
		}

        /// <summary>
        /// Sends http request asynchroneusly
        /// </summary>
        /// <param name="param"></param>
        private void SendAsync(object param)
        {
            object[] paramArray = param as object[];
            if (paramArray == null || paramArray.Length != 2) return;
            ITrafficDataAccessor source = paramArray[0] as ITrafficDataAccessor;
            Queue<TVRequestInfo> requestsToSend = paramArray[1] as Queue<TVRequestInfo>;
            if (source == null || requestsToSend == null) return;
            while (true)
            {
                TVRequestInfo info = null;
                lock (_lock)
                {
                    if (requestsToSend.Count == 0) return;
                    info = requestsToSend.Dequeue();
                    
                }
                if(info!=null) SendRequest(source, info);
            }
        }


		/// <summary>
		/// Sends the specified request info
		/// </summary>
		/// <param name="source"></param>
		/// <param name="info"></param>
		private void SendRequest(ITrafficDataAccessor source, TVRequestInfo info)
		{
			byte[] reqBytes = source.LoadRequestData(info.Id);
            string updatedRequest = PatternTracker.Instance.UpdateRequest(Constants.DefaultEncoding.GetString(reqBytes));

            HttpRequestInfo reqInfo = new HttpRequestInfo(updatedRequest);
			reqInfo.IsSecure = info.IsHttps;

			_sessionIdHelper.UpdateSessionIds(reqInfo, _prevRequest, _prevResponse);
            

			info.RequestTime = DateTime.Now;
			//save the request that will be sent
            source.SaveRequest(info.Id, Constants.DefaultEncoding.GetBytes(updatedRequest));
			try
			{
				_prevResponse = _httpClient.SendRequest(reqInfo);

				_prevRequest = reqInfo;

				//save the request and response
				if (_prevResponse != null)
				{
					info.ResponseTime = DateTime.Now;
                    PatternTracker.Instance.UpdatePatternValues(_prevResponse);
					source.SaveResponse(info.Id, _prevResponse.ToArray());
				}
				else
				{
					source.SaveResponse(info.Id, Constants.DefaultEncoding.GetBytes(_communicationError));
				}
			}
			catch (Exception ex)
			{
				SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Error playing back request {0}", ex.Message);
				source.SaveResponse(info.Id, Constants.DefaultEncoding.GetBytes(Resources.CommunicationError));
			}


		}
	}
}
