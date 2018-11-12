using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TrafficViewerSDK;
using TrafficViewerSDK.Http;

namespace TrafficViewerControls
{
	public partial class BrowserView : UserControl
	{
		/// <summary>
		/// Used to override the extension of the file displayed in the browser view
		/// </summary>
		protected virtual string ExtensionOverride
		{
			get 
			{
				return String.Empty;
			}
		}

		/// <summary>
		/// Allows classes that inherit this control to process the content, for example html parsing to xml
		/// </summary>
		/// <param name="respInfo"></param>
		/// <returns></returns>
		protected virtual byte[] GetResponseBody(HttpResponseInfo respInfo)
		{
			return respInfo.ResponseBody.ToArray();
		}

		private byte[] _responseBytes;

		private const string SCRIPTING_LANGUAGES_EXTENSION = @"\.(aspx?|asmx|jsp|do|ws|php|js|dll|exe|action)";

		private const string HTM_EXTENSION = ".htm";

		private TempFile _temp;

		/// <summary>
		/// Navigates the browser control to a temporary resource containing the response bytes
		/// </summary>
		/// <param name="requestHeader">The request information</param>
		/// <param name="fullResponse">The full response</param>
		public void Navigate(TVRequestInfo requestHeader, byte[] fullResponse)
		{
			if (requestHeader == null) return;

			HttpResponseInfo respInfo = new HttpResponseInfo();
			respInfo.ProcessResponse(fullResponse);
			byte [] fileContent = GetResponseBody(respInfo);

			if (_responseBytes == fileContent) return; //this response is already displayed

			_responseBytes = fileContent;

			if (_responseBytes != null)
			{
				//extract the file name from the request header
				string reqLine = requestHeader.RequestLine;
				int index = reqLine.IndexOf(' '); //after the first space follows the uri
				reqLine = reqLine.Substring(index + 1);
				string tempFileName = Utils.GetFileNameFromUri(reqLine);
				
				//attach .htm to certain extensions or if the file doesn't have an extension
				if (Utils.IsMatch(tempFileName, SCRIPTING_LANGUAGES_EXTENSION) 
					|| !tempFileName.Contains("."))
				{
					tempFileName += HTM_EXTENSION;
				}
				
				//if the overriding extension is set append it
				tempFileName += ExtensionOverride;

				//we don't care if the file name is empty, the temp file class will handle this case
				try
				{
					//create a temp file
					_temp = new TempFile(tempFileName);
					_temp.Write(_responseBytes);
					_webBrowser.Navigate(_temp.Url);

				}
				catch { }
			}
			else
			{
				_webBrowser.Navigate("about:blank");
			}
		}



		public BrowserView()
		{
			InitializeComponent();
		}
	}
}
