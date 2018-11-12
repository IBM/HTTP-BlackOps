using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;
using TrafficServer;
using TrafficViewerControls.Properties;
using TrafficViewerSDK.Http;
using TrafficViewerInstance;
using System.Net;
using System.Net.Security;

namespace TrafficViewerControls.Browsing
{
    public partial class ExternalBrowserListener : Form
    {
		BaseProxy _proxy;

        public ExternalBrowserListener(ITrafficDataAccessor source, BaseProxy proxy = null, bool hideControls=false)
        {

            InitializeComponent();

            if (hideControls)
            {
                _checkTrapReq.Visible = _checkTrapResp.Visible = _labelTrap.Visible = _textTrapMatch.Visible = _checkTrackReqContext.Visible = false;
                this.MinimumSize = new Size(this.Width, this.Height / 2);
                this.Size = this.MaximumSize = this.MinimumSize;
                
            }


            if (proxy != null)
            {
                _proxy = proxy;
            }
            else
            {
                _proxy = new AdvancedExploreProxy(TrafficViewer.Instance.Options.TrafficServerIp, TrafficViewer.Instance.Options.TrafficServerPort, source);
                if (TrafficViewerOptions.Instance.UseProxy)
                {
                    WebProxy webProxy = new WebProxy(TrafficViewerOptions.Instance.HttpProxyServer, TrafficViewerOptions.Instance.HttpProxyPort);
                    _proxy.NetworkSettings.WebProxy = webProxy;
                }

                _proxy.NetworkSettings.CertificateValidationCallback = new RemoteCertificateValidationCallback(SSLValidationCallback.ValidateRemoteCertificate);

            }
			StartProxy();
        }

        private void StartProxy()
        {
            _buttonStart.Enabled = false;
            _proxy.Start();
            _labelMessage.Text = String.Format(Resources.ExternalBrowserTextStarted, _proxy.Host, _proxy.Port);
            _buttonStart.Text = Resources.Stop;
            _buttonStart.Enabled = true;
			_textTrapMatch.Enabled = false;
			
        }


        private void StopProxy()
        {
            _buttonStart.Enabled = false;
            _proxy.Stop();
            _labelMessage.Text = Resources.ExternalBrowserTextStopped;
            _buttonStart.Text = Resources.Start;
            _buttonStart.Enabled = true;
			_textTrapMatch.Enabled = true;
        }

        private void _buttonStart_Click(object sender, EventArgs e)
        {
            if (_proxy.IsListening)
            {
                StopProxy();
				
            }
            else
            {
				HttpTrap.Instance.TrapRequests = _checkTrapReq.Checked;
				HttpTrap.Instance.TrapResponses = _checkTrapResp.Checked;
                StartProxy();
				
            }
        }

        private void _checkTrap_CheckedChanged(object sender, EventArgs e)
        {
            HttpTrap.Instance.TrapRequests = _checkTrapReq.Checked;
        }

        private void ExternalBrowserListener_FormClosing(object sender, FormClosingEventArgs e)
        {
            _proxy.Stop();
        }

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			HttpTrap.Instance.TrapResponses = _checkTrapResp.Checked;
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
            if (_proxy is AdvancedExploreProxy)
            {
                _proxy.ExtraOptions.Add("TrackRequestContext",_checkTrackReqContext.Checked.ToString());
            }
		}

		private void _labelMessage_Click(object sender, EventArgs e)
		{

		}

		private void ExternalBrowserListener_Load(object sender, EventArgs e)
		{
			_checkTrapReq.Checked = HttpTrap.Instance.TrapRequests;
			_checkTrapResp.Checked = HttpTrap.Instance.TrapResponses;
		}
    }
}
