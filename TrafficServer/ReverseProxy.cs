using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;

namespace TrafficServer
{
	/// <summary>
	/// A proxy that can be used to intercept Http Requests when the client doesn't support the use of a proxy
	/// </summary>
	public class ReverseProxy : BaseProxy
	{
        public static readonly string FORWARDING_HOST_OPT = "ForwardingHost";
        public static readonly string FORWARDING_PORT_OPT = "ForwardingPort";

		private string _forwardingHost;
		private int _forwardingPort;
		private ITrafficDataAccessor _dataStore;
		private Dictionary<string, string> _replacements;
		/// <summary>
		/// Regex strings to be replaced before sending the request
		/// </summary>
		public Dictionary<string, string> Replacements
		{
			get { return _replacements; }
			set { _replacements = value; }
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="forwardingHost">The hosts thar requests will be sent to</param>
		/// <param name="forwardingPort">The forwarding port</param>
		/// <param name="host">The host of the proxy</param>
		/// <param name="port">The port of the proxy</param>
		/// <param name="dataStore">Data store where the requests will be saved</param>
		public ReverseProxy(string host, int port, int securePort, ITrafficDataAccessor dataStore) :
			base(host, port, securePort)
		{
            ExtraOptions.Add(ReverseProxy.FORWARDING_HOST_OPT, String.Empty);
            ExtraOptions.Add(ReverseProxy.FORWARDING_PORT_OPT, String.Empty);
			_dataStore = dataStore;
		}


		public override void Start()
		{
            if (ExtraOptions.ContainsKey(ReverseProxy.FORWARDING_HOST_OPT) && !String.IsNullOrWhiteSpace(ExtraOptions[ReverseProxy.FORWARDING_HOST_OPT]))
            {
                _forwardingHost = ExtraOptions[ReverseProxy.FORWARDING_HOST_OPT];
            }
            else
            {
                HttpServerConsole.Instance.WriteLine(LogMessageType.Error,
                "Need to configure 'ForwardingHost' extra proxy option");
                return;
            }


            if (ExtraOptions.ContainsKey(ReverseProxy.FORWARDING_PORT_OPT) && !String.IsNullOrWhiteSpace(ExtraOptions[ReverseProxy.FORWARDING_PORT_OPT]))
            {
                string fwPortString = ExtraOptions[ReverseProxy.FORWARDING_PORT_OPT];
                if (!int.TryParse(fwPortString, out _forwardingPort))
                {
                    HttpServerConsole.Instance.WriteLine(LogMessageType.Error,
                "Invalid value for 'ForwardingPort' extra option");
                    return;
                }

            }
            else
            {
                HttpServerConsole.Instance.WriteLine(LogMessageType.Error,
                "Need to configure 'ForwardingPort' extra proxy option");
                return;
            }

			base.Start();
			HttpServerConsole.Instance.WriteLine(LogMessageType.Warning,
				"All requests (including attacks) will be forwarded to {0}:{1}",
				_forwardingHost,
				_forwardingPort);
		}

		/// <summary>
		/// Gets a connection
		/// </summary>
		/// <param name="clientInfo"></param>
		/// <returns></returns>
		protected override IProxyConnection GetConnection(TcpClientInfo clientInfo)
		{
			return new ReverseProxyConnection(_forwardingHost, _forwardingPort, 
				clientInfo.Client, clientInfo.IsSecure, _dataStore, NetworkSettings, Replacements);
		}


	}
}
