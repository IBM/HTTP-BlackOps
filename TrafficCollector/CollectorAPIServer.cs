/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
 */
using TrafficViewerSDK.Http;

namespace TrafficCollector
{
    class CollectorAPIServer : BaseProxy
    {
        protected override IProxyConnection GetConnection(TcpClientInfo clientInfo)
        {
            return new CollectorAPIConnection(clientInfo.Client, clientInfo.IsSecure);
        }

        public CollectorAPIServer(string host, int port) : base(host, port, 0) { }
    }
}
