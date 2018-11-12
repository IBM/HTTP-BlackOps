using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Options;
using System.Reflection;
using System.IO;
using TrafficViewerSDK.Http;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Constructs proxies
	/// </summary>
	public class HttpProxyFactory : BaseExtensionFactory<IHttpProxyFactory>
	{

		/// <summary>
		/// Returns the extension function
		/// </summary>
		protected override TrafficViewerExtensionFunction ExtensionFunction
		{
			get { return TrafficViewerExtensionFunction.HttpProxyFactory; }
		}

		/// <summary>
		/// Returns all available http proxy factories
		/// </summary>
		/// <returns></returns>
        public override IList<IHttpProxyFactory> GetExtensions()
		{
            IList<IHttpProxyFactory> proxyFactories = base.GetExtensions();
			return proxyFactories;
		}
	}
}
