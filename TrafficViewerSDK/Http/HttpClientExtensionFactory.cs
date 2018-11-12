using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK.Http
{
    /// <summary>
    /// Loads HttpClientFactories
    /// </summary>
    public class HttpClientExtensionFactory : BaseExtensionFactory<IHttpClientFactory>
    {
		/// <summary>
		/// Specifies the functionality of this extension
		/// </summary>
        protected override TrafficViewerExtensionFunction ExtensionFunction
        {
            get { return TrafficViewerExtensionFunction.HttpClientFactory; }
        }

		/// <summary>
		/// Gets all available http clients
		/// </summary>
		/// <returns></returns>
        public override IList<IHttpClientFactory> GetExtensions()
        {
            IList<IHttpClientFactory> clientFactories = base.GetExtensions();

            clientFactories.Add(new TrafficViewerHttpClientFactory());

            return clientFactories;
        }
    }
}
