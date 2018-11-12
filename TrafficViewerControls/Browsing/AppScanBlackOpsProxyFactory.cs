using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Testing;
using TrafficServer;
using TrafficViewerControls.DefaultExploiters;
using TrafficViewerControls.Properties;
using TrafficViewerInstance;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace TrafficViewerControls.Browsing
{
    /// <summary>
    /// Makes proxies available in this solution
    /// </summary>
    public class AppScanBlackOpsProxyFactory
    {
        private List<string> _availableTypes = new List<string>();
        /// <summary>
        /// Gets the available types
        /// </summary>
        public List<string> AvailableTypes
        {
            get { return _availableTypes; }
        }

        public AppScanBlackOpsProxyFactory()
        {
            _availableTypes.Add(Resources.CaptureProxy);
            _availableTypes.Add(Resources.DriveByAttackProxy);
            _availableTypes.Add(Resources.SequentialAttackProxy);
            _availableTypes.Add(Resources.ReverseProxy);
            _availableTypes.Add(Resources.TrafficFileProxy);
            _availableTypes.Add(Resources.BinaryReverseProxy);
            _availableTypes.Add(Resources.TrackingProxy);

        }

        public IHttpProxy MakeProxy(string type)
        {
            HttpServerConsole.Instance.WriteLine(LogMessageType.Information, "Created proxy '{0}'", type);
            string host = TrafficViewerOptions.Instance.TrafficServerIp;
            int port = TrafficViewerOptions.Instance.TrafficServerPort;
            int securePort = TrafficViewerOptions.Instance.TrafficServerPortSecure;
            ITrafficDataAccessor dataStore = TrafficViewer.Instance.TrafficViewerFile;
            var exploiter = new CustomTestsExploiter();
            IHttpProxy result = null;

            if (type.Equals(Resources.CaptureProxy)) result = new AdvancedExploreProxy(host, port, dataStore);
            else if (type.Equals(Resources.DriveByAttackProxy)) result = new DriveByAttackProxy(exploiter, dataStore, host, port);
            else if (type.Equals(Resources.SequentialAttackProxy)) result = new SequentialAttackProxy(exploiter, dataStore, host, port);
            else if (type.Equals(Resources.ReverseProxy)) result = new ReverseProxy(host, port, securePort, dataStore);
            else if (type.Equals(Resources.TrafficFileProxy)) result = new TrafficStoreProxy(dataStore, dataStore, host, port, securePort);
            else if (type.Equals(Resources.BinaryReverseProxy)) result = new BinaryReverseProxy(host, port, securePort, dataStore);
            else if (type.Equals(Resources.TrackingProxy)) result = new TrackingReverseProxy(host, port, securePort, dataStore);


            if (result != null)
            {
                if (!(result is TrafficStoreProxy))
                {
                    if (TrafficViewerOptions.Instance.UseProxy)
                    {
                        WebProxy proxy = new WebProxy(TrafficViewerOptions.Instance.HttpProxyServer, TrafficViewerOptions.Instance.HttpProxyPort);
                        result.NetworkSettings.WebProxy = proxy;
                    }
                    result.NetworkSettings.CertificateValidationCallback = new RemoteCertificateValidationCallback(SSLValidationCallback.ValidateRemoteCertificate);
                }


                if (result is BaseAttackProxy)
                {
                    result.ExtraOptions[BaseAttackProxy.TEST_FILE_PATH] = Path.Combine(TrafficViewerOptions.TrafficViewerAppDataDir, "CustomTests.xml");
                }

                if (result is ReverseProxy || result is BinaryReverseProxy)
                {
                    result.ExtraOptions[ReverseProxy.FORWARDING_HOST_OPT] = TrafficViewerOptions.Instance.ForwardingHost;
                    result.ExtraOptions[ReverseProxy.FORWARDING_PORT_OPT] = TrafficViewerOptions.Instance.ForwardingPort.ToString();
                }
            }
            return result;
        }
    }
}
