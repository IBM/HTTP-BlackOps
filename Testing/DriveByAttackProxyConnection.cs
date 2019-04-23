/**
Copyright 2019 Trend Micro, Incorporated, All Rights Reserved.
SPDX-License-Identifier: Apache-2.0
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TrafficServer;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace Testing
{
    public class DriveByAttackProxyConnection : AdvancedExploreProxyConnection
    {
        private DriveByAttackProxy _parentProxy;

        public DriveByAttackProxyConnection(TcpClient tcpClient, bool isSecure, INetworkSettings networkSettings, DriveByAttackProxy parentProxy, ITrafficDataAccessor dataStore) :
            base(tcpClient, isSecure, dataStore, "Drive By Attack Proxy", networkSettings, false)
        {
            _parentProxy = parentProxy;
        }


        protected override HttpResponseInfo OnBeforeResponseToClient(HttpResponseInfo responseInfo)
        {
            if (!_isNonEssential)
            {
                _parentProxy.HandleRequest(_requestInfo, responseInfo);
            }
            return base.OnBeforeResponseToClient(responseInfo);
        }

        
    }
}
