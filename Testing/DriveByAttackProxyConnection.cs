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

        protected override HttpRequestInfo OnBeforeRequestToSite(HttpRequestInfo requestInfo)
        {
            if (!_isNonEssential)
            {
                requestInfo = _parentProxy.HandleRequest(requestInfo);
            }
            
            requestInfo = base.OnBeforeRequestToSite(requestInfo);
            
            return requestInfo;
        }

        
    }
}
