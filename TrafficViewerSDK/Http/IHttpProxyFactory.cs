using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrafficViewerSDK.Http
{
    /// <summary>
    /// Interface used to extend the proxy functionality
    /// </summary>
    public interface IHttpProxyFactory
    {
        /// <summary>
        /// Gets the name to be displayed in the UI
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Constructs the proxy server
        /// </summary>
        /// <param name="host">The host to listen on for example the ip of the network card for public access</param>
        /// <param name="port">The unsecured port</param>
        /// <param name="securePort">The secure port</param>
        /// <param name="dataStore">The traffic data store used for various operations</param>
        /// <returns></returns>
        IHttpProxy MakeProxyServer(string host, int port, int securePort, ITrafficDataAccessor dataStore);

    }
}
