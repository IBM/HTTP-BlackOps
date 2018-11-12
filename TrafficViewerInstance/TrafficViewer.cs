using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Options;
using TrafficViewerSDK.Exporters;
using TrafficViewerSDK.AnalysisModules;
using TrafficViewerSDK.Http;

using System.Threading;
using TrafficViewerSDK.Properties;
using System.IO;
using System.Reflection;
using TrafficViewerSDK.Importers;
using TrafficViewerSDK.Senders;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TrafficViewerSDK;
using TrafficViewerInstance.Properties;
using System.Diagnostics;
using TrafficViewerInstance.Extensions;
using TrafficViewerSDK.Exploiters;

namespace TrafficViewerInstance
{
	public class TrafficViewer
	{
		private IList<ITrafficParser> _trafficParsers;
		private IList<ITrafficExporter> _trafficExporters;
		private IList<IAnalysisModule> _analysisModules;
		private IList<IExploiter> _exploiters;

		
		private IList<ISender> _senders;
        private IList<IHttpClientFactory> _httpClientFactoryList;


        IHttpClientFactory _httpClientFactory;
        /// <summary>
        /// Gets/sets the http clients factory
        /// </summary>
        public IHttpClientFactory HttpClientFactory
        {
            get { return _httpClientFactory; }
            set { _httpClientFactory = value; }
        }



		private TrafficViewerFile _trafficViewerFile;
		private string _currentFileName = String.Empty;
		private IExceptionMessageHandler _exceptionMessageHandler;

		private static object _lock = new object();
		private object _opLock = new object();

		private static TrafficViewer _instance = null;

		private bool _launchedFromAppScan = false;
        private IList<IHttpProxyFactory> _httpProxyFactoryList;
        /// <summary>
        /// Gets the http proxy factory list
        /// </summary>
        public IList<IHttpProxyFactory> HttpProxyFactoryList
        {
            get { return _httpProxyFactoryList; }
        }

		/// <summary>
		/// WHether traffic viewer is running as an AppScan extension
		/// </summary>
		public bool LaunchedFromAppScan
		{
			get { return _launchedFromAppScan; }
			set { _launchedFromAppScan = value; }
		}


		public static TrafficViewer Instance
		{
			get
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = new TrafficViewer();
					}
				}

				return _instance;
			}

		}

		/// <summary>
		/// Allows handling of exception messages
		/// </summary>
		public IExceptionMessageHandler ExceptionMessageHandler
		{
			get { return _exceptionMessageHandler; }
			set { _exceptionMessageHandler = value; }
		}

		private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
		{
			return true;
		}


		private TrafficViewer()
		{
			LoadExtensions();

			//initialize traffic file
			_trafficViewerFile = new TrafficViewerFile();

			//ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);

			
			InitSdkSettings();
		}

		/// <summary>
		/// Initializes SDK settings
		/// </summary>
		public static void InitSdkSettings()
		{
			SdkSettings.Instance.ResponsePatterns = TrafficViewerOptions.Instance.GetResponsePatterns();
			SdkSettings.Instance.SessionIdPatterns = TrafficViewerOptions.Instance.GetSessionIdRegexes();
			SdkSettings.Instance.VariableDefinitions = TrafficViewerOptions.Instance.VariableDefinitions;
			SdkSettings.Instance.MemoryBufferSize = TrafficViewerOptions.Instance.MemoryBufferSize;
			SdkSettings.Instance.HttpRequestTimeout = TrafficViewerOptions.Instance.HttpRequestTimeout;
			SdkSettings.Instance.RequestDelay = TrafficViewerOptions.Instance.RequestDelay;
			SdkSettings.Instance.RequestDelayFilter = TrafficViewerOptions.Instance.RequestDelayFilter;
            SdkSettings.Instance.Logger = new DefaultLogWriter(TrafficViewerOptions.TrafficViewerAppDataDir + "\\TrafficViewerLog.txt", TraceLevel.Verbose);
			
            
            //load proxy cert 
			if(!String.IsNullOrWhiteSpace(TrafficViewerOptions.Instance.ProxyCert))
			{
				AppScanProxyCert.LoadCertFromFile(TrafficViewerOptions.Instance.ProxyCert, TrafficViewerOptions.Instance.ProxyCertPass);
			}

            //setup dynamic certificate generation
            if (String.IsNullOrWhiteSpace(TrafficViewerOptions.Instance.CACertPrivKey))
            {
                GenerateBlackopsCA();
            }
          
            SdkSettings.Instance.CAPrivateKey = Encryptor.DecryptToString(TrafficViewerOptions.Instance.CACertPrivKey);
            

		}

        /// <summary>
        /// Generates the CA and saves it into options
        /// </summary>
        public static void GenerateBlackopsCA()
        {
           
            string key, base64Cer;
            CertificateAuthority.GenerateCACert(out key, out base64Cer);
            string encKey = Encryptor.EncryptToString(key);
            TrafficViewerOptions.Instance.CACertPrivKey = encKey;
            TrafficViewerOptions.Instance.CACertCer = base64Cer;
            TrafficViewerOptions.Instance.Save();
        }

		/// <summary>
		/// Loads the Traffic Viewer extensions
		/// </summary>
		public void LoadExtensions()
		{
			SdkSettings.Instance.ExtensionInfoList = TrafficViewerOptions.Instance.GetExtensionInfo();

			//load modules
			TrafficParserFactory parserFactory = new TrafficParserFactory();
			_trafficParsers = parserFactory.GetExtensions();
			ExporterFactory exporterFactory = new ExporterFactory();
			_trafficExporters = exporterFactory.GetExtensions();
			AnalysisModulesFactory analysisModulesFactory = new AnalysisModulesFactory();
			_analysisModules = analysisModulesFactory.GetExtensions();
			ExploiterFactory exploitersFactory = new ExploiterFactory();
			_exploiters = exploitersFactory.GetExtensions();
			
			SenderFactory senderFactory = new SenderFactory();
			_senders = senderFactory.GetExtensions();
            HttpClientExtensionFactory clientFactoryLoader = new HttpClientExtensionFactory();
            _httpClientFactoryList = clientFactoryLoader.GetExtensions();
			_httpClientFactoryList.Add(new WebRequestClientFactory());
            _httpClientFactory = null;
            //find in the list of client factories the one with the name specified in options
            foreach (IHttpClientFactory clientFactory in _httpClientFactoryList)
            {
                if (String.Compare(clientFactory.ClientType, TrafficViewerOptions.Instance.HttpClientName) == 0)
                {
                    _httpClientFactory = clientFactory;
                    break;
                }
            }

            if (_httpClientFactory == null)
            {
                //if the factory was not found use the default 
                _httpClientFactory = new TrafficViewerHttpClientFactory();
                //log a warning
                SdkSettings.Instance.Logger.Log(TraceLevel.Warning, "Could not find HTTP client factory for {0}", TrafficViewerOptions.Instance.HttpClientName);
                TrafficViewerOptions.Instance.HttpClientName = _httpClientFactory.ClientType;
            }

            HttpProxyFactory proxyFactory = new HttpProxyFactory();
            _httpProxyFactoryList = proxyFactory.GetExtensions();
            
		}

        /// <summary>
        /// Makes an http client to be used with a request
        /// </summary>
        /// <returns></returns>
        public IHttpClient MakeHttpClient()
        {
            IHttpClient client = null;
            client = _httpClientFactory.MakeClient();
			DefaultNetworkSettings netSettings = new DefaultNetworkSettings();

			netSettings.CertificateValidationCallback = ValidateServerCertificate;
            if (TrafficViewerOptions.Instance.UseProxy)
            {
				
				netSettings.WebProxy = new WebProxy(TrafficViewerOptions.Instance.HttpProxyServer, TrafficViewerOptions.Instance.HttpProxyPort);
                
            }
			client.SetNetworkSettings(netSettings);
            return client;
        }


		public string Version
		{
			get { return TrafficViewerConstants.DLL_VERSION; }
		}

		/// <summary>
		/// Gets the list of available traffic parsers
		/// </summary>
		public IList<ITrafficParser> TrafficParsers
		{
			get { return _trafficParsers; }
		}

		/// <summary>
		/// Returns the specified parser
		/// </summary>
		/// <param name="parserName"></param>
		/// <returns></returns>
		public ITrafficParser GetParser(string parserName)
		{
			foreach (ITrafficParser parser in _trafficParsers)
			{
				if (String.Compare(parserName, parser.Name, true) == 0)
				{
					return parser;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the list of available Traffic Exporters
		/// </summary>
		public IList<ITrafficExporter> TrafficExporters
		{
			get { return _trafficExporters; }
		}
		/// <summary>
		/// Gets the list of available analysis modules
		/// </summary>
		public IList<IAnalysisModule> AnalysisModules
		{
			get { return _analysisModules; }
		}

		/// <summary>
		/// Gets the list of exploiters
		/// </summary>
		public IList<IExploiter> Exploiters
		{
			get { return _exploiters; }
		}

		/// <summary>
		/// Gets the available sender extensions
		/// </summary>
		public IList<ISender> Senders
		{
			get { return _senders; }
		}
        /// <summary>
        /// Gets the list of Http client factories
        /// </summary>
        public IList<IHttpClientFactory> HttpClientFactoryList
        {
            get { return _httpClientFactoryList; }
        }
		public TrafficViewerFile TrafficViewerFile
		{
			get { return _trafficViewerFile; }
		}

		/// <summary>
		/// Gets the current options
		/// </summary>
		public TrafficViewerOptions Options
		{
			get { return TrafficViewerOptions.Instance; }
		}



		public string CurrentFileName
		{
			get { return _currentFileName; }
		}

		public bool Tail
		{
			get
			{
				return _trafficViewerFile.Tail;
			}
			set
			{
				_trafficViewerFile.Tail = value;
			}
		}


		public void NewTvf()
		{
			if (_trafficViewerFile != null)
			{
				//close the existing file
				_trafficViewerFile.Close(false);
			}
			_trafficViewerFile = new TrafficViewerFile();
			_trafficViewerFile.Profile = Options.GetDefaultProfile();
			_trafficViewerFile.SaveUnpacked();
		}

		public void BeginOpenTvf(string trafficViewerFilePath, AsyncCallback callback)
		{
			TVAsyncOperation openThread = new TVAsyncOperation(OpenTvf, _exceptionMessageHandler, callback, callback);
			openThread.Start(trafficViewerFilePath);
		}

		private void OpenTvf(object param)
		{
			string path = (string)param;
           

			_trafficViewerFile.Open(path);
		}

		public void BeginOpenUnpackedTvf(string tempFolder, AsyncCallback callback)
		{
			TVAsyncOperation openThread = new TVAsyncOperation(OpenUnpackedTvf, _exceptionMessageHandler, callback, callback);
			openThread.Start(tempFolder);
		}

		private void OpenUnpackedTvf(object param)
		{
			string path = (string)param;
			_trafficViewerFile.OpenUnpacked(path);

		}
		public void BeginSaveTvf(string trafficViewerFilePath, AsyncCallback callback)
		{
			TVAsyncOperation openThread = new TVAsyncOperation(SaveTvf, _exceptionMessageHandler, callback, callback);
			openThread.Start(trafficViewerFilePath);
		}

		private void SaveTvf(object param)
		{
			string path = (string)param;
			_trafficViewerFile.EnableDefrag = true;
			_trafficViewerFile.Save(path);
		}

		/// <summary>
		/// Starts an asyncroneus import operation
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="pathsToFilesToImport"></param>
		/// <param name="options"></param>
		/// <param name="callback"></param>
		public void BeginImport(ITrafficParser parser, List<string> pathsToFilesToImport, ParsingOptions options, AsyncCallback callback)
		{
			ImportInfo info = new ImportInfo();
			info.Parser = parser;
			info.TargetFiles = pathsToFilesToImport;
			info.Profile = options;

			BeginImport(info, callback);
		}

		/// <summary>
		/// Starts an asyncroneus import operation
		/// </summary>
		/// <param name="parser"></param>
		/// <param name="sender"></param>
		/// <param name="options"></param>
		/// <param name="callback"></param>
		public void BeginImport(ITrafficParser parser, object sender, ParsingOptions options, AsyncCallback callback)
		{
			ImportInfo info = new ImportInfo();
			info.Parser = parser;
			info.Sender = sender;
			info.Profile = options;

			BeginImport(info, callback);
		}


		/// <summary>
		/// Starts an asyncroneus import operation
		/// </summary>
		/// <param name="info"></param>
		/// <param name="callback"></param>
		public void BeginImport(ImportInfo info, AsyncCallback callback)
		{
			TVAsyncOperation openThread = new TVAsyncOperation(Import, _exceptionMessageHandler, callback, callback);
			openThread.Start(info);
		}

		private void Import(object param)
		{
			ImportInfo info = param as ImportInfo;

			if (info.TargetFiles != null && info.TargetFiles.Count > 0)
			{
				foreach (string path in info.TargetFiles)
				{
					try
					{
						_trafficViewerFile.StartImport(info.Parser, path, info.Profile);
					}
					catch (Exception ex)
					{
						string error = String.Format(Resources.ImportException, path, info.Parser.Name);
                        SdkSettings.Instance.Logger.Log(TraceLevel.Error, "{0} exception thrown: {1}", error, ex.Message);
						_trafficViewerFile.StopImport();
						if (_exceptionMessageHandler != null)
						{
							_exceptionMessageHandler.Show(error);
						}
					}
				}
			}
			else if (info.Sender != null)
			{
				try
				{
					_trafficViewerFile.StartImport(info.Parser, info.Sender, info.Profile);
				}
				catch (Exception ex)
				{
					string error = String.Format(Resources.ImportException, info.Sender.ToString(), info.Parser.Name);
                    SdkSettings.Instance.Logger.Log(TraceLevel.Error, "{0}, exception thrown: {1}", error, ex.Message);
					_trafficViewerFile.StopImport();
					if (_exceptionMessageHandler != null)
					{
						_exceptionMessageHandler.Show(error);
					}
				}
			}

		}



		public void CancelImport()
		{
			_trafficViewerFile.StopImport();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="exporter"></param>
		/// <param name="stream"></param>
		/// <param name="isSSL"></param>
		/// <param name="newHost"></param>
		/// <param name="newPort"></param>
		/// <param name="callback"></param>
		public void BeginExport(ITrafficExporter exporter, Stream stream, string newHost, int newPort, AsyncCallback callback)
		{
			TVAsyncOperation thread = new TVAsyncOperation(Export, _exceptionMessageHandler, callback, callback);
			thread.Start(new object[4] { exporter, stream, newHost, newPort });
		}

		private void Export(object param)
		{
			object[] paramArray = (object[])param;
			ITrafficExporter exporter = (ITrafficExporter)paramArray[0];
			Stream stream = (Stream)paramArray[1];
			string newHost = (string)paramArray[2];
			int newPort = (int)paramArray[3];

			exporter.Export(_trafficViewerFile, stream, newHost, newPort);

		}

		public void BeginClear(bool shouldClearSource, AsyncCallback callback)
		{
			TVAsyncOperation openThread = new TVAsyncOperation(Clear, _exceptionMessageHandler, callback, callback);
			openThread.Start(shouldClearSource);
		}

		private void Clear(object param)
		{
			bool shouldClearSource = (bool)param;

			bool tail = Tail;
			if (tail)
			{
				Tail = false;
			}

			_trafficViewerFile.Clear(shouldClearSource);


			if (tail)
			{
				TrafficViewer.Instance.Tail = true;
			}

		}

		public void CloseTvf(bool keepTempData)
		{
			_trafficViewerFile.Close(keepTempData);
            
		}

        

	}
}

