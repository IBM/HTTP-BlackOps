using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;

namespace TrafficServer
{
	/// <summary>
	/// Proxy that allows the user to do trap requests and responses
	/// </summary>
	public class AdvancedExploreProxy : ManualExploreProxy
	{

		/// <summary>
		/// Creates a  proxy to be used for traffic recording purposes which also allows trapping requests and responses
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="dataStore">Where the traffic data will be stored</param>
		public AdvancedExploreProxy(string host, int port, ITrafficDataAccessor dataStore)
			: base(host, port, dataStore) 
		{
            PatternTracker.Instance.PatternsToTrack = dataStore.Profile.GetTrackingPatterns();
            ExtraOptions.Add(TRACK_REQUEST_CONTEXT_OPTION, "false");
		}


        public override void Start()
        {
            if (!bool.TryParse(ExtraOptions[TRACK_REQUEST_CONTEXT_OPTION], out _trackRequestContext))
            {
                HttpServerConsole.Instance.WriteLine("Invalid option for '{0}'. Using 'false'", TRACK_REQUEST_CONTEXT_OPTION);
            }
            base.Start();
        }

		/// <summary>
		/// Gets a http connection that can trap requests and responses
		/// </summary>
		/// <param name="clientInfo"></param>
		/// <returns></returns>
		protected override IProxyConnection GetConnection(TcpClientInfo clientInfo)
		{
			return new AdvancedExploreProxyConnection(clientInfo.Client, clientInfo.IsSecure, TrafficDataStore, "Manual Explore", NetworkSettings, _trackRequestContext);
		}


		private bool _trackRequestContext = false;
        private string TRACK_REQUEST_CONTEXT_OPTION = "TrackRequestContext";
		/// <summary>
		/// Whether to capture where the request was extracted from
		/// </summary>
		public bool TrackRequestContext
		{
			get { return _trackRequestContext; }
		}

		
	}
}
