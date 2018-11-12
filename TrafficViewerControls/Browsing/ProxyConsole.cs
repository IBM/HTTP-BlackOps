using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;
using System.Threading;
using TrafficServer;
using TrafficViewerSDK.Http;
using TrafficViewerControls.Browsing;
using System.Net;
using TrafficViewerInstance;
using CommonControls;
using TrafficViewerControls.Properties;

namespace TrafficViewerControls.Browsing
{
    /// <summary>
    /// Traffic Server console (Singleton)
    /// </summary>
    public partial class ProxyConsole : UserControl, ITrafficServerConsoleOutput
    {
        private const int MAX_ITEMS = 10000;

        private const int DISPLAY_DELAY = 1;

        private AppScanBlackOpsProxyFactory _proxyFactory = new AppScanBlackOpsProxyFactory();

        private Queue<KeyValuePair<LogMessageType, string>> _messageQueue = new Queue<KeyValuePair<LogMessageType, string>>();

        private Dictionary<string, IHttpProxy> _initializedProxies = new Dictionary<string, IHttpProxy>();

        private Queue<RequestTrapEventEventArgs> _trapQueue = new Queue<RequestTrapEventEventArgs>();

        private object _lock = new object();
        

        public ProxyConsole()
        {
            InitializeComponent();
            _splitContainer.Panel2Collapsed = true;
            _displayTimer.Start();
            
        }

        /// <summary>
        /// Stops all proxies that are running in the console
        /// </summary>
        public void StopAllProxies()
        {
            //configure all the proxies
            foreach (string proxyType in _initializedProxies.Keys)
            {
                var proxy = _initializedProxies[proxyType];
                if(proxy.IsListening)
                {
                    proxy.Stop();
                }
            }
            _buttonStart.Image = Resources.play;
            
        }

        public void WriteLine(LogMessageType type, string message)
        {
            if (message != null)
            {
                _messageQueue.Enqueue(new KeyValuePair<LogMessageType, string>(type, message));
                
            }
        }

        /// <summary>
        /// Updates the statistics
        /// </summary>
        /// <param name="found"></param>
        /// <param name="missed"></param>
        /// <param name="total"></param>
        public void WriteStats(int found, int missed, int total)
        {
            this.Invoke((MethodInvoker)delegate
            {
                _labelStatistics.Text = String.Format(TrafficViewerControls.Properties.Resources.ProxyConsoleStatistics,
                                        found, missed, total);
            });
        }

        private TVConsoleStatus _status;
        /// <summary>
        /// Gets or sets the status of the console
        /// </summary>
        public TVConsoleStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                switch (_status)
                {
                    case TVConsoleStatus.ServerStarted:
                        _buttonStart.Enabled = true;

                        break;
                    case TVConsoleStatus.ServerStopped:
                        _buttonStart.Enabled = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Occurs when the user click the start server button
        /// </summary>
        public event EventHandler StartRequested
        {
            add
            {
                _buttonStart.Click += value;
            }
            remove
            {
                _buttonStart.Click -= value;
            }
        }

        /// <summary>
        /// Occurs when the user clicks the stop button
        /// </summary>
        public event EventHandler StopRequested
        {
            add
            {
                _stopEvent = value;
            }
            remove
            {
                _stopEvent = null;
            }
        }

        private event EventHandler _stopEvent;




        private void ClearClick(object sender, EventArgs e)
        {
            _boxConsole.Rows.Clear();
            _messageQueue.Clear();
        }

        private void DisplayTimerTick(object sender, EventArgs e)
        {
            _displayTimer.Stop();
            if (HttpTrap.Instance.TrapRequests || HttpTrap.Instance.TrapResponses)
            {
                _buttonRelease.Text = String.Format(Resources.TrapRelease, _trapQueue.Count);
            }
            while (_messageQueue.Count > 0)
            {
                if (_boxConsole.Rows.Count == MAX_ITEMS - 1)
                {
                    _boxConsole.Rows.RemoveAt(0);
                }

                KeyValuePair<LogMessageType, string> m = _messageQueue.Dequeue();
                if (String.IsNullOrWhiteSpace(m.Value)) continue; //skip empty rows
                if(m.Key == LogMessageType.Notification)
                {
                    if (_splitContainer.Panel2Collapsed)
                    {
                        _splitContainer.Panel2Collapsed = false;
                    }
                    _notificationLabel.Text = m.Value;
                    continue;
                }

                string value = m.Value.Length > 255 ? m.Value.Substring(0,255) + "..." : m.Value;
                int index = _boxConsole.Rows.Add(value);
                switch (m.Key)
                {
                    case LogMessageType.Error: _boxConsole.Rows[index].DefaultCellStyle.ForeColor = Color.Red; break;
                    case LogMessageType.Warning: _boxConsole.Rows[index].DefaultCellStyle.ForeColor = Color.Orange; break;
                    case LogMessageType.Information: _boxConsole.Rows[index].DefaultCellStyle.ForeColor = Color.Lime; break;
                   
                }


                if (_boxConsole.SelectedRows.Count < 2)
                {
                    bool scroll = false;
                    //check if the last row was selected previously
                    if (_boxConsole.Rows.Count > 1)
                    {
                        int selectedRow;
                        selectedRow = _boxConsole.Rows.GetLastRow(DataGridViewElementStates.Selected);
                        if (selectedRow == _boxConsole.Rows.Count - 2)
                        {
                            //deselect this row
                            _boxConsole.Rows[selectedRow].Selected = false;
                            scroll = true;
                        }
                    }

                    //if the list was empty before automatically select
                    if (_boxConsole.Rows.Count == 1)
                    {
                        scroll = true;
                    }

                    if (scroll)
                    {
                        //select the last visible element in the list
                        int l = _boxConsole.Rows.GetLastRow(DataGridViewElementStates.Visible);
                        _boxConsole.Rows[l].Selected = true;
                        //scroll to it
                        if (l > -1)
                        {
                            _boxConsole.FirstDisplayedScrollingRowIndex = l;
                        }
                    }
                }
            }

            _displayTimer.Start();
        }


        private void ProxyConsoleLoad(object sender, EventArgs e)
		{
            //configure the http trap
            HttpTrap.Instance.TrapDefs.Clear();
            HttpTrap.Instance.TrapDefs = TrafficViewerOptions.Instance.GetTraps();
            HttpTrap.Instance.RequestTrapped += RequestTrapEvent;
            HttpTrap.Instance.ResponseTrapped += ResponseTrapped;

            HttpServerConsole.Instance.Output = this;
         
            
            
		}

        public void ReloadProxies()
        {
            //create all the default proxies
            try
            {
                _initializedProxies.Clear();
                _availableProxies.Items.Clear();
                foreach (string proxyType in _proxyFactory.AvailableTypes)
                {
                    _initializedProxies.Add(proxyType,
                        _proxyFactory.MakeProxy(proxyType));
                    _availableProxies.Items.Add(proxyType);
                }

                foreach (IHttpProxyFactory factory in TrafficViewer.Instance.HttpProxyFactoryList)
                {
                    _initializedProxies.Add(factory.Name,
                        factory.MakeProxyServer(TrafficViewerOptions.Instance.TrafficServerIp,
                        TrafficViewerOptions.Instance.TrafficServerPort,
                        TrafficViewerOptions.Instance.TrafficServerPortSecure,
                        TrafficViewer.Instance.TrafficViewerFile));
                    _availableProxies.Items.Add(factory.Name);
                    HttpServerConsole.Instance.WriteLine(LogMessageType.Information,
                        "Created extension proxy: '{0}'", factory.Name);
                }
                _availableProxies.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                HttpServerConsole.Instance.WriteLine(ex);
            }
        }

        private void QueueTrapEvent(RequestTrapEventEventArgs e)
        {
            lock (_lock)
            {
                _trapQueue.Enqueue(e);
            }
        }

        private void ResponseTrapped(RequestTrapEventEventArgs e)
        {
            HttpServerConsole.Instance.WriteLine(LogMessageType.Information,
                "Response TRAPPED");
            QueueTrapEvent(e);
        }

      

        private void RequestTrapEvent(RequestTrapEventEventArgs e)
        {
            HttpServerConsole.Instance.WriteLine(LogMessageType.Information,
                "Request TRAPPED: '{0}'", e.HttpReqInfo.RequestLine);
            QueueTrapEvent(e);
        }

      
        private void _boxConsole_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void SettingsClick(object sender, EventArgs e)
        {
            
            IHttpProxy proxy = GetCurrentProxy();

            if (proxy != null)
            {
                ProxySetup setupForm = new ProxySetup(proxy);
                setupForm.ShowDialog();
            }


        }

        /// <summary>
        /// Returns the current proxy based on the user selection
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        private IHttpProxy GetCurrentProxy()
        {
            IHttpProxy proxy;
            string currentSelection = _availableProxies.Text;

            //proxies not yet initialized
            if (_initializedProxies.Count == 0 || String.IsNullOrWhiteSpace(currentSelection))
            {
                return null;
            }

            if (_initializedProxies.ContainsKey(currentSelection))
            {
                proxy = _initializedProxies[currentSelection];
            }
            else
            {
                ErrorBox.ShowDialog("Unrecognized proxy");
                proxy = null;
            }
            return proxy;
        }

        private void StartStopClick(object sender, EventArgs e)
        {
            
            IHttpProxy proxy = GetCurrentProxy();

            if (proxy != null)
            {
                if (proxy.IsListening)
                {
                    proxy.Stop();
                }
                else
                {
                    proxy.Start();
                }
                UpdateStartStopButtonStatus(proxy);

            }
        }

        private void AvailableProxiesSelectedIndexChanged(object sender, EventArgs e)
        {
            
            IHttpProxy proxy = GetCurrentProxy();

            if (proxy != null)
            {
                UpdateStartStopButtonStatus(proxy);
            
            }
        }

        private void UpdateStartStopButtonStatus(IHttpProxy proxy)
        {
            if (proxy.IsListening)
            {
                _buttonStart.Image = Resources.pause1;
            }
            else
            {
                _buttonStart.Image = Resources.play;
            }
        }

        private void ReloadProxyClick(object sender, EventArgs e)
        {
            IHttpProxy proxy = GetCurrentProxy();
            if (proxy.IsListening)
            {
                proxy.Stop();
            }
            string currentSelection = _availableProxies.Text;

            if (_proxyFactory.AvailableTypes.Contains(currentSelection))
            {
                proxy = _proxyFactory.MakeProxy(currentSelection);
            }
            else
            {
                foreach (IHttpProxyFactory factory in TrafficViewer.Instance.HttpProxyFactoryList)
                {
                    if(factory.Name.Equals(currentSelection))
                    {
                        HttpServerConsole.Instance.WriteLine(LogMessageType.Information,
                            "Re-creating proxy: '{0}'", factory.Name);
                        proxy = factory.MakeProxyServer(TrafficViewerOptions.Instance.TrafficServerIp,
                        TrafficViewerOptions.Instance.TrafficServerPort,
                        TrafficViewerOptions.Instance.TrafficServerPortSecure,
                        TrafficViewer.Instance.TrafficViewerFile);
                    }
                }
            }
            _initializedProxies[currentSelection] = proxy;
        }

        private void SetupTrapsClick(object sender, EventArgs e)
        {
            TrapsSetup form = new TrapsSetup();
            form.ShowDialog();
        }

        private void RequestTrapCheckedChanged(object sender, EventArgs e)
        {
            HttpTrap.Instance.TrapRequests = _checkTrapReq.Checked;
            
        }

        private void ResponseTrapCheckedChanged(object sender, EventArgs e)
        {
            HttpTrap.Instance.TrapResponses = _checkTrapResp.Checked;
            
        }

        private void ReleaseClick(object sender, EventArgs e)
        {
            lock (_lock)
            {
                if (_trapQueue.Count > 0)
                {
                    var trapLock = _trapQueue.Dequeue();
                    trapLock.ReqLock.Set();
                    _buttonRelease.Text = String.Format(Resources.TrapRelease, _trapQueue.Count);
                }
            }
        }

        private void ReleaseAllClick(object sender, EventArgs e)
        {
            lock (_lock)
            {
                while (_trapQueue.Count > 0)
                {
                    var trapLock = _trapQueue.Dequeue();
                    trapLock.ReqLock.Set();
                }
                _buttonRelease.Text = String.Format(Resources.TrapRelease, _trapQueue.Count);
            }
        }

        private void CloseNotification(object sender, EventArgs e)
        {
            _splitContainer.Panel2Collapsed = true;
        }

     
    }


}