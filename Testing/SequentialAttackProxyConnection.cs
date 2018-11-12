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
    public class SequentialAttackProxyConnection : AdvancedExploreProxyConnection
    {
        private SequentialAttackProxy _parentProxy;

        public SequentialAttackProxyConnection(TcpClient tcpClient, bool isSecure, INetworkSettings networkSettings, SequentialAttackProxy parentProxy, ITrafficDataAccessor dataStore):
            base(tcpClient, isSecure, dataStore, "Sequential Attack Proxy", networkSettings, false)
        {
            _parentProxy = parentProxy;
        }

        protected override HttpRequestInfo OnBeforeRequestToSite(HttpRequestInfo requestInfo)
        {
            requestInfo = base.OnBeforeRequestToSite(requestInfo);
            if (!_isNonEssential)
            {
                bool mutated;
                requestInfo = _parentProxy.HandleRequest(requestInfo, out mutated);
                if (mutated)
                {
                    CurrDataStoreRequestInfo.Description = "Custom Test";
                }
                TrafficDataStore.SaveRequest(CurrDataStoreRequestInfo.Id, requestInfo.ToArray(false));
                TrafficDataStore.UpdateRequestInfo(CurrDataStoreRequestInfo);
            }
            return requestInfo;
        }

        protected override HttpResponseInfo OnBeforeResponseToClient(HttpResponseInfo responseInfo)
        {
            responseInfo = base.OnBeforeResponseToClient(responseInfo);
            if (!_isNonEssential)
            {
                //validate if the test was successful
                if (_parentProxy.ValidateResponse(responseInfo))
                { 
                    //the test was found
                    CurrDataStoreRequestInfo.Description = "Vulnerable Response";
                    TrafficDataStore.UpdateRequestInfo(CurrDataStoreRequestInfo);
                }
            }
           

            return responseInfo;
        }
    }
}
