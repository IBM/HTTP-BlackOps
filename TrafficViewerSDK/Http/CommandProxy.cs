using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using TrafficViewerSDK.Exporters;
using TrafficViewerSDK.Properties;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Allows users to start Manual Explore proxies remotely
	/// </summary>
	public class CommandProxy : BaseProxy
	{
		private ILogWriter _logWriter;
		
		private string _filesDirectory = null;
		/// <summary>
		/// This is the directory where Manual Explore files will be saved
		/// </summary>
		public string FilesDirectory
		{
			get { return _filesDirectory; }
		}

		private int _maxProxies = 100;
		/// <summary>
		/// The maximum number of proxies
		/// </summary>
		public int MaxProxies
		{
			get { return _maxProxies; }
		}


		private Dictionary<int, ManualExploreProxy> _manualExploreProxies;
		private string[] _exclusions;
		private bool _ignoreCertErrors = false;
        private string _recordingType;

		/// <summary>
		///  Allows users to start Manual Explore proxies remotely
		/// </summary>
		/// <param name="host">The host to listen to</param>
		/// <param name="port">The port to listen to</param>
		/// <param name="filesDirectory">This is the directory where Manual Explore files will be saved</param>
		/// <param name="maxProxies">The maximum number of manual explore proxies</param>
		/// <param name="ignoreCertErrors">Whether to ignore invalid certificates</param>
		/// <param name="recordingType">The type of the recording output</param>
        /// <param name="exclusions">The default file exclusions to use for the recorders</param>
		public CommandProxy(string host, int port, string filesDirectory, int maxProxies, bool ignoreCertErrors, string recordingType, params string [] exclusions) : base(host, port, 0)
		{
			_filesDirectory = Path.GetFullPath(filesDirectory);
			_maxProxies = maxProxies;
			_manualExploreProxies = new Dictionary<int, ManualExploreProxy>();
			_logWriter = TrafficViewerSDK.SdkSettings.Instance.Logger;
			_ignoreCertErrors = ignoreCertErrors;
			_exclusions = exclusions;
            _recordingType = recordingType;
			StringBuilder exclusionsString = new StringBuilder();
			foreach (string exclusion in exclusions)
			{
				exclusionsString.Append("\r\n\t");
				exclusionsString.Append(exclusion);
			}
			string message = String.Format(Resources.ManualExploreServerStarting, host, port, filesDirectory, maxProxies, ignoreCertErrors, exclusionsString);
			_logWriter.Log(TraceLevel.Info, message);
			HttpServerConsole.Instance.WriteLine(LogMessageType.Information, message);
			
		}

		/// <summary>
		/// Attempts to start a manual explore proxy on the specified port. 
		/// </summary>
		/// <param name="port"></param>
		public void StartManualExploreProxy(int port)
		{
			//check if the maximum number of proxies was reached
			if (_manualExploreProxies.Count >= _maxProxies)
			{
				_logWriter.Log(TraceLevel.Error, "Maximum number of proxies ({0}) reached.", _maxProxies);
				throw new HttpProxyException(HttpStatusCode.ServiceUnavailable, "Maximum number of proxies reached", ServiceCode.CommandProxyMaxiumumProxiesReached);
			}

			ManualExploreProxy proxy;
			TrafficViewerFile file = new TrafficViewerFile();
			file.Profile.SetExclusions(_exclusions);
			proxy = new ManualExploreProxy(Host,port,file,true);
			//set certificate policy
			if (_ignoreCertErrors)
			{
				proxy.NetworkSettings.CertificateValidationCallback = new RemoteCertificateValidationCallback(IgnoreCertificateErrs);
			}

			proxy.NetworkSettings.WebProxy = WebRequest.GetSystemWebProxy();

			proxy.Start();
			
			_manualExploreProxies.Add(port, proxy);

		}


		/// <summary>
		/// Stops a manual explore proxy on the specified port and then save the resulting traffic file if specified
		/// </summary>
		/// <param name="port">The corresponding proxy port</param>
		/// <param name="fileName">Optional - The file name to use. If not provided no file will get created.</param>
		public void StopManualExploreProxy(int port, string fileName = null)
		{

			if (!_manualExploreProxies.ContainsKey(port))
			{
				_logWriter.Log(TraceLevel.Error, "Can't stop proxy on port {0}. Not found.", port);
				throw new HttpProxyException(HttpStatusCode.NotFound, "Proxy not found on the specified port", ServiceCode.CommandProxyStopCannotFindPort);
			}

			bool saveFile = !String.IsNullOrWhiteSpace(fileName);
			
			string absolutePath = null;

			if (saveFile) //we need to save a file name after stopping, perform validations on the file name
			{
				if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
				{
					//invalid file name
					_logWriter.Log(TraceLevel.Error, "Invalid characters in file name: {0}", fileName);
					throw new HttpProxyException(HttpStatusCode.BadRequest, "Invalid file name", ServiceCode.CommandProxyStopInvalidFileName);
				}

				//now generate full path and check it
				string fullPath = Path.Combine(_filesDirectory, String.Format("{0}.{1}",fileName,_recordingType));

				absolutePath = Path.GetFullPath(fullPath);
				
				if (!fullPath.StartsWith(_filesDirectory))
				{
					//path traversal attempt
					_logWriter.Log(TraceLevel.Error, "Directory traversal attempted with file name: {0}", fileName);
					throw new HttpProxyException(HttpStatusCode.BadRequest, "Directory traversal", ServiceCode.ProxyInternalError);
				}

				//check if the file already exists
				if (File.Exists(fullPath))
				{
					_logWriter.Log(TraceLevel.Error, "File {0} already exists.", fileName);
					throw new HttpProxyException(HttpStatusCode.Forbidden, "File already exists", ServiceCode.CommandProxyStopFileExists);
				}

			}

			ManualExploreProxy proxy = _manualExploreProxies[port];
			
			try
			{
				proxy.Stop();
				//remove the proxy from the list of existing proxies
				_manualExploreProxies.Remove(port);
			}
			catch (Exception ex)
			{
				_logWriter.Log(TraceLevel.Error, "Internal error trying to stop a proxy: {0}", ex);
				throw new HttpProxyException(HttpStatusCode.InternalServerError, "Internal error trying to stop a proxy", ServiceCode.ProxyInternalError);
			}

			if (!saveFile)
			{
				_logWriter.Log(TraceLevel.Verbose, 
					"No file name was specified. Discarding the data for proxy started on port {0}", port);
				//we are done here
				return;
			}

			//file path is fine
			try
			{
				TrafficViewerFile htd = (TrafficViewerFile)proxy.TrafficDataStore;
                if (_recordingType.Equals(Constants.HTD_STRING))
                {
                    htd.Save(absolutePath);
                }
                else if(_recordingType.Equals(Constants.EXD_STRING))
                {
                    ManualExploreExporter exporter = new ManualExploreExporter();
                    Stream exportStream = new FileStream(absolutePath,FileMode.CreateNew);
                    exporter.Export(htd, exportStream);
                    exportStream.Close();
                }
			}
			catch (Exception ex)
			{
				_logWriter.Log(TraceLevel.Error, "An exception occured saving traffic file: {0}", ex);
				throw new HttpProxyException(HttpStatusCode.InternalServerError, "Cannot save file", ServiceCode.ProxyInternalError);
			}

		}

		/// <summary>
		/// Stops the proxy
		/// </summary>
		public override void Stop()
		{
			base.Stop();
			if (_manualExploreProxies != null)
			{
				//stop all the recording proxies
				foreach (int key in _manualExploreProxies.Keys)
				{
					_manualExploreProxies[key].Stop();
				}
				_manualExploreProxies.Clear();
			}
		}

		/// <summary>
		/// Gets the connection
		/// </summary>
		/// <param name="clientInfo"></param>
		/// <returns></returns>
		protected override IProxyConnection GetConnection(TcpClientInfo clientInfo)
		{
			return new CommandProxyConnection(this, clientInfo.Client);
		}

		/// <summary>
		/// Allways ignore cert errors
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="certificate"></param>
		/// <param name="chain"></param>
		/// <param name="sslPolicyErrors"></param>
		/// <returns></returns>
		private static bool IgnoreCertificateErrs(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}
	}
}