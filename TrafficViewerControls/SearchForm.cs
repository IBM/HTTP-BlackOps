/******************************************************************
* IBM Confidential
* OCO Source Materials
* IBM Rational Traffic Viewer
* (c) Copyright IBM Corp. 2010 All Rights Reserved.
* 
* The source code for this program is not published or otherwise
* divested of its trade secrets, irrespective of what has been
* deposited with the U.S. Copyright Office.
******************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;

namespace TrafficViewerControls
{
	public partial class SearchForm : Form
	{
		private ITrafficViewerDataAccessor _dataSource;
		private Search _search;
		private const int LOAD_CHUNK_SIZE = 500; //how many search results to load at a time
		private int _last;
		private bool _requestTimerStop;

		/// <summary>
		/// Search control constructor
		/// </summary>
		/// <param name="dataSource">Data source of the search</param>
		public SearchForm(ITrafficViewerDataAccessor dataSource)
		{
			InitializeComponent();
			_dataSource = dataSource;
			_dropType.SelectedIndex = 3;
		}

		/// <summary>
		/// Occurs when the search button was clicked 
		/// </summary>
		public event SearchExecutedEvent SearchExecuted;

		/// <summary>
		/// Occurs when an item was selected on the search form
		/// </summary>
		public event SearchIndexChangedEvent SearchIndexChanged;

		private void SearchClick(object sender, EventArgs e)
		{
			StartSearch();
		}

		private void StartSearch()
		{
			if (_boxSearchText.Text != String.Empty)
			{
				_buttonSearch.Enabled = false;
				_progressBar.Visible = true;
				this.Text = _boxSearchText.Text;

				//configure the search
				SearchType type = (SearchType)_dropType.SelectedIndex;
				_search = new Search(_boxSearchText.Text, _checkIsRegex.Checked, type, _boxDescriptionFilter.Text);
				_last = 0;
				_list.Items.Clear();

				//run the background worker
				_searchWorker.RunWorkerAsync();

				//start the load timer
				_requestTimerStop = false;
				_timer.Start();
				
				//invoke the event
				if (SearchExecuted != null)
				{
					SearchExecuted.Invoke(new SearchExecutedEventArgs(_boxSearchText.Text,_checkIsRegex.Checked));
				}
			}
		}

		private void SearchWorkerDoWork(object sender, DoWorkEventArgs e)
		{
			_dataSource.Search(ref _search);
		}

		private void SearchWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			_requestTimerStop = true;		
		}

		private void TimerTick(object sender, EventArgs e)
		{
			_timer.Stop();

			//update the progress bar
			if (_progressBar.Value >= 100)
			{
				_progressBar.Value = 0;
			}
			_progressBar.Value += _progressBar.Step;

			int i, n = _search.Matches.Count;

			for (i = _last; i < n && i-_last < LOAD_CHUNK_SIZE; i++)
			{
				_list.Items.Add(String.Format("{0:D6}: {1}",
					_search.Matches[i].Key, _search.Matches[i].Value));
			}
			_last = i;

			if (_last < n || !_requestTimerStop)
			{
				_timer.Start();
			}
			else
			{
				_progressBar.Visible = false;
				_buttonSearch.Enabled = true;
			}
		}

		private void SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SearchIndexChanged != null)
			{
				KeyValuePair<int,string> entry = _search.Matches[_list.SelectedIndex];
				SearchIndexChanged.Invoke(new SearchIndexChangedEventArgs(_boxSearchText.Text,entry.Key,entry.Value,_checkIsRegex.Checked));
			}
		}

		private void SearchTextKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				e.KeyChar = '\0';
				StartSearch();
			}
		}

		private void DescriptionFilterKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				e.KeyChar = '\0';
				StartSearch();
			}
		}
	}
}