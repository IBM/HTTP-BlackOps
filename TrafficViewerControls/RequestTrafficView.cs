
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using TrafficViewerControls.TextBoxes;
using TrafficViewerSDK.Http;

namespace TrafficViewerControls
{
	/// <summary>
	/// Control representing the GUI Request/Response view
	/// </summary>
	public partial class RequestTrafficView : UserControl
	{
		private Color HIGHLIGHT_COLOR = Color.Yellow;
		
		private void SaveChangesClick(object sender, EventArgs e)
		{
			if (SaveRequested != null)
			{ 
				SaveRequested.Invoke(new RequestTrafficViewSaveArgs(_requestBox.Text,_responseBox.Text));
			}
		}

		/// <summary>
		/// Event that occurs when the save was requested
		/// </summary>
		public event RequestTrafficViewSaveEvent SaveRequested;
		

		/// <summary>
		/// Gets and sets the text of the request box
		/// </summary>
		public string RequestText
		{
			get
			{
				return _requestBox.Text;
			}
			set
			{
				_requestBox.Text = value;
			}
		}

		/// <summary>
		/// Gets and sets the text of the response box
		/// </summary>
		public string ResponseText
		{
			get
			{
				return _responseBox.Text;
			}
			set
			{
				_responseBox.Text = value;
			}
		}

		/// <summary>
		/// Sets highlighting during on click. Used for search
		/// </summary>
		/// <param name="text">The text to highlight</param>
		/// <param name="isRegex">If is a regex</param>
		/// <param name="line">The line the text is on</param>
		public void SetHighlighting(string text, bool isRegex, string line)
		{
			_requestBox.AutoSearchCriteria = new
				TrafficViewerSDK.Search.SearchCriteria(isRegex, text);
			_requestBox.LineMatch = line;
            
			_responseBox.AutoSearchCriteria = new
				TrafficViewerSDK.Search.SearchCriteria(isRegex, text);
			_responseBox.LineMatch = line;
		}

		/// <summary>
		/// Sets highlighting during on click. Used for search
		/// </summary>
		/// <param name="text">The text to highlight</param>
		/// <param name="isRegex">If is a regex</param>
		public void SetHighlighting(string text, bool isRegex)
		{
			SetHighlighting(text, isRegex, String.Empty);
		}


		/// <summary>
		/// Ctor
		/// </summary>
		public RequestTrafficView()
		{
			InitializeComponent();
			//add a menu item to save the current traffic
			ToolStripMenuItem save = new ToolStripMenuItem();
			save.Name = "save";
			save.Text = TrafficViewerControls.Properties.Resources.RequestViewSaveChangesMenu;
			save.ShortcutKeys = Keys.F2;
			save.Click+=new EventHandler(SaveChangesClick);
			ToolStripSeparator separator = new ToolStripSeparator();
			
			_requestBox.ContextMenuStrip.Items.Insert(0, separator);
			_requestBox.ContextMenuStrip.Items.Insert(0, save);

			save = new ToolStripMenuItem();
			save.Name = "save";
			save.Text = TrafficViewerControls.Properties.Resources.RequestViewSaveChangesMenu;
			save.ShortcutKeys = Keys.F2;
			save.Click += new EventHandler(SaveChangesClick);
			separator = new ToolStripSeparator();

			_responseBox.ContextMenuStrip.Items.Insert(0, separator);
			_responseBox.ContextMenuStrip.Items.Insert(0, save);

			//attach code to events for the send request
			_requestBox.RequestResent += new EventHandler(RequestResent);
			_requestBox.RequestCompleted += new HttpClientRequestCompleteEvent(RequestCompleted);

		}

		void RequestCompleted(HttpClientRequestCompleteEventArgs e)
		{
			Invoke((MethodInvoker)delegate
			{
				if (e.Result == HttpClientResult.Error || (e.HttpResponse == null && e.ByteResponse == null))
				{
					_responseBox.Text = Properties.Resources.ConnectionError;
				}
				else
				{
					if (e.HttpResponse != null)
					{
						_responseBox.Text = e.HttpResponse.ToString();
					}
					else
					{
						_responseBox.Text = Constants.DefaultEncoding.GetString(
						e.ByteResponse);
					
					}
				}
			});
		}

		
		private void RequestResent(object sender, EventArgs e)
		{
			//signal to the user that we are receiving a response
			_responseBox.Text = Properties.Resources.WaitingForResponse;
		}

		private void _splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
		{

		}

	}
	/// <summary>
	/// Used with changes in the request information
	/// </summary>
	/// <param name="e"></param>
	public delegate void RequestTrafficViewSaveEvent(RequestTrafficViewSaveArgs e);
	/// <summary>
	/// Information regarding the requests being modified
	/// </summary>
	public class RequestTrafficViewSaveArgs : EventArgs
	{
		private string _request;
		public string Request
		{
			get { return _request; }
		}

		private string _response;
		public string Response
		{
			get { return _response; }
		}

		public RequestTrafficViewSaveArgs(string request, string response)
		{
			_request = request;
			_response = response;
		}
	}
}
