/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace TrafficCollector
{
    class TestController : ITestController
    {
        private TrafficViewerFile _trafficFile;

        public TestController(TrafficViewerFile dataFile)
        {
            _trafficFile = dataFile;
        }


        public void HandleIssueFound(string origRawReq, string origRawResp, Uri requestUri, string englishIssueTypeName, string parameterName, List<string> testRequestList, List<string> testResponseList, string validation, string comment)
        {
            string testRequest = String.Empty;
            string testResponse = String.Empty;
            int count = testRequestList.Count;
            for (int idx = 0; idx < count; idx++)
            {
                testRequest = testRequestList[idx];
                testResponse = testResponseList[idx];
                SaveRequest(String.Format("{0} {1}/{2} - Vulnerability", englishIssueTypeName, idx + 1, count), new HttpRequestInfo(testRequest), requestUri.Scheme.Equals("https"), testResponse, DateTime.Now, DateTime.Now, validation);
            }
        }

        private TVRequestInfo SaveRequest(string description, HttpRequestInfo reqInfo, bool isSecure, string testResponse, DateTime requestTime, DateTime responseTime, string validation)
        {
            TVRequestInfo newReqInfo = new TVRequestInfo();
            newReqInfo.Description = description;
            newReqInfo.Validation = validation;
            newReqInfo.Host = reqInfo.Host;
            newReqInfo.ResponseStatus = HttpResponseInfo.GetResponseStatus(testResponse);
            newReqInfo.RequestTime = requestTime;

            newReqInfo.IsHttps = isSecure;
            newReqInfo.ThreadId = Utils.GetCurrentWin32ThreadId().ToString();
            newReqInfo.RequestLine = reqInfo.RequestLine;
            int newId = _trafficFile.AddRequestInfo(newReqInfo);
            _trafficFile.SaveRequest(newId, Constants.DefaultEncoding.GetBytes(reqInfo.ToString(false)));
            if (!String.IsNullOrWhiteSpace(testResponse))
            {
                _trafficFile.SaveResponse(newId, Constants.DefaultEncoding.GetBytes(testResponse));
                newReqInfo.ResponseTime = responseTime;
            }
            return newReqInfo;
        }

        public void Log(string format, params object[] args)
        {
            //do nothing
        }

        public string SendTest(string mutatedRequest, string host, int port, bool useSSL)
        {
            IHttpClient client = new WebRequestClient();
            HttpRequestInfo reqInfo = new HttpRequestInfo(mutatedRequest);
            reqInfo.IsSecure = useSSL;
            reqInfo.Host = host;
            reqInfo.Port = port;
            HttpResponseInfo resp = null;
            try
            {
                resp = client.SendRequest(reqInfo);
            }
            catch
            { }
            string response;
            if (resp != null)
            {
                PatternTracker.Instance.UpdatePatternValues(resp);
                response = resp.ToString();
            }
            else
            {
                response = String.Empty;
            }

            return response;
        }

        public string UpdateSessionIds(string mutatedRequest, bool useSSL)
        {
            return PatternTracker.Instance.UpdateRequest(mutatedRequest);
        }
    }
}
