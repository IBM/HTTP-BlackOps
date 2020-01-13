
/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
 */
using TrafficViewerSDK;
using TrafficViewerSDK.Http;
using TrafficViewerSDK.Options;
using System;
using System.IO;
using TrafficServer;
using TrafficCollector.Properties;
using Testing;
using System.Collections.Generic;

namespace TrafficCollector
{
    /// <summary>
    /// This is a virtual HTTP client, it is in fact handling HTTP requests that come into the server
    /// and controls the operation
    /// </summary>
    class CollectorAPIController : IHttpClient
    {
        public HttpResponseInfo SendRequest(HttpRequestInfo requestInfo)
        {
            requestInfo.ParseVariables();
            switch (requestInfo.Path)
            {
                case "/": return GetResponse(200, "OK", Resources.Usage, requestInfo.FullUrl);
                case "/start": return StartProxy(requestInfo);
                case "/stop": return StopProxy(requestInfo);
            }

            return GetResponse(404,"Not Found","");
        }


        private HttpResponseInfo StartProxy(HttpRequestInfo requestInfo)
        {
            IHttpProxy proxy;

            //get the port from the url
            string portString = null;
            requestInfo.QueryVariables.TryGetValue("port", out portString);
            //optional secret to protect the recording session
            string secret = null;
            requestInfo.QueryVariables.TryGetValue("secret", out secret);
            //the host to record traffic for
            string targetHost = null;
            requestInfo.QueryVariables.TryGetValue("targetHost", out targetHost);
            //whether to execute inline tests
            string test = null;
            requestInfo.QueryVariables.TryGetValue("test", out test);

            int port;
            if (int.TryParse(portString, out port) && port >= 0 && port <= 65535)
            {
                if (CollectorProxyList.Instance.ProxyList.ContainsKey(port))
                {
                    return GetResponse(400, "Bad Request", "Port in use.");
                }

                if (targetHost == null)
                {
                    return GetResponse(400, "Bad Request", "'targetHost' parameter is not specified.");
                }

                if (!Utils.IsMatch(targetHost, "^[\\w._-]+$") || !Utils.IsMatch(targetHost,TrafficCollectorSettings.Instance.AllowedHostsPattern))
                {
                    return GetResponse(400, "Bad Request", "Invalid target host!");
                }

                try
                {
                    TrafficViewerFile trafficFile = new TrafficViewerFile();
                    trafficFile.Profile = ParsingOptions.GetRawProfile();
                    //optional secret to prevent others stopping the recording
                    if(!String.IsNullOrWhiteSpace(secret)) trafficFile.Profile.SetSingleValueOption("secret", secret);
                    trafficFile.Profile.SetSingleValueOption("targetHost", targetHost);

                    
                    if (test != null && test.Equals("true"))
                    {
                        CustomTestsFile testsFile = new CustomTestsFile();
                        testsFile.Load(TrafficCollectorSettings.Instance.TestFile);
                        Dictionary<string, AttackTarget> targetDef = new Dictionary<string, AttackTarget>();
                        targetDef.Add("targetHost", new AttackTarget("targetHost", "Enabled", String.Format("Host:\\s*{0}",targetHost)));
                        testsFile.SetAttackTargetList(targetDef);

                        proxy = new DriveByAttackProxy(new TestController(trafficFile), testsFile, trafficFile, TrafficCollectorSettings.Instance.Ip, port);
                    }
                    else
                    {
                        proxy = new AdvancedExploreProxy(TrafficCollectorSettings.Instance.Ip, port, trafficFile);
                    }
                    proxy.Start();
                    CollectorProxyList.Instance.ProxyList.Add(proxy.Port, proxy);
                }
                catch (Exception ex)
                {
                    return GetResponse(500, "Unexpected error.", ex.Message);
                }
            }
            else
            {
                return GetResponse(400, "Bad Request", "Invalid 'port' parameter.");
            }


            return GetResponse(200, "OK", "Proxy is listening on port: {0}.", proxy.Port);
        }


        private HttpResponseInfo StopProxy(HttpRequestInfo requestInfo)
        {
            string report = "";
            //get the port from the url
            string portString = null;
            requestInfo.QueryVariables.TryGetValue("port", out portString);
            //optional secret to protect the recording session
            string secret = null;
            requestInfo.QueryVariables.TryGetValue("secret", out secret);
            //optional flag indicating if similar requests should be skiped
            string skipSimilar = null;
            requestInfo.QueryVariables.TryGetValue("skipSimilar", out skipSimilar);
            //the file to save to
            string fileName = null;
            requestInfo.QueryVariables.TryGetValue("fileName", out fileName);

            if (fileName == null)
            {
                //assign a random file name
                fileName = DateTime.Now.Ticks.ToString();
            }

            if (!Utils.IsMatch(fileName, "^[\\w._-]+$"))
            {
                return GetResponse(400, "Bad Request", "Invalid file name.");
            }

            int port;
            if (int.TryParse(portString, out port))
            {
                if (!CollectorProxyList.Instance.ProxyList.ContainsKey(port))
                {
                    return GetResponse(400, "Bad Request", "Port not found.");
                }
                else
                {
                    IHttpProxy proxy = CollectorProxyList.Instance.ProxyList[port];
                    TrafficViewerFile trafficFile = (proxy as ManualExploreProxy).TrafficDataStore as TrafficViewerFile;

                    //check the secret if it exists
                    string configuredSecret = trafficFile.Profile.GetOption("secret") as String;
                    if (!String.IsNullOrWhiteSpace(configuredSecret) && !configuredSecret.Equals(secret))
                    {
                        return GetResponse(401, "Unauthorized", "Invalid secret.");
                    }

                    string filePath = Path.Combine(TrafficCollectorSettings.Instance.DumpDir, fileName + ".htd");

                    
                    if (proxy is DriveByAttackProxy)
                    {
                        DriveByAttackProxy dProx = proxy as DriveByAttackProxy;
                        int requestsLeft = dProx.RequestsLeft;
                        if (requestsLeft > 0)
                        {
                            return GetResponse(206, "Partial Content", "Please wait... {0} request(s) left, {1} test job(s) in queue", requestsLeft, dProx.TestCount);
                        }
                        else
                        {
                            int id = -1;
                            TVRequestInfo info = null;
                            report = "\r\n\r\nVulnerability List\r\n";
                            report += "============================\r\n";
                            int count = 0;
                            while ((info = trafficFile.GetNext(ref id)) != null)
                            {
                                if (info.Description.Contains("Vulnerability"))
                                {
                                    count++;
                                    report += String.Format("Request {0} - {1} ({2})\r\n",info.RequestLine,info.Description, info.Validation);
                                }
                            }
                            report += String.Format("Total: {0}\r\n", count);
                        }

                    }

                    if (File.Exists(filePath)) //load the existing file and check the secret
                    {
                        TrafficViewerFile existingFile = new TrafficViewerFile();
                        existingFile.Open(filePath);
                        configuredSecret = existingFile.Profile.GetOption("secret") as String;
                        existingFile.Close(false);

                        if (String.IsNullOrWhiteSpace(configuredSecret) || String.IsNullOrWhiteSpace(secret) || !configuredSecret.Equals(secret))
                        {
                            return GetResponse(401, "Unauthorized", "Cannot override existing file.");
                        }
                    }


                    proxy.Stop();
                    CollectorProxyList.Instance.ProxyList.Remove(port);
                    if (trafficFile.RequestCount > 0)
                    {
                        if (skipSimilar != null && skipSimilar.Equals("true", StringComparison.OrdinalIgnoreCase))
                        {
                            trafficFile = removeSimilar(trafficFile);
                        }

                        trafficFile.Save(filePath);
    
                        report += String.Format("Traffic file saved at '{0}'\r\n", filePath);
                    }
                    else
                    {
                        report += "Nothing recorded.";
                    }

                }
               
            }
            else
            {
                return GetResponse(400, "Bad Request", "Invalid 'port' parameter.");
            }

            return GetResponse(200, "OK", "Proxy stopped. {0}", report);
        }

        private TrafficViewerFile removeSimilar(TrafficViewerFile source)
        {
            TrafficViewerFile dest = new TrafficViewerFile();
            TVRequestInfo info;
            int id = -1;
            List<int> _reqHashes = new List<int>();

            while ((info = source.GetNext(ref id)) != null)
            {
                byte[] request = source.LoadRequestData(info.Id);
                HttpRequestInfo reqInfo = new HttpRequestInfo(request, true);
                int hash = reqInfo.GetHashCode(TrafficServerMode.BrowserFriendly);

                if (!_reqHashes.Contains(hash))
                {
                    byte[] response = source.LoadResponseData(info.Id);
                    dest.AddRequestResponse(request, response);
                    _reqHashes.Add(hash);
                }
            }

            return dest;

        }

        /// <summary>
        /// Gets an HTTP response
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private HttpResponseInfo GetResponse(int statusCode, string status, string message, params object[] args)
        {
            message = String.Format(message, args);
            string rawResponse = String.Format("HTTP/1.1 {0} {1}\r\nContent-type:text/plain\r\n\r\n \t{2}\r\n", statusCode, status, message);
            return new HttpResponseInfo(rawResponse);
        }

        public void SetNetworkSettings(INetworkSettings networkSettings)
        {
           //do nothing
        }

    }
}
