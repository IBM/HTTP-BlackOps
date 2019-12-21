/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
 */
using System.Net.Sockets;
using TrafficViewerSDK.Http;

namespace TrafficCollector
{
    class CollectorAPIConnection : BaseProxyConnection
    {
        IHttpClient _httpClient;

        public CollectorAPIConnection(TcpClient client, bool isSecure) : base(client, isSecure)
        {
            _httpClient = new CollectorAPIController();
        }

        protected override IHttpClient HttpClient
        {
            get
            {
                return _httpClient;
            }
        }
    }
}
