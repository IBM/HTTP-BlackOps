using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using TrafficServer;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;
using System.Text.RegularExpressions;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using TrafficViewerSDK.Options;
using System.IO;
using TrafficViewerControls.Properties;
using TrafficViewerInstance;
using System.Diagnostics;

namespace TrafficViewerControls.Browsing
{
	/// <summary>
	/// A browser that shows the user the product interaction with the site
	/// </summary>
	public partial class TrafficPlaybackBrowser : Form
	{
		#region Fields & Constants

		protected TrafficStoreProxy Proxy;
		protected ITrafficDataAccessor Source;

		private int _currId = -1;
		private string KNOWN_RESOURCE_EXTENSIONS = @"\.(js|css|gif|jpe?g|axd|swf|png|jar)";
		private const string SCRIPTING_LANGUAGES_EXTENSION = @"\.(aspx?|asmx|jsp|do|ws|php|js|dll|exe|action)";
		private const int INTERVAL_UNIT = 300; //used to calculate the request interval
		private int _requestInterval = 3 * INTERVAL_UNIT; //the amount of time in ms between "clicks"

		private int _lastRequestHash = 0;//the hash of the last request visited
		private bool _certIsInstalled = false;

		

		#endregion

		#region Properties

		/// <summary>
		/// Gets the browser object
		/// </summary>
		public WebBrowser Browser
		{
			get
			{
				return WebBrowser;
			}
		}

		/// <summary>
		/// Gets the address box
		/// </summary>
		public TextBox AddressBox
		{
			get
			{
				return _textAddress;
			}
		}

		/// <summary>
		/// Gets the internal proxy address
		/// </summary>
		public string ProxyHost
		{
			get
			{
				return String.Format("{0}:{1}", Proxy.Host, Proxy.Port);
			}
		}

		#endregion

		#region Private Methods - Control Event Handlers



		private void GoClick(object sender, EventArgs e)
		{
			try
			{
				WebBrowser.Navigate(_textAddress.Text);
			}
			catch { }
		}

		protected void PlayClick(object sender, EventArgs e)
		{
			ButtonPlay.Enabled = false;
			ButtonPause.Enabled = true;
			ButtonStop.Enabled = true;

			if (Proxy == null)
			{
				Proxy = new TrafficStoreProxy(Source);
			}

			if (!Proxy.IsListening)
			{
				Proxy.Start();
				SetIEProxySettings(String.Format("{0}:{1}", Proxy.Host, Proxy.Port));
			}

			OnPlayClick();
		}

		/// <summary>
		/// Add custom actions for when the play button is pressed
		/// </summary>
		protected virtual void OnPlayClick()
		{
			_playbackTicker.Start();
		}

		private void StopClick(object sender, EventArgs e)
		{
			ButtonPlay.Enabled = true;
			ButtonPause.Enabled = false;
			ButtonStop.Enabled = false;

			_currId = -1;

			SetIEProxySettings("");

			OnStopClick();
		}

		/// <summary>
		/// Add custom actions for when the stop button is pressed
		/// </summary>
		protected virtual void OnStopClick()
		{
			_playbackTicker.Stop();
		}

		private void PauseClick(object sender, EventArgs e)
		{
			ButtonPlay.Enabled = true;
			ButtonPause.Enabled = false;
			ButtonStop.Enabled = true;

			OnPauseClick();

		}

		/// <summary>
		/// Add custom actions for when the pause button is pressed
		/// </summary>
		protected virtual void OnPauseClick()
		{
			_playbackTicker.Stop();
		}

		private void TrafficPlaybackFormFormClosing(object sender, FormClosingEventArgs e)
		{
			Proxy.Stop();
			//re-enable the certificate mismatch warning.
			Utils.EnableCertNameMismatchWarning();

			if (_certIsInstalled)
			{
				UnInstallCert();
			}

			OnClose();
		}

		/// <summary>
		/// Add custom actions for when the pause button is pressed
		/// </summary>
		protected virtual void OnClose()
		{
			;
		}


		private void TrafficPlaybackBrowserOnLoad(object sender, EventArgs e)
		{
			OnLoad();
		}

		/// <summary>
		/// Add custom actions for when the form is loaded
		/// </summary>
		protected virtual void OnLoad()
		{
			PlayClick(null, null);
		}



		private void WebBrowserNavigate(object sender, WebBrowserNavigatingEventArgs e)
		{
			//if this is an https
			if (!_certIsInstalled && e.Url.Scheme == "https")
			{
				InstallCert();
			}
			
		}

		private void WebBrowserNavigated(object sender, WebBrowserNavigatingEventArgs e)
		{
			

		}

		private void SliderScroll(object sender, EventArgs e)
		{
			_requestInterval = (Slider.Value + 1) * INTERVAL_UNIT;
		}
		#endregion

		#region Private Methods - Navigation

		[DllImport("wininet.dll", SetLastError = true)]
		private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

		/// <summary>
		/// Sets the playback browser to use the internal proxy
		/// </summary>
		/// <param name="strProxy"></param>
		private void SetIEProxySettings(string strProxy)
		{
			const int INTERNET_OPTION_PROXY = 38;
			const int INTERNET_OPEN_TYPE_PROXY = 3;

			INTERNET_PROXY_INFO struct_IPI;

			// Filling in structure

			struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
			struct_IPI.proxy = Marshal.StringToHGlobalAnsi(strProxy);
			struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("");

			// Allocating memory
			IntPtr intptrStruct = Marshal.AllocCoTaskMem(Marshal.SizeOf(struct_IPI));

			// Converting structure to IntPtr
			Marshal.StructureToPtr(struct_IPI, intptrStruct, true);

			bool iReturn = InternetSetOption(IntPtr.Zero,
				INTERNET_OPTION_PROXY, intptrStruct, Marshal.SizeOf(struct_IPI));


		}

		/// <summary>
		/// Occurs when the page was loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			_playbackTicker.Interval = _requestInterval;
		}

		/// <summary>
		/// Gets the next unsent request
		/// </summary>
		/// <returns>Http Request Info</returns>
		private HttpRequestInfo GetNextRequest()
		{
			TVRequestInfo tvReqInfo = null;
			HttpRequestInfo httpReqInfo = null;
			ICacheable cacheEntry = null;

			//get the next unvisited request
			bool isCached = true;
			int hash = 0;
			do
			{
				tvReqInfo = Source.GetNext(ref _currId);
				if (tvReqInfo != null)
				{
					isCached = true;
					//skip js files and images
					if (!Utils.IsMatch(tvReqInfo.RequestLine, KNOWN_RESOURCE_EXTENSIONS))
					{
						byte[] requestBytes = Source.LoadRequestData(tvReqInfo.Id);
						httpReqInfo = new HttpRequestInfo(requestBytes);
						//skip invalid requests
						if (httpReqInfo.Path.Contains("/"))
						{
							hash = httpReqInfo.GetHashCode();

							//check the cache for the request to see if it was already visited
							//allow the same request to be sent more than once
							if (hash != _lastRequestHash)
							{
								cacheEntry = TrafficServerCache.Instance.GetEntry(hash);
								isCached = cacheEntry != null;
							}
						}
					}

				}
				//keep doing this until the end of requests or until we found a request that is not cached
			}
			while (tvReqInfo != null && isCached);

			if (hash != _lastRequestHash)
			{
				_lastRequestHash = hash;
				//clear the cache so we don't skip any repeats of the previous request
				TrafficServerCache.Instance.Clear();
			}
			return httpReqInfo;
		}

		/// <summary>
		/// The ticker that loads the next request
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PlaybackTickerTick(object sender, EventArgs e)
		{
			//increase the interval to allow the page to load
			_playbackTicker.Interval = 10 * _requestInterval;

			HttpRequestInfo nextRequest = GetNextRequest();

			if (nextRequest == null)
			{
				//stop
				StopClick(sender, e);
			}
			else
			{
				BrowserClicker.Click(this, nextRequest);
			}

		}


		private void InstallCert()
		{

			//add the certificate to the trusted store
			X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
			store.Open(OpenFlags.ReadWrite);
			//check if the cert is there
			if (store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, AppScanProxyCert.Cert.Subject, false).Count == 0)
			{
				//add the cert
				store.Add(AppScanProxyCert.Cert);
			}
			store.Close();

			_certIsInstalled = true;
		}


		private void UnInstallCert()
		{

			//remove the certificate to the trusted store
			X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
			store.Open(OpenFlags.ReadWrite);
			//check if the cert is there
			if (store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, AppScanProxyCert.Cert.Subject, false).Count > 0)
			{
				//add the cert
				store.Remove(AppScanProxyCert.Cert);
			}
			store.Close();
			_certIsInstalled = false;
		}


		#endregion

		/// <summary>
		/// This is the traffic playback browser
		/// </summary>
		/// <param name="source"></param>
		public TrafficPlaybackBrowser(ITrafficDataAccessor source)
		{
			//disable the certificate name mismatch warning 
			Utils.DisableCertNameMismatchWarning();

			InitializeComponent();

			Source = source;
			_playbackTicker.Interval = _requestInterval;
			WebBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(DocumentCompleted);
			WebBrowser.Navigating += new WebBrowserNavigatingEventHandler(WebBrowserNavigate);
			
			WebBrowser.NewWindow += new CancelEventHandler(OnNewWindow);


			/*_axBrowser = (SHDocVw.WebBrowser)WebBrowser.ActiveXInstance;

			_axBrowser.NewWindow3 += new SHDocVw.DWebBrowserEvents2_NewWindow3EventHandler(OnNewWindow3);
			_axBrowser.NavigateError += new SHDocVw.DWebBrowserEvents2_NavigateErrorEventHandler(OnNavigateError);*/

		}

		private void OnNavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
		{
			try
			{
				Cancel = true;
                int errorCode = (int)StatusCode;
                WebBrowser.DocumentText = String.Format("<h1>{0}:{1}</h1>", Resources.ConnectionError, errorCode);
                //WebRequest request = WebRequest.Create((string)URL);
                //
                //request.Proxy = new WebProxy(Proxy.Host, Proxy.Port);
                //string hostAndPort;

                //if (request.RequestUri.Port == 0 || request.RequestUri.IsDefaultPort)
                //{
                //    hostAndPort = request.RequestUri.Host;
                //}
                //else
                //{
                //    hostAndPort = String.Format("{0}:{1}", request.RequestUri.Host, request.RequestUri.Port);
                //}

                //int hostAndPortHash = hostAndPort.GetHashCode();


                //if (errorCode == 401)
                //{
                    ////try to obtain an entry from the authentication manager
                    //ICacheable entry = HttpAuthenticationManager.Instance.GetEntry(hostAndPortHash);
                    //HttpAuthenticationInfo authInfo = null;

                    //string domain = null;
                    //string userName = null;
                    //string password = null;

                    //if (entry == null)
                    //{
                    //    authInfo = new HttpAuthenticationInfo();


                    //    CredentialsForm form = new CredentialsForm();
                    //    if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    //    {
                    //        string user = form.UserName;
                    //        string[] userParts = user.Split('\\');
                    //        if (userParts.Length == 2)
                    //        {
                    //            domain = userParts[0];
                    //            userName = userParts[1];
                    //        }
                    //        else
                    //        {
                    //            userName = user;
                    //        }
                    //        password = form.Password;
                    //    }


                    //    if (authInfo != null && !String.IsNullOrEmpty(userName)
                    //        && !String.IsNullOrEmpty(password))
                    //    {
                    //        if (domain == null)
                    //        {
                    //            request.Credentials = new NetworkCredential(userName, password);
                    //        }
                    //        else
                    //        {
                    //            request.Credentials = new NetworkCredential(userName, password, domain);
                    //        }

                    //        authInfo.Credentials = request.Credentials;
                    //        //add the entry to the authorization manager
                    //        HttpAuthenticationManager.Instance.Add(hostAndPortHash, new CacheEntry(authInfo));
                    //    }
                    //}
                    //else
                    //{ 
                    //    //get the cached credentials
                    //    authInfo = entry.GetClone() as HttpAuthenticationInfo;
                    //    if (authInfo != null)
                    //    {
                    //        request.Credentials = authInfo.Credentials;
                    //    }
                    //}


                    //if (request.Credentials != null)
                    //{
                    //    //navigate again to the url
                    //    if (_authRedirectCount < 3)
                    //    {
                    //        _authRedirectCount++;
                    //        WebBrowser.Navigate((string)URL);
                    //    }
                    //    else
                    //    {
                    //        _authRedirectCount = 0;
                    //    }
                    //}
                //    WebBrowser.DocumentText = String.Format("<h1>{0}</h1>", Resources.ConnectionError);
                //}
                //else
                //{
					
                //}


			}
			catch (Exception ex)
			{
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Error sending web request: {0}", ex.Message);
			}
		}

		private void WebRequestWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			string text = e.Result as string;
			WebBrowser.DocumentText = text;

		}

		private void WebRequestWork(object sender, DoWorkEventArgs e)
		{

			WebRequest request = e.Argument as WebRequest;
			WebResponse response = null;

			string text;
			
			try
			{
				response = request.GetResponse();
				// Get the stream associated with the response.
				Stream receiveStream = response.GetResponseStream();

				// Pipes the stream to a higher level stream reader with the required encoding format. 
				StreamReader readStream = new StreamReader(receiveStream, Constants.DefaultEncoding);
				text = readStream.ReadToEnd();
			}
			catch 
			{
				text = Resources.ConnectionError;	
			}
			
			e.Result = text;
		}


		protected virtual void OnNewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
		{
			;
		}

		void OnNewWindow(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
		}



		private void AddressKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				GoClick(null, null);
			}
		}

		private void BackClick(object sender, EventArgs e)
		{
			WebBrowser.GoBack();
		}

		private void FwdClick(object sender, EventArgs e)
		{
			WebBrowser.GoForward();
		}

	}

	///// <summary>
	///// Information used for proxy settings of the browser
	///// </summary>
	//public struct INTERNET_PROXY_INFO
	//{

	//    public int dwAccessType;

	//    public IntPtr proxy;

	//    public IntPtr proxyBypass;

	//};
}