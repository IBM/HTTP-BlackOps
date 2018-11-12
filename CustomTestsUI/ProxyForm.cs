using CommonControls;
using CustomTestsUI.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Testing;
using TrafficViewerSDK.Http;

namespace CustomTestsUI
{
    public partial class ProxyForm : Form
    {
        private BaseAttackProxy _proxy;
        private bool _isSequentialProxy;
        private ITestRunner _testRunner;
        private INetworkSettings _networkSettings;

        public ProxyForm(ITestRunner testRunner, INetworkSettings netSettings, bool isSequential)
        {
            InitializeComponent();
            _testRunner = testRunner;
            _isSequentialProxy = isSequential;
            _networkSettings = netSettings;
            _proxy = _testRunner.GetTestProxy(_networkSettings, _isSequentialProxy);
            Application.ApplicationExit += OnApplicationExit;

        }

        void OnApplicationExit(object sender, EventArgs e)
        {
            if (_proxy != null) _proxy.Stop();
        }

        private void ProxyForm_Load(object sender, EventArgs e)
        {
            _proxy.Start();
            _timer.Start();
        }

        private void StartClick(object sender, EventArgs e)
        {
           

            if (_proxy.IsListening)
            {
                _proxy.Stop();
            }
            else
            {
                _proxy.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_proxy == null)
            {
                return;
            }

            if (_proxy.IsListening)
            {
                _button.Text = "Pause";
            }
            else
            {
                _button.Text = "Start";
            }

            _labelHostAndPort.Text = String.Format("Proxy is listening on host {0} and port {1}",_proxy.Host, _proxy.Port);
            _labelTestsRemaining.Text = String.Format("Total tests remaining: {0}", _proxy.TestCount);
            _labelCurrentTestRequest.Text = String.Format("Last test request index: {0}", _proxy.CurrentTestReqIdx);
            _labelCurrentRequest.Text = String.Format("Current request index: {0}", _proxy.CurrentReqIdx);
        }

        private void ProxyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_proxy!=null) _proxy.Stop();
        }

       
    }
}
