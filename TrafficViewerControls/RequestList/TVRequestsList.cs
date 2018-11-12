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
using TrafficViewerControls.Diff;
using TrafficViewerSDK.Options;
using TrafficViewerControls.Properties;
using TrafficViewerSDK.Http;
using TrafficViewerSDK.Search;
using System.Text.RegularExpressions;
using TrafficViewerSDK.Senders;
using TrafficViewerInstance;
using System.Diagnostics;
using System.Net.Security;
using TrafficViewerControls.Browsing;
using System.Net;
using TrafficServer;
using CommonControls;

namespace TrafficViewerControls.RequestList
{
    /// <summary>
    /// This class encapsulates the graphics and the GUI operations required to control the requests list
    /// </summary>
    public partial class TVRequestsList : UserControl
    {
        #region Fields & Constants

        /// <summary>
        /// Max len of req line
        /// </summary>
        private int MAX_REQ_LINE_LEN = 1024;

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
        private Dictionary<int, DataGridViewRow> _addedRows;

        /// <summary>
        /// Stores requests ids to be added to the GUI
        /// </summary>
        private Queue<int> _requestHeaderEventsQueue;

        /// <summary>
        /// Information obtained from right click events
        /// </summary>
        private int _currentRightClickedRow = -1;

        /// <summary>
        /// Filters the request list based on a specified criteria
        /// </summary>
        private DynamicFilter _filter;

       
        /// <summary>
        /// DIsplays a progress dialog
        /// </summary>
        private ProgressDialog _progressDialog = new ProgressDialog();


        private bool _disableAutoScroll = false;
        /// <summary>
        /// DIsables the auto scroll for the grid (don't forget to turn it back on....)
        /// </summary>
        public bool DisableAutoScroll
        {
            get { return _disableAutoScroll; }
            set { _disableAutoScroll = value; }
        }

        #endregion

        #region Properties

        private int _loadMaxAdvanceSize = TrafficViewerOptions.Instance.LoadMaxAdvanceSize;

        private ITrafficDataAccessor _dataSource;
        /// <summary>
        /// Gets or sets the data source the request list will feed on
        /// </summary>
        public ITrafficDataAccessor DataSource
        {
            get { return _dataSource; }
            set
            {
                //remove all previous events from teh previous data source
                if (_dataSource != null)
                {
                    _dataSource.RequestEntryAdded -= RequestEntryAddedEvent;
                    _dataSource.RequestEntryUpdated -= RequestEntryUpdatedEvent;
                    _dataSource.RequestChanged -= RequestChangedEvent;
                    _dataSource.ResponseChanged -= ResponseChangedEvent;
                    _dataSource.RequestEntryRemoved -= RequestEntryRemovedEvent;
                    _dataSource.RequestBatchRemoved -= RequestBatchRemovedEvent;
                    _dataSource.StateChanged -= StateChangedEvent;
                    _dataSource.DataCleared -= DataClearedEvent;
                }
                _dataSource = value;
                //add new events
                if (value != null)
                {
                    //attach events
                    _dataSource.RequestEntryAdded += RequestEntryAddedEvent;
                    _dataSource.RequestEntryUpdated += RequestEntryUpdatedEvent;
                    _dataSource.RequestChanged += RequestChangedEvent;
                    _dataSource.ResponseChanged += ResponseChanged;
                    _dataSource.RequestEntryRemoved += RequestEntryRemovedEvent;
                    _dataSource.RequestBatchRemoved += RequestBatchRemovedEvent;
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
        /// Occurs when the user updates the description of the request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellUpdated(object sender, DataGridViewCellEventArgs e)
        {
            if (_dataGrid.SelectedRows.Count > 0)
            {
                string idstring = (string)_dataGrid.SelectedRows[0].Cells["_id"].Value;
                string description = (string)_dataGrid.SelectedRows[0].Cells["_description"].Value;
                string scheme = (string)_dataGrid.SelectedRows[0].Cells["_scheme"].Value;
                string requestContext = (string)_dataGrid.SelectedRows[0].Cells["_requestContext"].Value;
                string refererIdString = _dataGrid.SelectedRows[0].Cells["_refererId"].Value as string;
                int id;
                if (int.TryParse(idstring, out id))
                {
                    TVRequestInfo selectedTVInfo = _dataSource.GetRequestInfo(id);
                    if (selectedTVInfo != null)
                    {
                        selectedTVInfo.Description = description;
                        selectedTVInfo.IsHttps = String.Compare(scheme, "https", true) == 0;
                        selectedTVInfo.RequestContext = requestContext;
                        int refererId;
                        if (int.TryParse(refererIdString, out refererId))
                        {
                            selectedTVInfo.RefererId = refererId;
                        }
                    }
                }
            }

            //re-enable the context menu
            _dataGrid.ContextMenuStrip = _contextMenuStrip;
        }

        private void CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //disable the context menu
            _dataGrid.ContextMenuStrip = null;
        }

        /// <summary>
        /// Gets the requests currently selected in the list
        /// </summary>
        /// <returns></returns>
        public List<TVRequestInfo> GetSelectedRequests()
        {
            GetSelectedRequestsAction action = new GetSelectedRequestsAction(this, _dataGrid);
            action.Execute();

            return action.SelectedRequests;

        }

        /// <summary>
        /// Selects a request in the list
        /// </summary>
        /// <param name="requestId">The request id</param>
        /// <param name="selectOne">Whether to deselect previous</param>
        public void Select(int requestId, bool selectOne = true)
        {
            if (_addedRows.ContainsKey(requestId))
            {
                if (selectOne)
                {
                    DeselectAll();
                }
                DataGridViewRow row = _addedRows[requestId];
                if (row.DataGridView != null)
                {
                    row.Selected = true;
                    row.Visible = true;
                    int rowIndex = _dataGrid.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                    int dispRowCount = _dataGrid.DisplayedRowCount(false);
                    int firstDisplayed = _dataGrid.FirstDisplayedScrollingRowIndex;

                    if (rowIndex < firstDisplayed)
                    {
                        _dataGrid.FirstDisplayedScrollingRowIndex = rowIndex;
                    }
                    else if (rowIndex >= firstDisplayed + dispRowCount)
                    {
                        _dataGrid.FirstDisplayedScrollingRowIndex = rowIndex - dispRowCount + 1;
                    }
                }
            }
        }

        /// <summary>
        /// Handles a replace event
        /// </summary>
        /// <param name="replaceArgs"></param>
        public void HandleReplace(ReplaceEventArgs replaceArgs)
        {
            foreach (LineMatch match in replaceArgs.Matches)
            {
                if (match.Context == SearchContext.Request)
                {
                    //get the corresponding request info
                    TVRequestInfo reqInfo = _dataSource.GetRequestInfo(match.RequestId);
                    //update the list
                    _addedRows[match.RequestId].Cells["_requestLine"].Value = reqInfo.RequestLine;
                }
            }
        }


        /// <summary>
        /// Triggered when the user selects the option to clear all filters
        /// </summary>
        public event EventHandler FilterCleared;

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
            if (RequestSelected != null && _dataSource.State != AccessorState.RemovingEntries)
            {
                InvokeRequestSelected();
            }
        }

        private void InvokeRequestSelected()
        {
            //finds the last selected row and extracts the request id
            int rowId = _dataGrid.Rows.GetLastRow(DataGridViewElementStates.Selected | DataGridViewElementStates.Visible);

            /*
            if (rowId == -1)
            {
                rowId = _dataGrid.Rows.GetFirstRow(DataGridViewElementStates.Visible);
            }
            */
            if (rowId > -1)
            {
                DataGridViewRow row = _dataGrid.Rows[rowId];
                int requestId = Convert.ToInt32(row.Cells["_id"].Value);
                TVRequestInfo requestHeader = _dataSource.GetRequestInfo(requestId);
                RequestSelected.Invoke(new TVDataAccessorDataArgs(requestId, requestHeader));
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
        /// Used to store the logic to handle RequestEntryUpdated on the data source
        /// </summary>
        private event TVDataAccessorDataEvent RequestEntryUpdatedEvent;


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

                if (_requestHeaderEventsQueue.Count >= _requestEventsQueueSize 
                    || !String.IsNullOrWhiteSpace(_filter.Filter)
                    || _dataSource.State == AccessorState.Idle)//always add new requests when the state is idle
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
                        if (_dataSource.State == AccessorState.Loading)
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
                TVRequestInfo header = _dataSource.GetRequestInfo(id);
                AddRow(header, true);
            }
        }

        private event TVDataAccessorDataEvent RequestChangedEvent;
        /// <summary>
        /// Used to update request info, when the request was changed 
        /// </summary>
        /// <param name="e"></param>
        private void RequestChanged(TVDataAccessorDataArgs e)
        {
            if (_stopLoadingEvents) return;

            if (_dataSource.State == AccessorState.Loading || _dataSource.State == AccessorState.Tailing) return;

            this.Invoke((MethodInvoker)delegate
            {
                lock (_listLock) //prevent multiple threads from doing this at the same time
                {
                    int id = e.RequestId;
                    string stringId = id.ToString(NUMBER_FORMAT);
                    if (_addedRows.ContainsKey(id))
                    {
                        DataGridViewRow row = _addedRows[id];
                        row.Cells["_requestLine"].Value = GetRequestLine(e.Header);
                        row.Cells["_description"].Value = e.Header.Description;
                        row.Cells["_thread"].Value = e.Header.ThreadId;
                        row.Cells["_reqTime"].Value = e.Header.RequestTime.ToString(TIME_FORMAT);
                        row.Cells["_respTime"].Value = e.Header.ResponseTime.ToString(TIME_FORMAT);
                        row.Cells["_duration"].Value = e.Header.Duration;
                        row.Cells["_requestContext"].Value = e.Header.RequestContext;
                        row.Cells["_refererId"].Value = e.Header.RefererId;
                        row.Cells["_updatedPath"].Value = e.Header.UpdatedPath;
                        row.Cells["_host"].Value = e.Header.Host;
                        ApplyHighlighting(e.Header, stringId, row.Index);
                    }
                }
            });
        }

        private string GetRequestLine(TVRequestInfo info)
        {
            string reqLine = String.Empty;
            if (!String.IsNullOrWhiteSpace(info.RequestLine))
            {
                reqLine = info.RequestLine.Length > MAX_REQ_LINE_LEN ? info.RequestLine.Substring(0, MAX_REQ_LINE_LEN) + "..." : info.RequestLine;
            }
            return reqLine;
        }

        private event TVDataAccessorDataEvent ResponseChangedEvent;
        /// <summary>
        /// Used to update response status for request headers that didn't have a response at the time
        /// </summary>
        /// <param name="e"></param>
        private void ResponseChanged(TVDataAccessorDataArgs e)
        {
            if (_stopLoadingEvents) return;

            this.Invoke((MethodInvoker)delegate
            {
                lock (_listLock) //prevent multiple threads from doing this at the same time
                {
                    int id = e.RequestId;
                    string stringId = id.ToString(NUMBER_FORMAT);
                    if (_addedRows.ContainsKey(id))
                    {
                        DataGridViewRow row = _addedRows[id];
                        row.Cells["_status"].Value = e.Header.ResponseStatus;
                        row.Cells["_respTime"].Value = e.Header.ResponseTime.ToString(TIME_FORMAT);
                        decimal respSize = (decimal)e.Header.ResponseLength / 1024;
                        row.Cells["_respSize"].Value = String.Format(SIZE_FORMAT, respSize);
                        row.Cells["_duration"].Value = e.Header.Duration;
                        string[] customFields = e.Header.GetCustomFieldsArray(MAX_CUSTOM_COLUMNS);
                        row.Cells["_custom1"].Value = customFields[0];
                        row.Cells["_custom2"].Value = customFields[1];
                        row.Cells["_custom3"].Value = customFields[2];
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
                int id = e.RequestId;
                if (_addedRows.ContainsKey(id))
                {
                    DataGridViewRow row = _addedRows[id];
                    _addedRows.Remove(id);
                    _dataGrid.Rows.Remove(row);
                }
            });
        }

        private event TVDataAccessorDataBatchEvent RequestBatchRemovedEvent;
        /// <summary>
        /// Occurs when a batch of requests was unlinked from the list
        /// </summary>
        /// <param name="e"></param>
        private void RequestBatchRemoved(TVDataAccessorBatchEventArgs e)
        {

            this.Invoke((MethodInvoker)delegate
            {
                IEnumerable<int> requestList = e.RequestList;
                foreach (int id in requestList)
                {
                    if (_addedRows.ContainsKey(id))
                    {
                        DataGridViewRow row = _addedRows[id];
                        _addedRows.Remove(id);
                        _dataGrid.Rows.Remove(row);
                    }
                }
                InvokeRequestSelected();
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
                    _currentRightClickedRow = -1;
                    _requestHeaderEventsQueue.Clear();
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
            try
            {

                //Change the control in a thread safe manner
                this.Invoke((MethodInvoker)delegate
                    {
                        //reset the request queue size indifferent of the current state
                        _requestEventsQueueSize = 0;

                        if (e.State == AccessorState.Loading)
                        {
                            _stopLink.Visible = true;
                            _progressBar.Visible = true;
                            _labelStatus.Text = String.Empty;
                            _labelStatus.Visible = true;

                            //start the progress timer
                            _progressTimer.Start();
                        }
                        else if (e.State == AccessorState.Tailing)
                        {
                            _stopLoadingEvents = false;
                            _stopLink.Visible = true;
                            _progressBar.Visible = true;
                            _labelStatus.Text = String.Empty;
                            _labelStatus.Visible = true;
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
                            lock (_listLock)
                            {
                                //load all the remaining requests to the queue if there's any
                                DrainRequestEvents();
                                _stopLink.Visible = false;
                                _progressBar.Visible = false;
                                _labelStatus.Visible = false;
                                _progressTimer.Stop();
                            }
                        }
                    }
                );
            }
            catch (Exception ex)
            {
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, ex.ToString());
            }

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
            _labelStatus.Text = String.Format(TrafficViewerControls.Properties.Resources.RequestsLoadedText, _dataSource.RequestCount);
            _progressTimer.Start();
        }
        #endregion

        #region Data Operations

        /// <summary>
        /// Adds a header row to the request list
        /// </summary>
        /// <param name="info"></param>
        public void AddRow(TVRequestInfo info, bool autoScroll)
        {
            if (info == null)
            {
                //the request list was probably cleared in the meanwhile
                return;
            }

            if (_filter.GetRowVisibility(info.ToString(" ", true)) && !_addedRows.ContainsKey(info.Id))
            {

                //ADD
                //obtain custom values defined by the user
                string[] customFields = info.GetCustomFieldsArray(MAX_CUSTOM_COLUMNS);
                decimal respSize = (decimal)info.ResponseLength / 1024;
                //add the row
                string stringId = info.Id.ToString(NUMBER_FORMAT);
                string reqLine = GetRequestLine(info);
                
                _dataGrid.Rows.Add(stringId,
                    info.Host,
                    reqLine,
                    info.ResponseStatus,
                    info.ThreadId,
                    info.Description,
                    info.RequestTime.ToString(TIME_FORMAT),
                    info.ResponseTime.ToString(TIME_FORMAT),
                    info.Duration,
                    String.Format(SIZE_FORMAT, respSize),
                    info.DomUniquenessId,
                    info.IsHttps ? "https" : "http",
                    info.RequestContext,
                    info.RefererId.ToString(),
                    info.UpdatedPath,
                    customFields[0],
                    customFields[1],
                    customFields[2]);

                int currentId = _dataGrid.Rows.Count - 1;

                _addedRows.Add(info.Id, _dataGrid.Rows[currentId]);

                ApplyHighlighting(info, stringId, currentId);


                //TAIL Select and Scroll
                //if auto scroll on add is enabled 
                if ((_dataSource.State == AccessorState.Tailing || _dataSource.State == AccessorState.Idle)
                    && _dataGrid.SelectedRows.Count < 2 && autoScroll && !_disableAutoScroll)
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
        }

        /// <summary>
        /// Applies highlighting for the specified row
        /// </summary>
        /// <param name="header"></param>
        /// <param name="requestId"></param>
        /// <param name="rowId"></param>
        private void ApplyHighlighting(TVRequestInfo header, string requestId, int rowId)
        {
            //HIGHLIGHTING
            Color selectedColor, requestColor;

            selectedColor = TVColorConverter.GetColorForRequestDescription
                (_dataSource.Profile, header.ToString(" ", true));

            //check for specific request highlighting
            requestColor = TVColorConverter.GetColorForRequestId
                (_dataSource.Profile, requestId);

            //if the user specified a highlight for this specific request this color takes precedence
            if (requestColor != Color.Black)
            {
                selectedColor = requestColor;
            }

            //if there is a color defined for this description 
            if (selectedColor != Color.Black || _dataGrid.Rows[rowId].DefaultCellStyle.BackColor != Color.Black)
            {
                _dataGrid.Rows[rowId].DefaultCellStyle.BackColor = selectedColor;
            }
        }

        /// <summary>
        /// Deselects all the selected rows
        /// </summary>
        private void DeselectAll()
        {
            _dataGrid.ClearSelection();
        }

        /// <summary>
        /// Removes all rows from the gui
        /// </summary>
        public void ClearAllRows()
        {
            _dataGrid.Rows.Clear();
            _addedRows.Clear();
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
                    SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Cannot set request list columns state: " + err.Message);
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
                List<Cookie> cookies;
                string fullPath = GetAddress(prefix, out cookies);
                System.Diagnostics.Process IE = new System.Diagnostics.Process();
                IE.StartInfo.FileName = "iexplore.exe";
                IE.StartInfo.Arguments = fullPath;
                IE.Start();
            }
            catch { }
        }

        private string GetAddress(string prefix, out List<Cookie> cookies)
        {
            string fullPath = String.Empty;
            cookies = new List<Cookie>();
            if (_dataGrid.Rows.Count > 0)
            {
                int index = Convert.ToInt32(_dataGrid.Rows[_currentRightClickedRow].Cells["_id"].Value);
                byte[] requestData = _dataSource.LoadRequestData(index);


                try
                {
                    HttpRequestInfo httpReqInfo = new HttpRequestInfo(requestData, true);
                    foreach (string cookieName in httpReqInfo.Cookies.Keys)
                    {
                        cookies.Add(new Cookie(cookieName, httpReqInfo.Cookies[cookieName]));
                    }
                }
                catch
                {
                    SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Could not parse cookies in TVRequestsList.GetAddress");
                }
                string request = Constants.DefaultEncoding.GetString(requestData);
                TVRequestInfo requestHeader = _dataSource.GetRequestInfo(index);

                string host = Utils.RegexFirstGroupValue(request, @"Host: ([^\r\n]+)");

                //get full path including query
                fullPath = Utils.RegexFirstGroupValue(requestHeader.RequestLine, @"^(?:GET|POST) (.+) HTTP");

                //create URL
                if (fullPath.IndexOf("http", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    if (prefix == null)
                    {
                        prefix = requestHeader.IsHttps ? "https://" : "http://";
                    }
                    fullPath = String.Format("{0}{1}{2}", prefix, host, fullPath);
                }
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
            List<Cookie> cookies;
            Clipboard.SetText(GetAddress("http://", out cookies));
        }

        private void CopyUrlHttpsClick(object sender, EventArgs e)
        {
            List<Cookie> cookies;
            Clipboard.SetText(GetAddress("https://", out cookies));
        }

        private void BrowseMenuOver(object sender, EventArgs e)
        {
            if (_currentRightClickedRow > -1 && _currentRightClickedRow < _dataGrid.Rows.Count)
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
                this.SuspendLayout();

                int i, n = _dataGrid.SelectedRows.Count;
                List<int> requestsToDelete = new List<int>();
                for (i = n - 1; i > -1; i--)
                {
                    DataGridViewRow row = _dataGrid.SelectedRows[i];
                    string stringId = (string)row.Cells["_id"].Value;
                    int id = Convert.ToInt32(stringId);
                    if (_currentRightClickedRow == id)
                    {
                        _currentRightClickedRow = -1;
                    }
                    //unlink the request from the data source, this also removes the request from the UI
                    requestsToDelete.Add(id);
                }
                //unlink the requests from the data source this also removes them from the list
                //by executing the code associated with _dataSource.RequestBatchRemoved event
                _dataSource.RemoveRequestBatch(requestsToDelete);

                this.ResumeLayout();
            }
        }

        /// <summary>
        /// Adds a new request to the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewRawRequestClick(object sender, EventArgs e)
        {
            NewRequest(null);
        }

        private void FromURLToolStripMenuItemClick(object sender, EventArgs e)
        {
            InputMessageBox inputBox = new InputMessageBox();
            string url = inputBox.ShowDialog(Resources.InputUrl);

            if (url != null)
            {
                try
                {
                    NewRequest(url);
                }
                catch
                {
                    ErrorBox.ShowDialog(Resources.InputUrlInvalid);
                }
            }
        }


        /// <summary>
        /// Adds a request
        /// </summary>
        /// <param name="url"></param>
        public void NewRequest(string url)
        {
            string pathAndQuery;
            string host;
            int port;


            if (url != null)
            {
                Uri uri = new Uri(url);
                pathAndQuery = uri.PathAndQuery;
                host = uri.Host;
                port = uri.Port;
            }
            else
            {
                pathAndQuery = "/" + Utils.UrlEncode(Properties.Resources.TrafficViewerRequestDescription);
                host = "demo.testfire.net";
                port = 80;
            }

            TVRequestInfo reqInfo = new TVRequestInfo();
            reqInfo.RequestLine = String.Format("GET {0} HTTP/1.1", pathAndQuery);

            string request = reqInfo.RequestLine + Environment.NewLine + Properties.Resources.SampleRequestHeaders;

            HttpRequestInfo httpReqInfo = new HttpRequestInfo(request);

            httpReqInfo.Host = host;
            httpReqInfo.Port = port;


            reqInfo.Description = Properties.Resources.TrafficViewerRequestDescription;
            reqInfo.ResponseStatus = "200";
            reqInfo.ThreadId = "N/A";
            reqInfo.RequestTime = DateTime.Now;
            reqInfo.ResponseTime = DateTime.Now;


            string response = Properties.Resources.SampleHttpResponse;

            int temp = _requestEventsQueueSize;
            //temporarily change the events queue size to allow the new request to be automatically added
            _requestEventsQueueSize = 0;

            RequestResponseBytes reqData = new RequestResponseBytes();
            reqData.AddToRequest(Constants.DefaultEncoding.GetBytes(httpReqInfo.ToString()));
            reqData.AddToResponse(Constants.DefaultEncoding.GetBytes(response));

            _dataSource.AddRequestInfo(reqInfo);
            _dataSource.SaveRequest(reqInfo.Id, reqData);
            _dataSource.SaveResponse(reqInfo.Id, reqData);
            //revert to the old event queue size
            _requestEventsQueueSize = temp;
            Select(reqInfo.Id);
        }


       


        void ResendRequestsWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //_progressDialog.Stop();
        }

       



        #endregion

        private void CompareFirstTwoRequestsResponses(object sender, EventArgs e)
        {
            if (_dataGrid.SelectedRows.Count > 1)
            {
                DataGridViewRow firstRow = _dataGrid.SelectedRows[0];
                int firstIndex = Convert.ToInt32(firstRow.Cells["_id"].Value);
                DataGridViewRow secondRow = _dataGrid.SelectedRows[1];
                int secondIndex = Convert.ToInt32(secondRow.Cells["_id"].Value);

                SimpleDiffForm diffForm = new SimpleDiffForm(_dataSource, firstIndex, secondIndex);
                diffForm.Show(this);
            }
        }

        private void SenderClick(object sender, EventArgs e)
        {
            string currCaption = (sender as ToolStripMenuItem).Text;
            ISender curSender = null;
            foreach (ISender s in TrafficViewer.Instance.Senders)
            {
                if (s.Caption == currCaption)
                {
                    curSender = s;
                    break;
                }
            }
            if (curSender != null)
            {
                //build the requests for the sender
                List<TVRequestInfo> selectedRequests = new List<TVRequestInfo>();
                foreach (DataGridViewRow row in _dataGrid.SelectedRows)
                {
                    int id = Convert.ToInt32(row.Cells["_id"].Value);
                    selectedRequests.Add(_dataSource.GetRequestInfo(id));
                }
                //call the sender
                curSender.Send(selectedRequests);
            }
        }


        #endregion

        #region Code to load from a fully loaded data source


        private void StopLinkClicked(object sender, EventArgs e)
        {
            _stopRequested = true;
        }

        /// <summary>
        /// Loads a batch of requests to the list periodically
        /// </summary>
        private void LoadTick(object sender, EventArgs e)
        {
            TVRequestInfo requestHeader;
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
                int currIndex = -1;
                requestHeader = _dataSource.GetNext(ref currIndex);
                if (requestHeader != null)
                {
                    AddRow(requestHeader, true);
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
            while (requestHeader != null);
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
        public void LoadRequests(ITrafficDataAccessor source)
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
            TVRequestInfo header;
            while (index < _dataGrid.Rows.Count)
            {
                //get id of the current row
                id = Convert.ToInt32(_dataGrid.Rows[index].Cells["_id"].Value);
                header = _dataSource.GetRequestInfo(id);
                if (header.ResponseStatus != String.Empty)
                {
                    _dataGrid.Rows[index].Cells["_status"].Value = header.ResponseStatus;
                    _dataGrid.Rows[index].Cells["_respTime"].Value = header.ResponseTime.ToString(TIME_FORMAT);
                    _dataGrid.Rows[index].Cells["_duration"].Value = header.Duration;
                }
                index++;
            }
        }

        #endregion

        #region Filters

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

        private void ShowAllClick(object sender, EventArgs e)
        {
            _filter.ClearFilter();
            if (FilterCleared != null)
            {
                FilterCleared.Invoke(this, null);
            }
        }


        
        #endregion

        /// <summary>
        /// Initializes a RequestList control
        /// </summary>
        public TVRequestsList()
        {
            InitializeComponent();

            // Populate event handlers
            // This event handlers are used to automatically syncronize the list with its data source
            RequestEntryAddedEvent = new TVDataAccessorDataEvent(RequestEntryAdded);
            RequestEntryUpdatedEvent = new TVDataAccessorDataEvent(RequestChanged);

            RequestChangedEvent = new TVDataAccessorDataEvent(RequestChanged);
            ResponseChangedEvent = new TVDataAccessorDataEvent(ResponseChanged);

            RequestEntryRemovedEvent = new TVDataAccessorDataEvent(RequestEntryRemoved);
            RequestBatchRemovedEvent = new TVDataAccessorDataBatchEvent(RequestBatchRemoved);

            StateChangedEvent = new TVDataAccessorStateHandler(StateChanged);
            DataClearedEvent = new TVDataAccessorDataEvent(DataCleared);


            // Load the columns state from the options file
            SetColumnsState();

            // Add column selector to the list context menu
            this.columnsToolStripMenuItem.DropDownItems.AddRange(GetColumnMenuEntries());

            // Initialize the request ids record
            _addedRows = new Dictionary<int, DataGridViewRow>();

            // Initialize the queue for the requests to be added to the GUI
            _requestHeaderEventsQueue = new Queue<int>();

            // Initialize the dynamic filter
            _filter = new DynamicFilter(this);

            //initialize the senders
            if (TrafficViewer.Instance.Senders.Count > 0)
            {
                _menuItemSendTo.Visible = true;
                foreach (ISender sender in TrafficViewer.Instance.Senders)
                {
                    ToolStripMenuItem newEntry = new ToolStripMenuItem(sender.Caption);
                    newEntry.Click += new EventHandler(SenderClick);
                    _menuItemSendTo.DropDownItems.Add(newEntry);
                }
            }
        }

        private void CountSelection(object sender, EventArgs e)
        {
            MessageBox.Show(_dataGrid.SelectedRows.Count.ToString());
        }

        private void DuplicateClick(object sender, EventArgs e)
        {
            DuplicateSelectedRequestsAction action = new DuplicateSelectedRequestsAction(this, _dataGrid);
            action.Execute();
        }

        private void ChangeDescriptionClick(object sender, EventArgs e)
        {
            InputMessageBox inputBox = new InputMessageBox();
            string newDescription = inputBox.ShowDialog(Resources.EnterNewDescription);

            int i, count = _dataGrid.SelectedRows.Count;
            for (i = count - 1; i > -1; i--)
            {
                DataGridViewRow row = _dataGrid.SelectedRows[i];

                //get the id
                string stringId = (string)row.Cells["_id"].Value;
                int id = Convert.ToInt32(stringId);
                row.Cells["_description"].Value = newDescription;
                TVRequestInfo reqInfo = _dataSource.GetRequestInfo(id);

                reqInfo.Description = newDescription.Trim('\r', '\n', '\t', ' ');
                ApplyHighlighting(reqInfo, stringId, row.Index);
            }
        }


        private void ChangeScheme(object sender, EventArgs e)
        {
            InputMessageBox inputBox = new InputMessageBox();
            string newScheme = inputBox.ShowDialog(Resources.EnterNewScheme);
            if (inputBox.DialogResult == DialogResult.Cancel) return;
            
            if (String.IsNullOrWhiteSpace(newScheme))
            {
                ErrorBox.ShowDialog("Invalid scheme");
                return;
            }
            

            int i, count = _dataGrid.SelectedRows.Count;
            for (i = count - 1; i > -1; i--)
            {
                DataGridViewRow row = _dataGrid.SelectedRows[i];

                //get the id
                string stringId = (string)row.Cells["_id"].Value;
                int id = Convert.ToInt32(stringId);
                row.Cells["_scheme"].Value = newScheme;
                TVRequestInfo reqInfo = _dataSource.GetRequestInfo(id);

                reqInfo.IsHttps = newScheme.Equals("https", StringComparison.OrdinalIgnoreCase);
                ApplyHighlighting(reqInfo, stringId, row.Index);
            }
        }

        private void rIAExploreToolStripMenuItem_Click(object sender, EventArgs e)
        {

            _progressBar.Visible = true;
            _progressTimer.Start();

            BackgroundWorker worker = new BackgroundWorker();
            //worker.DoWork += CrawlerDoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();

        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _progressBar.Visible = false;
            _progressTimer.Stop();
        }



        private void replicateHeadersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReplicateHeadersSelectedRequestsAction action = new ReplicateHeadersSelectedRequestsAction(this, _dataGrid);
            action.Execute();
        }



        private void selectFlowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectFlowSelectedRequestsAction action = new SelectFlowSelectedRequestsAction(this, _dataGrid);
            action.Execute();
        }

        private void saveResponseBodyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveResponseSelectedRequestsAction action = new SaveResponseSelectedRequestsAction(this, _dataGrid);
            action.Execute();
        }



        private void FilterEnter(object sender, EventArgs e)
        {
            _textFilter.BackColor = Color.Yellow;
        }

        private void FilterLeave(object sender, EventArgs e)
        {
            _textFilter.BackColor = Color.White;
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            SetFilter(_textFilter.Text, _reverseFilter.Checked);
        }

        private void ClearClick(object sender, EventArgs e)
        {
            this.SuspendLayout();
            _dataSource.Clear();
            this.ResumeLayout();
        }

        private void setEncryptedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in _dataGrid.SelectedRows)
            {
                int index = Convert.ToInt32(row.Cells["_id"].Value);
                var req = _dataSource.GetRequestInfo(index);
                if (req != null)
                {
                    //load the decrypted info
                    byte[] reqData = _dataSource.LoadRequestData(index);
                    byte[] respData = _dataSource.LoadResponseData(index);

                    req.IsEncrypted = !req.IsEncrypted;
                    _dataSource.SaveRequestResponse(index, reqData, respData);
                }
            }
        }

        private void ResendRequestsSingleThread(object sender, EventArgs e)
        {
            ResendSelected(1);
        }

        private void ResendRequestsMultiThread(object sender, EventArgs e)
        {
            ResendSelected(10);
        }


        private void ResendSelected(int numThreads)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(ResendSelectedAsync);
            
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ResendRequestsWorkerCompleted);

            worker.RunWorkerAsync(numThreads);

            //_progressDialog = new ProgressDialog();
            //_progressDialog.Start();
        }



        private void ResendSelectedAsync(object sender, DoWorkEventArgs e)
        {

            int numThreads = (int)e.Argument;


            try
            {
                int i, n = _dataGrid.SelectedRows.Count;

               
                List<int> idsToSend = new List<int>();

                for (i = n - 1; i >= 0; i--)
                {
                    DataGridViewRow row = _dataGrid.SelectedRows[i];
                    string stringId = (string)row.Cells["_id"].Value;
                    int id = Convert.ToInt32(stringId);

                    idsToSend.Add(id);

                }
                DefaultNetworkSettings networkSettings = new DefaultNetworkSettings();
                networkSettings.CertificateValidationCallback = new RemoteCertificateValidationCallback(SSLValidationCallback.ValidateRemoteCertificate);
                if (TrafficViewerOptions.Instance.UseProxy)
                {
                    WebProxy proxy = new WebProxy(TrafficViewerOptions.Instance.HttpProxyServer, TrafficViewerOptions.Instance.HttpProxyPort);
                    networkSettings.WebProxy = proxy;
                }
                RequestSender.RequestSender reqSender = new RequestSender.RequestSender(networkSettings);

                reqSender.Send(_dataSource, idsToSend, numThreads);


            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.ExceptionHasOccured, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Error playing back requests {0}", ex.Message);
            }

        }



        /**
        void CrawlerDoWork(object sender, DoWorkEventArgs e)
        {
            ManualExploreProxy proxy = new ManualExploreProxy("127.0.0.1", 0, TrafficViewer.Instance.TrafficDataAccessor);
            proxy.Start();

            List<Cookie> cookies;
            string address = GetAddress(null, out cookies);
            Crawler crawler = new Crawler();
            crawler.ProxyHost = proxy.Host;
            crawler.ProxyPort = proxy.Port;
            crawler.MaxNumberOfEventsToExecute = 200;
            crawler.EnableLogging = true;
			
            crawler.Init();
			
			
            crawler.CrawlUrl(address, cookies);
		

            crawler.Kill();
            proxy.Stop();
			
        }
        **/


    }
}
