using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace SampleProxy
{
    public class SampleProxyClass : ManualExploreProxy
    {
        private ITrafficDataAccessor _trafficDataStore;
        public SampleProxyClass(string host, int port, ITrafficDataAccessor dataStore)
            : base(host, port, dataStore) 
		{
            _trafficDataStore = dataStore;
            ExtraOptions.Add("SampleProxyOption", "Sample Value");
		}

        public override void Start()
        {
            HttpServerConsole.Instance.WriteLine("Starting Sample Proxy");
            base.Start();
        }
        
        protected override IProxyConnection GetConnection(TcpClientInfo clientInfo)
        {
            return new ManualExploreProxyConnection(clientInfo.Client, 
                clientInfo.IsSecure, 
                _trafficDataStore, 
                "Sample Proxy Description", NetworkSettings);
        }
    }
}
