using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViewerSDK.Http;
using System.Net.Sockets;
using TrafficViewerSDK;
using TrafficServer.Properties;
using TVDiff.Algorithms;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using TrafficViewerSDK.Options;

namespace TrafficServer 
{
	/// <summary>
	/// Proxy connection that replaces a special alias path with the path extracted from the original request context
	/// </summary>
	public class TrackingReverseProxyConnection : AdvancedExploreProxyConnection
	{
		private string REQ_ID_RX = AdvancedExploreProxyConnection.REQ_ID_STRING + "(\\d+)";
		private TVRequestInfo _trackingReqInfo = null;
		private IHttpProxy _parentProxy;
		private ITrafficDataAccessor _dataStore;
        private string _originalRawRequest;
		private INetworkSettings _networkSettings;
        private string TRAFFIC_VIEWER_HEADER = "APPSCAN-BLACK-OPS-REQ-ID";

		/// <summary>
		/// Creates a connection that will forward the request to the specified host and port
		/// </summary>
		/// <param name="fwHost"></param>
		/// <param name="fwPort"></param>
		/// <param name="client"></param>
		/// <param name="isSecure"></param>
		/// <param name="dataStore"></param>
		/// <param name="networkSettings"></param>
		/// <param name="parentProxy"></param>
		public TrackingReverseProxyConnection(TcpClient client, bool isSecure, ITrafficDataAccessor dataStore, INetworkSettings networkSettings, IHttpProxy parentProxy)
			: base(client, isSecure, dataStore, Resources.TrackingReverseProxyDescription, networkSettings, false)
		{
			_dataStore = dataStore;
			_parentProxy = parentProxy;
			_networkSettings = networkSettings;
		}

        

		/// <summary>
		/// Identifies aliases and in the first stage replaces them with the original value
		/// </summary>
		/// <param name="requestInfo"></param>
		/// <returns></returns>
		protected override HttpRequestInfo OnBeforeRequestToSite(HttpRequestInfo requestInfo)
		{
			
			requestInfo = base.OnBeforeRequestToSite(requestInfo);
            
            _trackingReqInfo = null;
            
			//update the request info
			if (requestInfo.Path.Contains(AdvancedExploreProxyConnection.REQ_ID_STRING))//this is an alias request
			{ 
				//get the req id
				string idString = Utils.RegexFirstGroupValue(requestInfo.Path, REQ_ID_RX);

				//load the original path
				int id;
				if (int.TryParse(idString, out id))
				{
					_trackingReqInfo = _dataStore.GetRequestInfo(id);
					if (_trackingReqInfo != null)
					{
						var lastPath = _trackingReqInfo.UpdatedPath;
                        
						if (String.IsNullOrWhiteSpace(lastPath))
						{
							throw new Exception(String.Format("Original path not the traffic file for request id: {0}", id));
						}
						else
						{
							HttpServerConsole.Instance.WriteLine("Path substituted to: '{0}'", lastPath);
						}
						requestInfo.Path = lastPath;
                        AddSpecialHeader(requestInfo);
						_originalRawRequest = requestInfo.ToString();
					}

				}
			}

			return requestInfo;
		}

        private void AddSpecialHeader(HttpRequestInfo requestInfo)
        {
            if (requestInfo.Headers[TRAFFIC_VIEWER_HEADER] == null)
            {
                requestInfo.Headers.Add(TRAFFIC_VIEWER_HEADER, _trackingReqInfo.Id.ToString());
            }
        }


		/// <summary>
		/// If the response obtained is different than original attempt to update the path and resend to obtain a good response
		/// </summary>
		/// <param name="responseInfo"></param>
		/// <returns></returns>
		protected override HttpResponseInfo OnBeforeResponseToClient(HttpResponseInfo responseInfo)
		{
			
			responseInfo = base.OnBeforeResponseToClient(responseInfo);

			if (_trackingReqInfo != null)
			{
				//this is a tracked request we may need to update it
				if (IsDifferentThanOriginalResponse(responseInfo))
				{ 
					//we need to obtain the referer response and update the request
					if (_trackingReqInfo.RefererId > -1 && !String.IsNullOrWhiteSpace(_trackingReqInfo.RequestContext))
					{

						//load the referer request
						var refererInfo = _dataStore.GetRequestInfo(_trackingReqInfo.RefererId);
						var refererReqBytes = _dataStore.LoadRequestData(_trackingReqInfo.RefererId);
						if (refererReqBytes != null && refererReqBytes.Length > 0)
						{
							var refererHttpInfo = new HttpRequestInfo(refererReqBytes);
							refererHttpInfo.IsSecure = refererInfo.IsHttps;
							//update request cookie headers from the current request
							refererHttpInfo.Cookies.Clear();
							refererHttpInfo.Headers["Cookie"] = _requestInfo.Headers["Cookie"];

							//now that the referer request is ready send it to the tracking proxy where if the response is different than the original it will in turn
							//get its referer resent
							IHttpClient trackingProxyClient = GetTrackingProxyHttpClient(); //following lines equivalent to a recursive call

							var refererUpdatedResponse = trackingProxyClient.SendRequest(refererHttpInfo);
							string refererRawResponse = refererUpdatedResponse.ToString();


							//update autotrack traccking patterns
							string updatedRequestValue = Utils.RegexFirstGroupValue(refererRawResponse, _trackingReqInfo.RequestContext);



							if (!String.IsNullOrWhiteSpace(updatedRequestValue) 
								&& AutoTrackingPatternList.ContainsKey(_trackingReqInfo.TrackingPattern))
							{
								TrackingPattern curPattern = AutoTrackingPatternList[_trackingReqInfo.TrackingPattern];

								//update request
								_originalRawRequest = Utils.ReplaceGroups(_originalRawRequest, 
									curPattern.RequestPattern, updatedRequestValue);
							}
							else
							{
								HttpServerConsole.Instance.WriteLine(LogMessageType.Error,
									"Could not update autodetect parameter for tracked request id {0}. Will resend anyway",												_trackingReqInfo.Id);
							}
                            //update patterns
                            string updatedRawRequest = PatternTracker.Instance.UpdateRequest(_originalRawRequest);
                            updatedRawRequest = UpdateDynamicPatterns(updatedRawRequest);
                            HttpRequestInfo newRequest = new HttpRequestInfo(updatedRawRequest);
                            
                            newRequest.IsSecure = _requestInfo.IsSecure;
                            AddSpecialHeader(newRequest);

							if (!_trackingReqInfo.UpdatedPath.Equals(newRequest.Path))
							{

								_trackingReqInfo.UpdatedPath = newRequest.Path;
								//update the path in the ui
								_dataStore.UpdateRequestInfo(_trackingReqInfo);
								//now send the request one more time
								
							}
						

                            //send the request with the normal HTTP client this time
                            responseInfo = HttpClient.SendRequest(newRequest);
                            //no matter what we return the last value
                            PatternTracker.Instance.UpdatePatternValues(responseInfo);

						}
						else
						{
							HttpServerConsole.Instance.WriteLine(LogMessageType.Error,
											"Missing http traffic for request id {0}", _trackingReqInfo.Id);
						}
					}
					else
					{
						HttpServerConsole.Instance.WriteLine(LogMessageType.Error,
										"Missing referer or request context for request id {0}", _trackingReqInfo.Id);
					}
					
				}
				else
				{
					HttpServerConsole.Instance.WriteLine(LogMessageType.Information,
									"Response similar for request id {0}", _trackingReqInfo.Id);
				}
			}
			return responseInfo;
		}

	

		private IHttpClient GetTrackingProxyHttpClient()
		{
			IHttpClient webClient = new WebRequestClient();
			DefaultNetworkSettings netSettings = new DefaultNetworkSettings();
			//send the referer request to the tracking proxy
			netSettings.CertificateValidationCallback = _networkSettings.CertificateValidationCallback; 
				
				
			netSettings.WebProxy = new WebProxy(_parentProxy.Host, _parentProxy.Port);
			webClient.SetNetworkSettings(netSettings);
			return webClient;
		}

		/// <summary>
		/// Calculates if the current response is different than the original response
		/// </summary>
		/// <param name="responseInfo"></param>
		/// <returns></returns>
		private bool IsDifferentThanOriginalResponse(HttpResponseInfo responseInfo)
		{
			bool isDifferent;
			if (!_trackingReqInfo.ResponseStatus.Equals(responseInfo.Status.ToString()))
			{
				isDifferent = true;
			}
			else if (ResponseLengthIsSignificantlyDifferent(responseInfo, _trackingReqInfo))
			{
				isDifferent = true;
			}
			else //calculate response similarity
			{
				byte[] oldRespBytes = _dataStore.LoadResponseData(_trackingReqInfo.Id);
				string oldRespString = Constants.DefaultEncoding.GetString(oldRespBytes);

				double similarity = ASESimilarityAlgorithm.CalculateSimilarity(oldRespString, responseInfo.ToString());

				isDifferent = similarity < 0.98;
			}
			return isDifferent;
		}

		private bool ResponseLengthIsSignificantlyDifferent(HttpResponseInfo responseInfo, TVRequestInfo _trackingReqInfo)
		{
			int curResponseLen = responseInfo.ResponseHead.Length + responseInfo.ResponseBody.Length;
			int diff = _trackingReqInfo.ResponseLength - curResponseLen;

			diff = diff > 0 ? diff : diff * -1;

			return diff > 10000;	
		}

	}
}
