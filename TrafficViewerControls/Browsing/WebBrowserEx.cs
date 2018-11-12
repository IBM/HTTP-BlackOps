using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Reflection;
using MsHtmHstInterop;



namespace TrafficViewerControls.Browsing
{

	public class WebBrowserEx : WebBrowser
	{

		private WebBrowserSiteEx _site;
		private bool _uiHandlerSet;
		private bool _setUIHandler = false;

		public bool SetUIHandler
		{
			get { return _setUIHandler; }
			set { _setUIHandler = value; }
		}

		private object _lock = new object();

		protected class WebBrowserSiteEx : WebBrowserSite, IOleCommandTarget, IDocHostUIHandler
		{

			public WebBrowserSiteEx(WebBrowserEx browser) : base(browser)
			{ 
			
			}

			void IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, ref object pvaIn, ref object pvaOut)
			{
			;
			}

			void IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, ref _tagOLECMD prgCmds, ref _tagOLECMDTEXT pCmdText)
			{
			;
			}

			public void EnableModeless(int fEnable)
			{
				;
			}

			public void FilterDataObject(MsHtmHstInterop.IDataObject pDO, out MsHtmHstInterop.IDataObject ppDORet)
			{
				ppDORet = null;
			}

			public void GetDropTarget(MsHtmHstInterop.IDropTarget pDropTarget, out MsHtmHstInterop.IDropTarget ppDropTarget)
			{
				ppDropTarget = null;
			}

			public void GetExternal(out object ppDispatch)
			{
				ppDispatch = null;
			}

			public void GetHostInfo(ref _DOCHOSTUIINFO pInfo)
			{
				;
			}

			public void GetOptionKeyPath(out string pchKey, uint dw)
			{
				pchKey = null;
			}

			public void HideUI()
			{
				;
			}

			public void OnDocWindowActivate(int fActivate)
			{
				;
			}

			public void OnFrameWindowActivate(int fActivate)
			{
				;
			}

			public void ResizeBorder(ref tagRECT prcBorder, IOleInPlaceUIWindow pUIWindow, int fRameWindow)
			{
				;
			}

			public void ShowContextMenu(uint dwID, ref tagPOINT ppt, object pcmdtReserved, object pdispReserved)
			{
				;
			}

			public void ShowUI(uint dwID, IOleInPlaceActiveObject pActiveObject, IOleCommandTarget pCommandTarget, IOleInPlaceFrame pFrame, IOleInPlaceUIWindow pDoc)
			{
				;
			}

			public void TranslateAccelerator(ref tagMSG lpmsg, ref Guid pguidCmdGroup, uint nCmdID)
			{
				//throw new COMException("Not handled",1);
			}

			public void TranslateUrl(uint dwTranslate, ref ushort pchURLIn, IntPtr ppchURLOut)
			{
				;
			}

			public void UpdateUI()
			{
				;
			}
		}

		/// <summary>
		/// Constructor, sets script error suppressed to false
		/// </summary>
		/// <param name="parent"></param>
		public WebBrowserEx()
		{
			ScriptErrorsSuppressed = false;
			//we suppress script errors in a different way
		}


		
		protected override void OnNavigated(WebBrowserNavigatedEventArgs e)
		{
			if (_setUIHandler && !_uiHandlerSet)
			{
				_uiHandlerSet = TrySetUIHandler();
			}
			base.OnNavigated(e);
		}

		private bool TrySetUIHandler()
		{
			SHDocVw.IWebBrowser2 webBrowser2 = this.ActiveXInstance as SHDocVw.IWebBrowser2;
			if (webBrowser2 == null)
				return false;

			ICustomDoc document = webBrowser2.Document as ICustomDoc;
			if (document == null)
				return false;

			document.SetUIHandler((WebBrowserSiteEx)CreateWebBrowserSiteBase());
			return true;
		}


		protected override WebBrowserSiteBase CreateWebBrowserSiteBase()
		{
			lock (_lock)
			{
				if (_site == null)
				{
					_site = new WebBrowserSiteEx(this);
				}
			}
			return _site;
		}
	

		private const int WM_LMOUSEBUTTON = 0x21;
		private const int WM_MOUSEDOWN = 0x201;
		private const int WM_RBUTTONDOWN = 0x204;
		private const int WM_MBUTTONDOWN = 0x207;


		//fixes the flash bug in the WebBrowser control
		protected override void WndProc(ref Message msg)
		{
			if (msg.Msg == WM_LMOUSEBUTTON || msg.Msg == WM_MOUSEDOWN ||
				msg.Msg == WM_RBUTTONDOWN || msg.Msg == WM_MBUTTONDOWN)
			{
				base.DefWndProc(ref msg);
				return;
			}
			base.WndProc(ref msg);
		}


		[DllImport("wininet.dll", SetLastError = true)]
		private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

		[Obsolete("This method is the old way of setting proxy")]
		/// <summary>
		/// Sets the browser to use a proxy
		/// </summary>
		/// <param name="strProxy">host:port</param>
		public void SetProxySettings(string strProxy)
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
		/// Sets the browser to use a proxy using the AppScan way
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		public void SetProxySettings(string host, int port)
		{
			BrowserUtils.ConfigureProxy(host, port);
		}

	}

	/// <summary>
	/// Information used for proxy settings of the browser
	/// </summary>
	public struct INTERNET_PROXY_INFO
	{

		public int dwAccessType;

		public IntPtr proxy;

		public IntPtr proxyBypass;

	}
}
