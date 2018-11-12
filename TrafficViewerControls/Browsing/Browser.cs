using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK.Http;


namespace TrafficViewerControls.Browsing
{
	public partial class Browser : Form
	{
		public Browser()
		{
			InitializeComponent();

			
			_webBrowser.Navigated += new WebBrowserNavigatedEventHandler(BrowserNavigated);
			_webBrowser.Navigating += new WebBrowserNavigatingEventHandler(BrowserNavigating);
			_webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(BrowserDocumentCompleted);
			_webBrowser.ProgressChanged += new WebBrowserProgressChangedEventHandler(BrowserProgressChanged);
			
			//we won't be able to load ActiveX if this is on
			_webBrowser.ScriptErrorsSuppressed = false;
			_progressBar.Visible = false;
			_trapRequestsMenu.Checked = HttpTrap.Instance.TrapRequests;
			_trapResponsesMenu.Checked = HttpTrap.Instance.TrapResponses;
		}

		void BrowserProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
		{
			_progressBar.Maximum = (int) e.MaximumProgress;
			if (e.CurrentProgress > 0 && e.CurrentProgress < _progressBar.Maximum)
			{
				_progressBar.Value = (int)e.CurrentProgress;
			}
		}


		private void BrowserNavigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			
			AddressBox.Text = _webBrowser.Url.ToString();
		}
		private void BrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			_progressBar.Visible = true;
			//webBrowser1.Document.Window.Frames[0].Url;
		}
		private void BrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			_progressBar.Visible = false;
			/*
			mshtml.HTMLDocumentClass doc = getDocumentObject(e.Url);
			if ((doc != null) && (doc.parentWindow != null))
			{
				{
					//doc.parentWindow.onerror = new BrowserEventWrapper(delegate { window_onerror(doc); });
					doc.parentWindow.execScript("window.onerror = function() {}");
				}
			}*/
		}

		private void GoClick(object sender, EventArgs e)
		{
			_webBrowser.Navigate(AddressBox.Text);
		}

		private void RefreshClick(object sender, EventArgs e)
		{
			_webBrowser.Refresh();
		}

		private void BackClick(object sender, EventArgs e)
		{
			_webBrowser.GoBack();
		}

		private void ForwardClick(object sender, EventArgs e)
		{
			_webBrowser.GoForward();
		}


		private void AddressBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				GoClick(null, null);
			}
		}

		private void TrapRequestsClick(object sender, EventArgs e)
		{
			_trapRequestsMenu.Checked = !_trapRequestsMenu.Checked;
			HttpTrap.Instance.TrapRequests = _trapRequestsMenu.Checked;
		}

		private void TrapResponseClick(object sender, EventArgs e)
		{
			_trapResponsesMenu.Checked = !_trapResponsesMenu.Checked;
			HttpTrap.Instance.TrapResponses = _trapResponsesMenu.Checked;
		}

	}
}
