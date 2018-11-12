using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;
using System.Threading;

namespace TrafficViewerControls.Browsing
{
	public class BrowserClicker
	{
		/// <summary>
		/// Finds a link or a form matching the specified request info and executes a click or
		/// simply a navigate to it
		/// </summary>
		/// <param name="browser"></param>
		/// <param name="reqInfo"></param>
		public static void Click(TrafficPlaybackBrowser playbackBrowser, HttpRequestInfo reqInfo)
		{
			WebBrowser browser = playbackBrowser.Browser;

			//if we have a document try clicking the actual links
			if (browser.Document != null)
			{
				//first try to find a control that matches the request and click it
                if (reqInfo.ContentData==null || reqInfo.ContentData.Length == 0)
				{
					//find links to click
					foreach (HtmlElement link in browser.Document.Links)
					{
						string href = link.GetAttribute("href");

						//if the element contains part pf the request click on it
						if (href.IndexOf(reqInfo.PathAndQuery, StringComparison.OrdinalIgnoreCase) > -1)
						{
							link.Style += "background-color:yellow";
							link.InvokeMember("click");
							return;
						}

					}
				}

				//if the click control was not found try to find forms that have a matching action elemen
				foreach (HtmlElement form in browser.Document.Forms)
				{
					string action = form.GetAttribute("action");
					if (action.IndexOf(reqInfo.Path, StringComparison.OrdinalIgnoreCase) > -1)
					{
						form.Style += "background-color:yellow";
						FillForm(reqInfo, form);
						//submit the form
						form.InvokeMember("submit");
						return;
					}
				}
			}

			//if neither link or form was found simply navigate to the link
			Navigate(playbackBrowser, reqInfo);

		}

		/// <summary>
		/// Fills the specified form with values extracted from the http request
		/// </summary>
		/// <param name="reqInfo"></param>
		/// <param name="form"></param>
		private static void FillForm(HttpRequestInfo reqInfo, HtmlElement form)
		{
			//auto form fill the form
			foreach (HtmlElement formEl in form.All)
			{
				string name = formEl.GetAttribute("name");
				string id = formEl.Id ?? String.Empty;
				string value;

				//try to get the value from the current request
				if (reqInfo.BodyVariables.TryGetValue(name, out value) ||
					reqInfo.QueryVariables.TryGetValue(name, out value) ||
					reqInfo.BodyVariables.TryGetValue(id, out value) ||
					reqInfo.QueryVariables.TryGetValue(id, out value))
				{
					formEl.SetAttribute("value", value);
				}

			}
		}

		/// <summary>
		/// Navigates to a new page
		/// </summary>
		/// <param name="playbackBrowser"></param>
		/// <param name="reqInfo"></param>
		private static void Navigate(TrafficPlaybackBrowser playbackBrowser, HttpRequestInfo reqInfo)
		{
			WebBrowser browser = playbackBrowser.Browser;
			//try sending the next request
			try
			{
				string url = String.Format("http://{0}{1}", playbackBrowser.ProxyHost, reqInfo.PathAndQuery);
				playbackBrowser.AddressBox.Text = url;
				if (reqInfo.ContentLength > 0)
				{
					browser.Navigate(url,
						String.Empty,
						reqInfo.ContentData,
						reqInfo.Headers.ToString());
				}
				else
				{
					browser.Navigate(url,
						String.Empty,
						null,
						reqInfo.Headers.ToString());
				}

			}
			catch { }
		}

	}
}
