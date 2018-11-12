using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViewerSDK.Options;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;
using System.IO;
using TrafficServer.Properties;
using TrafficViewerInstance;

namespace TrafficServer
{
	/// <summary>
	/// Proxy server used to read data from the traffic log
	/// </summary>
	public class TrafficStoreProxy : BaseProxy
	{
		private const string TRAFFIC_VIEWER_ENTRIES_START = "\r\n#Start Traffic Viewer hosts";
		private const string TRAFFIC_VIEWER_ENTRIES_END = "#End Traffic Viewer hosts";

		ITrafficDataAccessor _sourceStore;
		ITrafficDataAccessor _saveStore;
		
		private string _hostFilePath = Environment.GetEnvironmentVariable("systemroot") + @"\system32\drivers\etc\hosts";

		string _proxyDescription = Resources.TrafficLogProxyDescription;
		private TrafficServerMode _matchMode;
		private bool _ignoreAuth;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dataStore"></param>
		public TrafficStoreProxy(ITrafficDataAccessor dataStore):this(dataStore,dataStore){}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sourceStore"></param>
		/// <param name="saveStore"></param>
		public TrafficStoreProxy(ITrafficDataAccessor sourceStore, ITrafficDataAccessor saveStore)
			:
			this(
			sourceStore,
			saveStore,
			TrafficViewerOptions.Instance.TrafficServerIp,
			TrafficViewerOptions.Instance.TrafficServerPort,
            TrafficViewerOptions.Instance.TrafficServerPortSecure
			)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sourceStore"></param>
		/// <param name="saveStore"></param>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <param name="securePort"></param>
		public TrafficStoreProxy(
			ITrafficDataAccessor sourceStore, 
			ITrafficDataAccessor saveStore, 
			string host, 
			int port, 
			int securePort
		) 
			:
		base(host, port, securePort)
		{
			
			_sourceStore = sourceStore;
			_saveStore = saveStore;
            
            ExtraOptions.Add("MatchMode", "BrowserFriendly");
            ExtraOptions.Add("IgnoreAuth", "false");
            
			

			TrafficServerCache.Instance.Clear();
		}


		protected override IProxyConnection GetConnection(TcpClientInfo clientInfo)
		{
			return new TrafficStoreProxyConnection(
				_sourceStore,
				_matchMode,
				_ignoreAuth, 
				clientInfo.Client, 
				clientInfo.IsSecure,
				_saveStore,
				_proxyDescription);
		}


		/// <summary>
		/// Add the hosts in the traffic log to the host file
		/// </summary>
		private void AddHosts()
		{



			HttpServerConsole.Instance.WriteLine(LogMessageType.Warning,
			"Using the host of the first request");
			int i = -1;
			TVRequestInfo tvReqInfo = _sourceStore.GetNext(ref i);
			if (tvReqInfo != null)
			{
				HttpRequestInfo httpReqInfo = new HttpRequestInfo(_sourceStore.LoadRequestData(tvReqInfo.Id));

				if (httpReqInfo.Host == String.Empty)
				{
					HttpServerConsole.Instance.WriteLine(LogMessageType.Error,
				"Could not find a host. You might need to add the host manually");
				}
			}

		}

		/// <summary>
		/// Removes traffic viewer entries from the hosts file
		/// </summary>
		private void RemoveHosts()
		{
			if (File.Exists(_hostFilePath))
			{
				string hostFileContents = File.ReadAllText(_hostFilePath);
				int indexOfTVEntries = hostFileContents.IndexOf(TRAFFIC_VIEWER_ENTRIES_START);
				if (indexOfTVEntries > -1)
				{
					int indexOfTVEnd = hostFileContents.LastIndexOf(TRAFFIC_VIEWER_ENTRIES_END) + TRAFFIC_VIEWER_ENTRIES_END.Length;
					File.WriteAllText(_hostFilePath, hostFileContents.Substring(0, indexOfTVEntries));
					File.AppendAllText(_hostFilePath, hostFileContents.Substring(indexOfTVEnd));
					HttpServerConsole.Instance.WriteLine(LogMessageType.Information,
						"Successfully removed traffic viewer host file entries");
				}
			}
			else
			{
				HttpServerConsole.Instance.WriteLine(LogMessageType.Error,
					"Host file not found");
			}
		}

		public override void Start()
		{
            if(!Enum.TryParse<TrafficServerMode>(ExtraOptions["MatchMode"], out _matchMode))
            {
                HttpServerConsole.Instance.WriteLine(LogMessageType.Warning,
                    "Could not parse match mode '{0}'. Using 'BrowserFriendly'. Other options are 'Strict' and 'IgnoreCookies'.");
            }
            if (!bool.TryParse(ExtraOptions["IgnoreAuth"], out _ignoreAuth))
            {
                HttpServerConsole.Instance.WriteLine(LogMessageType.Warning,
                    "Could not parse ignore auth setting '{0}'. Using default: 'false'.");
            }

            HttpServerConsole.Instance.WriteLine
                ("Starting Proxy on Host: {0}, Port: {1}, Mode: {2}"
                , Host, _usesDynamicPort ? "[dynamic]" : Port.ToString(), _matchMode);
			
			base.Start();
		}

		public override void Stop()
		{
			base.Stop();

			//remove the traffic viewer entries from the hosts file
			RemoveHosts();

		}

	}
}
