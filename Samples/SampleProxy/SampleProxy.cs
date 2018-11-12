using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerSDK.Http;

namespace SampleProxy
{
    public class SampleProxy:IHttpProxyFactory
    {

        public string Name
        {
            get { return "Sample Proxy"; }
        }

        public IHttpProxy MakeProxyServer(string host, int port, int securePort, TrafficViewerSDK.ITrafficDataAccessor dataStore)
        {
            return new SampleProxyClass(host, port, dataStore);
        }
    }
}
