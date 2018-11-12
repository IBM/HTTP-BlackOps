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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using TrafficViewerSDK;
using System.Threading;

namespace TrafficViewerControls
{
	/// <summary>
	/// This class encapsulates the graphics and the GUI operations required to control the requests list
	/// </summary>
	public partial class RequestsList : UserControl
	{
		#region Fields & Constants

		/// <summary>
		/// Number format
		/// </summary>
		private const string NUMBER_FORMAT = "000000";

		/// <summary>
		/// The format to display the response size
		/// </summary>
		private const string SIZE_FORMAT = "{0,6:F1}kb";

		/// <summary>
		/// The maximum ammount of additional columns supported by this list
		/// </summary>
		private const int MAX_CUSTOM_COLUMNS = 3;

		/// <summary>
		/// The maximum number of retried a thred will try do a operation
		/// </summary>
		private const int MAX_THREAD_RETRIES = 3;

		/// <summary>
		/// How long a thread will wait before trying an operation again
		/// </summary>
		private const int RETRY_INTERVAL = 3000;

		/// <summary>
		/// The number of seconds the list will wait to pool the source for more requests
		/// </summary>
		private const int LOAD_DELAY = 200;

		/// <summary>
		/// Value used for creating string builders for cell concatenation operations
		/// </summary>
		private const int ROW_ESTIMATED_LENGTH = 128;

		/// <summary>
		/// Time format used by the request list
		/// </summary>
		private const string TIME_FORMAT = "HH:mm:ss.fff";

		/// <summary>
		/// Step for the progress bar
		/// </summary>
		private const int PROGRESS_STEP = 10;

		/// <summary>
		/// Timer that controls the load operation
		/// </summary>
		private System.Windows.Forms.Timer _timer;

		/// <summary>
		/// Variable that indicates that the load operation should be stopped
		/// </summary>
		private bool _stopRequested = false;

		/// <summary>
		/// Lock used for multithreaded operations
		/// </summary>
		private object _listLock = new object();

		/// <summary>
		/// Maps the request ids to the datagrid list ids
		/// </summary>
		private Dictionary<string, DataGridViewRow> _addedRows;

		/// <summary>
		/// Stores requests ids to be added to the GUI
		/// </summary>
		private Queue<int> _requestHeaderEventsQueue;

		/// <summary>
		/// Information obtained from right click events
		/// </summary>
		private int _currentRightClickedRow = -1;

		#endregion

		#region Properties

		private bool _tail = false;

		private int _loadMaxAdvanceSize = TrafficViewerOptions.Instance.LoadMaxAdvanceSize;

		private ITrafficViewerDataAccessor _dataSource;
		/// <summary>
		/// Gets or sets the data source the request list will feed on
		/// </summary>
		public ITrafficViewerDataAccessor DataSource
		{
			get { return _dataSource; }
			set
			{
				//remove all previous events from teh previous data source
				if (_dataSource != null)
				{
					_dataSource.RequestEntryAdded -= RequestEntryAddedEvent;
					_dataSource.ResponseAdded -= ResponseAddedEvent;
					_dataSource.RequestEntryRemoved -= RequestEntryRemovedEvent;
					_dataSource.StateChanged -= StateChangedEvent;
					_dataSource.DataCleared -= DataClearedEvent;
				}
				_dataSource = value;
				//add new events
				if (value != null)
				{
					//attach events
					_dataSource.RequestEntryAdded += RequestEntryAddedEvent;
					_dataSource.ResponseAdded += ResponseAdded;
					_dataSource.RequestEntryRemoved += RequestEntryRemovedEvent;
					_dataSource.StateChanged += StateChangedEvent;
					_dataSource.DataCleared += DataClearedEvent;

				}
			}
		}

		private int _requestEventsQueueSize = 0;
		/// <summary>
		/// Gets or sets how many requests should be loaded first before written to the GUI
		/// This is a performance option, because drawing operations are CPU intensive
		/// this option can reduce the amount of refresh operations perfomed by the list
		/// </summary>
		public int RequestEventsQueueSize
		{
			get { return _requestEventsQueueSize; }
			set { _requestEventsQueueSize = value; }
		}

		private bool _stopLoadingEvents = false;
		/// <summary>
		/// If this flag is set to true the reqeust list will discard any request related events that are thrown at it
		/// </summary>
		public bool StopLoadingEvents
		{
			get { return _stopLoadingEvents; }
			set { _stopLoadingEvents = value; }
		}

		#endregion

		#region Events & Actions

		/// <summary>
		/// Selects a request in the list
		/// </summary>
		/// <param name="requestId">The request id</param>
		public void Select(int requestId)
		{
			DeselectAll();
			DataGridViewRow row = _addedRows[requestId.ToString(NUMBER_FORMAT)];
			row.Selected = true;
			row.Visible = true;
			int rowIndex = _dataGrid.Rows.GetFirstRow(DataGridViewElementStates.Selected);
			int dispRowCount = _dataGrid.DisplayedRowCount(false);
			int firstDisplayed = _dataGrid.FirstDisplayedScrollingRowIndex;

			if (rowIndex < firstDisplayed)
			{
				_dataGrid.FirstDisplayedScrollingRowIndex = rowIndex;
			}
			else if(rowIndex >= firstDisplayed + dispRowCount)
			{
			    _dataGrid.FirstDisplayedScrollingRowIndex = rowIndex - dispRowCount + 1;
			}

		}

		/// <summary>
		/// Occurs when the user selects a row and passes as arguments the request id and header
		/// </summary>
		public event TVDataAccessorDataEvent RequestSelected;
		/// <summary>
		/// Responsible for invoking the RequestSelected event with the proper arguments
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DataGridSelectionChanged(object sender, EventArgs e)
		{
			if (RequestSelected != null)
			{
				//finds the last selected row and extracts the request id
				int rowId = _dataGrid.Rows.GetLastRow(DataGridViewElementStates.Selected | DataGridViewElementStates.Visible);
				if (rowId > -1)
				{
					DataGridViewRow row = _dataGrid.Rows[rowId];
					int requestId = Convert.ToInt32(row.Cells["_id"].Value);
					RequestHeader requestHeader = _dataSource.GetRequestHeader(requestId);
					RequestSelected.Invoke(new TVDataAccessorDataArgs(requestId, requestHeader));
				}
			}
		}

		/// <summary>
		/// Occurs when the stop link at the bottom of the list was clicked
		/// </summary>
		public event EventHandler StopLinkClick
		{
			add
			{
				_stopLink.Click += value;
			}
			remove
			{
				_stopLink.Click -= value;
			}
		}

		/// <summary>
		/// Used to store the logic to handle RequestEntryAdded on the data source
		/// </summary>
		private event TVDataAccessorDataEvent RequestEntryAddedEvent;
		/// <summary>
		/// Method used to initialize the RequestEntryAdded event handler
		/// </summary>
		/// <param name="e"></param>
		private void RequestEntryAdded(TVDataAccessorDataArgs e)
		{
			if (_stopLoadingEvents) return;

			bool isFullQueue = false;

			lock (_listLock)
			{
				//queue request ids to be loaded to the list
				_requestHeaderEventsQueue.Enqueue(e.RequestId);

				if (_requestHeaderEventsQueue.Count >= _requestEventsQueueSize)
				{
					isFullQueue = true;
				}

			}

			if (isFullQueue) //the events queue is full and tail is not on 
			{
				this.BeginInvoke((MethodInvoker)delegate //use invoke to avoid cross-thread exceptions
				{
					lock (_listLock)
					{
						//reset the full queue flag
						isFullQueue = false;

						//the queue is full load all the requests to the GUI
						DrainRequestEvents();

						//increase the queue size using an adaptive behavior, improves performance
						if (!_tail)
						{
							//if tail is not on increase the size with the amount of rows from the selected row to the end of the list
							if (_requestEventsQueueSize < _loadMaxAdvanceSize)
							{
								_requestEventsQueueSize =
									_dataGrid.Rows.Count - _dataGrid.Rows.GetFirstRow(DataGridViewElementStates.Selected);
								if (_requestEventsQueueSize > _loadMaxAdvanceSize)
								{
									_requestEventsQueueSize = _loadMaxAdvanceSize;
								}
							}
							//also wait a little bit between two loads to prevent the gui from becoming unresponsive
						}
						else
						{
							_requestEventsQueueSize = _dataSource.RequestCount - _dataGrid.Rows.Count;
						}
					}
				});
			}
		}

		/// <summary>
		/// Drains all the requests ids from the events queue and adds them to the GUI
		/// </summary>
		private void DrainRequestEvents()
		{
			while (_requestHeaderEventsQueue.Count > 0)
			{
				int id = _requestHeaderEventsQueue.Dequeue();
				RequestHeader header = _dataSource.GetRequestHeader(id);
				AddRow(new KeyValuePair<int, RequestHeader>(id, header));
			}
		}

		private event TVDataAccessorDataEvent ResponseAddedEvent;
		/// <summary>
		/// Mainly used to add response status for request headers that didn't have a response at the time
		/// </summary>
		/// <param name="e"></param>
		private void ResponseAdded(TVDataAccessorDataArgs e)
		{
			if (_stopLoadingEvents) return;

			this.Invoke((MethodInvoker)delegate
			{
				lock (_listLock) //prevent multiple threads from doing this at the same time
				{
					int id = e.RequestId;
					string stringId = id.ToString(NUMBER_FORMAT);
					if (_addedRows.ContainsKey(stringId))
					{
						DataGridViewRow row = _addedRows[stringId];
						row.Cells["_status"].Value = e.Header.ResponseStatus;
						row.Cells["_respTime"].Value = e.Header.ResponseTime.ToString(TIME_FORMAT);
						decimal respSize = (decimal)e.Header.ResponseLength / 1024;
						row.Cells["_respSize"].Value = String.Format(SIZE_FORMAT, respSize);
					}
				}
			});
		}

		private event TVDataAccessorDataEvent RequestEntryRemovedEvent;
		/// <summary>
		/// Requests when a request was unlinked from the list
		/// </summary>
		/// <param name="e"></param>
		private void RequestEntryRemoved(TVDataAccessorDataArgs e)
		{

			this.Invoke((MethodInvoker)delegate
			{
				lock (_listLock) //prevent multiple threads from doing this at the same time
				{
					int id = e.RequestId;
					string stringId = id.ToString(NUMBER_FORMAT);
					if (_addedRows.ContainsKey(stringId))
					{
						DataGridViewRow row = _addedRows[stringId];
						_addedRows.Remove(stringId);
						_dataGrid.Rows.Remove(row);
					}
				}
			});
		}

		private event TVDataAccessorDataEvent DataClearedEvent;
		/// <summary>
		/// Executes when the data source was cleared
		/// </summary>
		/// <param name="e"></param>
		private void DataCleared(TVDataAccessorDataArgs e)
		{
			this.Invoke((MethodInvoker)delegate
			{
				lock (_listLock) //prevent multiple threads from doing this at the same time
				{
					_dataGrid.Rows.Clear();
					_addedRows.Clear();
				}
			});
		}

		/// <summary>
		/// Stores the logic to handle the StateChanged event on the data source
		/// </summary>
		private event TVDataAccessorStateHandler StateChangedEvent;
		/// <summary>
		/// Occurs when the data source state has changed
		/// Modifies the status bar to indicate a change has occured in the data source
		/// </summary>
		/// <param name="e"></param>
		private void StateChanged(TVDataAccessorStateArgs e)
		{
			//Change the control in a thread safe manner
			this.Invoke((MethodInvoker)delegate
				{
					if (e.State == AccessorState.Loading)
					{
						//reset the request queue size
						_requestEventsQueueSize = 0;
						_stopLink.Visible = true;
						_progressBar.Visible = true;
						//start the progress timer
						_progressTimer.Start();
					}
					else if (e.State == AccessorState.Tailing)
					{
						_tail = true;

						_requestEventsQueueSize = 0;
						_stopLink.Visible = true;
						_progressBar.Visible = true;
						//start the progress timer
						_progressTimer.Start();

						if (_dataGrid.Rows.Count > 0)
						{
							//select the last request in the list
							DeselectAll();
							int lastRow = _dataGrid.Rows.GetLastRow(DataGridViewElementStates.Visible);
							_dataGrid.Rows[lastRow].Selected = true;
							_dataGrid.FirstDisplayedScrollingRowIndex = lastRow;
						}
					}
					else if (e.State == AccessorState.Idle)
					{
						//stop the tail if tail was on

						_tail = false;

						lock (_listLock)
						{
							//load all the remaining requests to the queue if there's any
							DrainRequestEvents();
							_stopLink.Visible = false;
							_progressBar.Visible = false;
							_progressTimer.Stop();
						}
					}
				}
			);

		}

		/// <summary>
		/// Advances the progress bar
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ProgressTimerTick(object sender, EventArgs e)
		{
			_progressTimer.Stop();
			//update progress bar
			if (_progressBar.Value + PROGRESS_STEP <= 100)
			{
				_progressBar.Value += PROGRESS_STEP;
			}
			else
			{
				_progressBar.Value = 0;
			}
			_progressTimer.Start();
		}

		private DynamicFilter _filter;

		#endregion

		#region Data Operations

		/// <summary>
		/// Hides rows that match the regular expression or hides the rows that don't match if 
		/// the reverse flag is set to true
		/// </summary>
		/// <param name="regex">Case insensitive regular expression</param>
		/// <param name="reverse">If true the rows that don't match will be hidden</param>
		public void SetFilter(string regex, bool reverse)
		{
			_filter.SetFilter(regex, reverse);
		}

		/// <summary>
		/// Adds a header row to the request list
		/// </summary>
		/// <param name="header">the header id and the header</param>
		private void AddRow(KeyValuePair<int, RequestHeader> header)
		{
			if (header.Value == null)
			{
				//the request list was probably cleared in the meanwhile
				return;
			}

			//ADD
			//obtain custom values defined by the user
			string[] customFields = header.Value.GetCustomFieldsArray(MAX_CUSTOM_COLUMNS);
			decimal respSize = (decimal)header.Value.ResponseLength / 1024;
			//add the row
			string stringId = header.Key.ToString(NUMBER_FORMAT);
			_dataGrid.Rows.Add(stringId,
				header.Value.RequestLine,
				header.Value.ResponseStatus,
				header.Value.ThreadId,
				header.Value.Description,
				header.Value.RequestTime.ToString(TIME_FORMAT),
				header.Value.ResponseTime.ToString(TIME_FORMAT),
				String.Format(SIZE_FORMAT,respSize),
				customFields[0],
				customFields[1],
				customFields[2]);

			int currentId = _dataGrid.Rows.Count - 1;

			_addedRows.Add(stringId, _dataGrid.Rows[currentId]);

			//HIGHLIGHTING
			Color selectedColor, requestColor;

			selectedColor = TVColorConverter.GetColorForRequestDescription
				(_dataSource.Profile, header.Value.Description);

			//check for specific request highlighting
			requestColor = TVColorConverter.GetColorForRequestId
				(_dataSource.Profile, stringId);

			//if the user specified a highlight for this specific request this color takes precedence
			if (requestColor != Color.White)
			{
				selectedColor = requestColor;
			}

			//if there is a color defined for this description 
			if (selectedColor != Color.White)
			{
				_dataGrid.Rows[currentId].DefaultCellStyle.BackColor = selectedColor;
			}

			//Filter
			_dataGrid.Rows[currentId].Visible = _filter.GetRowVisibility(_dataGrid.Rows[currentId]);

			//TAIL Select and Scroll
			//if auto scroll on add is enabled 
			if (_tail && _dataGrid.SelectedRows.Count < 2)
			{
				bool tailNow = false;
				//check if the last row was selected previously
				if (_dataGrid.Rows.Count > 1)
				{
					int selectedRow;
					selectedRow = _dataGrid.Rows.GetLastRow(DataGridViewElementStates.Selected);
					if (selectedRow == _dataGrid.Rows.Count - 2)
					{
						//deselect this row
						_dataGrid.Rows[selectedRow].Selected = false;
						tailNow = true;
					}
				}

				//if the list was empty before automatically select
				if (_dataGrid.Rows.Count == 1)
				{
					tailNow = true;
				}

				if (tailNow)
				{
					//select the last visible element in the list
					int l = _dataGrid.Rows.GetLastRow(DataGridViewElementStates.Visible);
					_dataGrid.Rows[l].Selected = true;
					//scroll to it
					if (l > -1)
					{
						_dataGrid.FirstDisplayedScrollingRowIndex = l;
					}
				}
			}
		}

		/// <summary>
		/// Deselects all the selected rows
		/// </summary>
		private void DeselectAll()
		{
			_dataGrid.ClearSelection();
		}

		#endregion

		#region Context Menu

		#region Columns

		/// <summary>
		/// Loads the user preferences regarding the required columns
		/// </summary>
		private void SetColumnsState()
		{
			Dictionary<string, bool> columns = TrafficViewerOptions.Instance.GetRequestListColumns();
			int index = 0;
			foreach (string columnName in columns.Keys)
			{
				try
				{
					_dataGrid.Columns[columnName].Visible = columns[columnName];
					_dataGrid.Columns[columnName].DisplayIndex = index;
					index++;
				}
				catch (Exception err)
				{
					Utils.Log("Cannot set request list columns state: " + err.Message, LogMessageType.Error);
				}
			}

		}

		/// <summary>
		/// Creates an array of menu items corresponding with the columns of the request list
		/// </summary>
		/// <returns></returns>
		private ColumnSelectMenuItem[] GetColumnMenuEntries()
		{
			ColumnSelectMenuItem[] entries = new ColumnSelectMenuItem[_dataGrid.Columns.Count];
			for (int i = 0; i < entries.Length; i++)
			{
				entries[i] = new ColumnSelectMenuItem();
				entries[i].Click += new EventHandler(ColumnSelect);
				entries[i].Column = _dataGrid.Columns[i];
			}
			return entries;
		}

		/// <summary>
		/// Sets a column visible or invisible and saves the user preferences
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ColumnSelect(object sender, EventArgs e)
		{
			ColumnSelectMenuItem columnMenu = (ColumnSelectMenuItem)sender;
			columnMenu.Checked = !columnMenu.Checked;
			columnMenu.Column.Visible = columnMenu.Checked;
			SaveColumnsState();
		}

		/// <summary>
		/// Saves the position and visibility of columns to the options file
		/// </summary>
		private void SaveColumnsState()
		{
			Dictionary<string, bool> columns = new Dictionary<string, bool>();
			DataGridViewColumn column = _dataGrid.Columns.GetFirstColumn(DataGridViewElementStates.None);
			while (column != null)
			{
				columns.Add(column.Name, column.Visible);
				column = _dataGrid.Columns.GetNextColumn(column, DataGridViewElementStates.None, DataGridViewElementStates.None);
			}
			TrafficViewerOptions.Instance.SetRequestListColumns(columns);
		}

		//Save the columns order every time when the user changes it
		private void ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
		{
			SaveColumnsState();
		}

		#endregion

		#region Highlight

		/// <summary>
		/// Saves the row that was right clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DataGridMouseClick(object sender, MouseEventArgs e)
		{
			//handle the right click for highlighting
			if (e.Button == MouseButtons.Right)
			{
				DataGridView.HitTestInfo hti = _dataGrid.HitTest(e.X, e.Y);
				if (hti.RowIndex > -1)
				{
					//save the current right clicked row
					_currentRightClickedRow = hti.RowIndex;

					//select the current row and deselect the previous (if only one row is selected)
					if (_dataGrid.SelectedRows.Count == 1)
					{
						_dataGrid.SelectedRows[0].Selected = false;
					}
					_dataGrid.Rows[_currentRightClickedRow].Selected = true;
				}
			}
		}

		private void HighlightDescriptionClick(object sender, EventArgs e)
		{
			DialogResult dr = _colorDialog.ShowDialog();

			if (dr == DialogResult.OK)
			{
				//get current description
				string description = (string)_dataGrid.Rows[_currentRightClickedRow].Cells["_description"].Value;
				//save this to the current profile
				Dictionary<string, string> hDefs = _dataSource.Profile.GetHighlightingDefinitions();
				if (!hDefs.ContainsKey(description))
				{
					hDefs.Add(description, TVColorConverter.GetARGBString(_colorDialog.Color));
					_dataSource.Profile.SetHighlightingDefinitions(hDefs);
				}
				//color all the visible rows that have this description
				int n = _dataGrid.Rows.Count;
				string c;
				for (int i = 0; i < n; i++)
				{
					c = (string)_dataGrid.Rows[i].Cells["_description"].Value;
					if (c == description)
					{
						_dataGrid.Rows[i].DefaultCellStyle.BackColor = _colorDialog.Color;
					}
				}
			}
		}

		private void HighlightSelectionClick(object sender, EventArgs e)
		{
			DialogResult dr = _colorDialog.ShowDialog();

			if (dr == DialogResult.OK)
			{
				Dictionary<string, string> hDefs = _dataSource.Profile.GetHighlightingDefinitions();
				string selectedColor = TVColorConverter.GetARGBString(_colorDialog.Color);
				foreach (DataGridViewRow row in _dataGrid.SelectedRows)
				{
					string id = (string)row.Cells["_id"].Value;
					//save this to the current profile
					if (!hDefs.ContainsKey(id))
					{
						hDefs.Add(id, selectedColor);
					}
					else
					{
						hDefs[id] = selectedColor;
					}

					//color the row
					row.DefaultCellStyle.BackColor = _colorDialog.Color;
					//deselect the row
					row.Selected = false;
				}

				//apply new highlighting definitions
				_dataSource.Profile.SetHighlightingDefinitions(hDefs);
				//deselect all rows

			}
		}

		#endregion

		#region Browse

		private void IELaunch(string prefix)
		{
			try
			{
				string fullPath = GetAddress(prefix);
				System.Diagnostics.Process IE = new System.Diagnostics.Process();
				IE.StartInfo.FileName = "iexplore.exe";
				IE.StartInfo.Arguments = fullPath;
				IE.Start();
			}
			catch { }
		}

		private string GetAddress(string prefix)
		{
			int index = Convert.ToInt32(_dataGrid.Rows[_currentRightClickedRow].Cells["_id"].Value);
			byte[] requestData = _dataSource.LoadRequestData(index);
			string request = Encoding.UTF8.GetString(requestData);
			RequestHeader requestHeader = _dataSource.GetRequestHeader(index);

			string host = Utils.RegexGroupValue(request, @"Host: ([^\r\n]+)");

			//get full path including query
			string fullPath = Utils.RegexGroupValue(requestHeader.RequestLine, @"^(?:GET|POST) (.+) HTTP");

			//create URL
			if (!Utils.IsMatch(fullPath, "https?://"))
			{
				fullPath = String.Format("{0}{1}{2}", prefix, host, fullPath);
			}
			return fullPath;
		}

		private void HttpLaunchClick(object sender, EventArgs e)
		{
			IELaunch("http://");
		}

		private void HttpsLaunchClick(object sender, EventArgs e)
		{
			IELaunch("https://");
		}

		private void CopyUrlHttpClick(object sender, EventArgs e)
		{
			Clipboard.SetText(GetAddress("http://"));
		}

		private void CopyUrlHttpsClick(object sender, EventArgs e)
		{
			Clipboard.SetText(GetAddress("https://"));
		}

		private void BrowseMenuOver(object sender, EventArgs e)
		{
			string requestLine = (string)_dataGrid.Rows[_currentRightClickedRow].Cells["_requestLine"].Value;
			if (!Utils.IsMatch(requestLine, "^(GET|POST)"))
			{
				_httpLaunch.Enabled = false;
				_httpsLaunch.Enabled = false;
			}
			else
			{
				_httpLaunch.Enabled = true;
				_httpsLaunch.Enabled = true;
			}
		}

		#endregion

		#region Edit

		private void SelectAllClick(object sender, EventArgs e)
		{
			_dataGrid.SelectAll();
		}

		private void CopySelectionClick(object sender, EventArgs e)
		{
			Clipboard.SetDataObject(_dataGrid.GetClipboardContent());
		}

		private void DeleteSelectionClick(object sender, EventArgs e)
		{
			lock (_listLock)
			{
				foreach (DataGridViewRow row in _dataGrid.SelectedRows)
				{
					string stringId = (string)row.Cells["_id"].Value;
					int id = Convert.ToInt32(stringId);

					//unlink the request from the data source
					_dataSource.RemoveRequest(id);
					//remove the request from the GUI
					//_dataGrid.Rows.Remove(row);
				}
			}
		}

		#endregion

		#endregion

		#region Code to load from a full data source


		private void StopLinkClicked(object sender, EventArgs e)
		{
			_stopRequested = true;
		}

		/// <summary>
		/// Loads a batch of requests to the list periodically
		/// </summary>
		private void LoadTick(object sender, EventArgs e)
		{
			KeyValuePair<int, RequestHeader> requestHeader;
			//stop the timer
			_timer.Stop();

			//update the progress bar
			if (_progressBar.Value >= 100)
			{
				_progressBar.Value = 0;
			}
			_progressBar.Value += _progressBar.Step;

			//read as many requests as there are available as long as the source is still
			//loading or as long as the user didn't pause the load operation
			do
			{
				if (_stopRequested)
				{
					//stop the load & hide the progress bar
					_timer.Stop();
					_stopLink.Visible = false;
					_progressBar.Visible = false;
					return;
				}

				//get the next request header, null will be returned at the end of the list
				requestHeader = _dataSource.GetNext();
				if (requestHeader.Value != null)
				{
					AddRow(requestHeader);
				}
				else
				{
					//go back and update the headers we didn't have a response for when they were added
					UpdateEmptyStatusRows();
					//check to see if the source is still loading or if it's done
					if (_dataSource.State == AccessorState.Idle)
					{
						_stopRequested = true; //the source has finished the loading operation
					}
				}
			}
			while (requestHeader.Value != null);
			//if there are no remaining requests to load but the source is still loading resume the timer
			_timer.Start();
		}



		/// <summary>
		/// Stops the loading operation
		/// </summary>
		public void CancelLoading()
		{
			_stopRequested = true;
		}


		/// <summary>
		/// Initializes an asyncroneous load operation from the specified source 
		/// </summary>
		/// <param name="source">Data accessor for our request list</param>
		public void LoadRequests(ITrafficViewerDataAccessor source)
		{
			_dataSource = source;
			_stopRequested = false;
			_stopLink.Visible = true;
			_progressBar.Visible = true;
			_timer = new System.Windows.Forms.Timer();
			_timer.Tick += new EventHandler(LoadTick);
			_timer.Interval = LOAD_DELAY;
			_timer.Start();
		}


		/// <summary>
		/// Updates rows that we didn't have a status for when we added them
		/// </summary>
		private void UpdateEmptyStatusRows()
		{
			int id, index = 0;
			RequestHeader header;
			while (index < _dataGrid.Rows.Count)
			{
				//get id of the current row
				id = Convert.ToInt32(_dataGrid.Rows[index].Cells["_id"].Value);
				header = _dataSource.GetRequestHeader(id);
				if (header.ResponseStatus != String.Empty)
				{
					_dataGrid.Rows[index].Cells["_status"].Value = header.ResponseStatus;
					_dataGrid.Rows[index].Cells["_respTime"].Value = header.ResponseTime.ToString(TIME_FORMAT);
				}
				index++;
			}
		}

		#endregion

		/// <summary>
		/// Initializes a RequestList control
		/// </summary>
		public RequestsList()
		{
			InitializeComponent();

			// Populate event handlers
			// This event handlers are used to automatically syncronize the list with its data source
			RequestEntryAddedEvent = new TVDataAccessorDataEvent(RequestEntryAdded);
			ResponseAddedEvent = new TVDataAccessorDataEvent(ResponseAdded);
			RequestEntryRemovedEvent = new TVDataAccessorDataEvent(RequestEntryRemoved);
			StateChangedEvent = new TVDataAccessorStateHandler(StateChanged);
			DataClearedEvent = new TVDataAccessorDataEvent(DataCleared);


			// Load the columns state from the options file
			SetColumnsState();

			// Add column selector to the list context menu
			this.columnsToolStripMenuItem.DropDownItems.AddRange(GetColumnMenuEntries());

			// Initialize the request ids record
			_addedRows = new Dictionary<string, DataGridViewRow>();

			// Initialize the queue for the requests to be added to the GUI
			_requestHeaderEventsQueue = new Queue<int>();

			// Initialize the dynamic filter
			_filter = new DynamicFilter(_dataGrid);
		}




	}
}
