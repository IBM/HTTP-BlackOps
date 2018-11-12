using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK;
using TrafficViewerControls.Properties;
using System.Security.Cryptography.X509Certificates;
using TrafficServer;
using TrafficViewerSDK.Http;
using TrafficViewerInstance;
using System.Net;
using System.Net.Security;

namespace TrafficViewerControls.Browsing
{
	class ManualExploreBrowser : Browser
	{

		private AdvancedExploreProxy _proxy;

		public ManualExploreBrowser(ITrafficDataAccessor source, string url)//:base(source)
		{
			//Start the internal proxy
			_proxy = new AdvancedExploreProxy(TrafficViewer.Instance.Options.TrafficServerIp, TrafficViewer.Instance.Options.TrafficServerPort, source);

			if (TrafficViewerOptions.Instance.UseProxy)
			{
				WebProxy proxy = new WebProxy(TrafficViewerOptions.Instance.HttpProxyServer, TrafficViewerOptions.Instance.HttpProxyPort);
				proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
				_proxy.NetworkSettings.WebProxy = proxy;
			}

			_proxy.NetworkSettings.CertificateValidationCallback = new RemoteCertificateValidationCallback(SSLValidationCallback.ValidateRemoteCertificate);

			_proxy.Start();

			//add logic to stop the proxy on closing
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ManualExploreBrowser_FormClosing);

			//set the browser to use the internal proxy
			WebBrowser.SetProxySettings(_proxy.Host,_proxy.Port);


			//enable the request trap menu
			//TrapMenu.Visible = true;

			Text = Resources.ManualExploreBrowser;
			
			//CheckTrap.Visible = true;
			//CheckTrap.Checked = RequestTrap.Instance.Enabled;
			//CheckTrap.CheckedChanged += new EventHandler(CheckTrap_CheckedChanged);

			//Slider.Visible = false;//hide the progress slider and the buttons
			//ButtonPause.Visible = ButtonPlay.Visible = ButtonStop.Visible = false;
			//this.Text = Resources.ManualExploreBrowser;
			
			////start the proxy
			//PlayClick(null, null);
			////navigate 
			//if (url != null)
			//{
			//    WebBrowser.Navigate(url);
			//}
		}

		void ManualExploreBrowser_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			_proxy.Stop();
			//WebBrowser.SetProxySettings1("");
		}

        void CheckTrap_CheckedChanged(object sender, EventArgs e)
        {
			//RequestTrap.Instance.Enabled = CheckTrap.Checked;
        }

		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManualExploreBrowser));
            this.SuspendLayout();
            // 
            // ManualExploreBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(896, 520);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ManualExploreBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manual Explore Browser";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}
