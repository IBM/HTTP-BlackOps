using System;
using System.Collections.Generic;
namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Interface for the Http Proxy
	/// </summary>
	public interface IHttpProxy
	{
		/// <summary>
		/// The host of the proxy
		/// </summary>
		string Host { get; set; }
		/// <summary>
		/// The port the proxy is listening to
		/// </summary>
        int Port { get; set; }
        /// <summary>
        /// The secure port the proxy is listening to
        /// </summary>
        int SecurePort { get; set; }
        /// <summary>
        /// Returns true if the proxy is started
        /// </summary>
        bool IsListening { get; }
		/// <summary>
		/// Method that starts the proxy
		/// </summary>
		void Start();
		/// <summary>
		/// Method that stops the proxy
		/// </summary>
		void Stop();
        /// <summary>
        /// Extra options collection <optionName,value>
        /// </summary>
        Dictionary<string, string> ExtraOptions {get;}
        /// <summary>
        /// Gets the network settings used by the proxy
        /// Can be used to configure the proxy to use a different proxy for the connection to site
        /// Can be used to configure invalid certificate callbacks
        /// Can be used to pass on credentials for a specific host (do not pass on credentials that are not tied to a host
        /// as it is a security concern)
        /// </summary>
        INetworkSettings NetworkSettings {get;}
	}
}
