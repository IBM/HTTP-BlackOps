using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;

namespace TrafficViewerControls
{
	/// <summary>
	/// Encapsulates the logic and controls used to view the request details
	/// </summary>
	public partial class RequestViewer : UserControl
	{
		/// <summary>
		/// Enum indicating the current selected tab
		/// </summary>
		public RequestViewerTabs SelectedTab
		{
			get
			{
				RequestViewerTabs returnValue = RequestViewerTabs.Unknown;
				this.Invoke((MethodInvoker)delegate
				{
					if (_tabControl.SelectedTab == _tabHttpTraffic)
					{
						returnValue = RequestViewerTabs.HttpTraffic;
					}
                    else if (_tabControl.SelectedTab == _tabEntities)
                    {
                        returnValue = RequestViewerTabs.Entities;
                    }
					else if (_tabControl.SelectedTab == _tabBrowser)
					{
						returnValue = RequestViewerTabs.Browser;
					}
					else if (_tabControl.SelectedTab == _tabDom)
					{
						returnValue = RequestViewerTabs.DOM;
					}
					else if (_tabControl.SelectedTab == _tabLogSync)
					{
						returnValue = RequestViewerTabs.LogSync;
					}
				});
				return returnValue;
			}
		}

		/// <summary>
		/// Gets and sets the text of the request box
		/// </summary>
		public string RequestText
		{
			get
			{
				return _pageTrafficView.RequestText;
			}
			set
			{
				_pageTrafficView.RequestText = value;
			}
		}

		/// <summary>
		/// Gets and sets the text of the response box
		/// </summary>
		public string ResponseText
		{
			get
			{
				return _pageTrafficView.ResponseText;
			}
			set
			{
				_pageTrafficView.ResponseText = value;
			}
		}

		/// <summary>
		/// Allows the traffic view control to be modified Cross-Thread
		/// </summary>
		/// <param name="requestText"></param>
		/// <param name="responseText"></param>
		public void CrossThreadPopulateTrafficView(string requestText, string responseText)
		{
			this.Invoke((MethodInvoker)delegate
			{
				if (_pageTrafficView.RequestText != requestText)
				{
					_pageTrafficView.RequestText = requestText;
				}

				if (_pageTrafficView.ResponseText != responseText)
				{
					_pageTrafficView.ResponseText = responseText;
				}
			});
		}

        /// <summary>
        /// Loads the entities view
        /// </summary>
        /// <param name="requestText"></param>
        /// <param name="responseText"></param>
        public void LoadEntitiesView(int requestId, ITrafficDataAccessor accessor, string requestText)
        {
            this.Invoke((MethodInvoker)delegate
            {
                _pageEntitiesView.LoadRequest(requestId, accessor, requestText);
            });
        }

		/// <summary>
		/// Navigates the browser
		/// </summary>
		/// <param name="requestHeader"></param>
		/// <param name="fullResponse"></param>
		public void NavigateBrowser(TVRequestInfo requestHeader, byte[] fullResponse)
		{
			this.Invoke((MethodInvoker)delegate
			{
				if (SelectedTab == RequestViewerTabs.Browser)
				{
					_pageBrowserView.Navigate(requestHeader, fullResponse);
				}
				else
				{
					_domView.Navigate(requestHeader, fullResponse);
				}
			});
		}

		/// <summary>
		/// The time to be searched in the log sync view
		/// </summary>
		public DateTime LogSyncViewEvent
		{
			get
			{
				return _pageLogSyncView.EventTime;
			}
			set
			{
				_pageLogSyncView.EventTime = value;
			}
		}

		/// <summary>
		/// Event triggered when the tab control index changes
		/// </summary>
		public event EventHandler SelectedIndexChanged
		{
			add
			{
				_tabControl.SelectedIndexChanged += value;
			}
			remove
			{
				_tabControl.SelectedIndexChanged -= value;
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
			_pageTrafficView.SetHighlighting(text, isRegex, line);
		}

		/// <summary>
		/// Sets highlighting during on click. Used for search
		/// </summary>
		/// <param name="text">The text to highlight</param>
		/// <param name="isRegex">If is a regex</param>
		public void SetHighlighting(string text, bool isRegex)
		{
			_pageTrafficView.SetHighlighting(text, isRegex, String.Empty);
		}

		/// <summary>
		/// Gets the traffic view member
		/// </summary>
		public RequestTrafficView TrafficView
		{
			get
			{
				return _pageTrafficView;
			}
		}


		/// <summary>
		/// Constructor
		/// </summary>
		public RequestViewer()
		{
			InitializeComponent();
		}


	}

}
