using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Options;
using System.Reflection;
using System.IO;
using TrafficViewerSDK.Properties;

namespace TrafficViewerSDK.Http
{
    /// <summary>
    /// Constructs http clients
    /// </summary>
    public class TrafficViewerHttpClientFactory : TrafficViewerSDK.Http.IHttpClientFactory
    {
        private object _lock = new object();

        /// <summary>
        /// The type of client this factory will make
        /// </summary>
        public string ClientType
        {
            get
            {
                return Resources.TrafficViewerHttpClientName;
            }
        }

        /// <summary>
        /// Makes a new traffic viewer http client
        /// </summary>
        /// <returns></returns>
        public IHttpClient MakeClient()
        {
            return new TrafficViewerHttpClient();
        }

		/// <summary>
		/// Action to execute on load - does nothing for this implementation
		/// </summary>
		public void OnLoad()
		{ 
			//do nothing
		}

    }
}

