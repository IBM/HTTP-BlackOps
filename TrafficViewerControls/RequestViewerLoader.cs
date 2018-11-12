using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;
using System.ComponentModel;
using TrafficViewerSDK.Http;
using TrafficViewerSDK.Search;
using System.Threading;

namespace TrafficViewerControls
{
	/// <summary>
	/// Encapsulates the logic required to load the tabs of the RequestViewer asyncroneusly
	/// </summary>
	public class RequestViewerLoader
	{
		#region Fields

		/// <summary>
		/// The GUI control that will be loaded
		/// </summary>
		private RequestViewer _control;

		/// <summary>
		/// The source for the traffic data
		/// </summary>
		private ITrafficDataAccessor _dataSource;

		/// <summary>
		/// The result of the async load request operation
		/// </summary>
		private string _requestText = String.Empty;

		/// <summary>
		/// The result of the async load response operation
		/// </summary>
		private string _responseText = String.Empty;

		/// <summary>
		/// Caches the response bytes for the current request to be used with the browser view as well
		/// </summary>
		private byte[] _responseBytes;

		/// <summary>
		/// Stores the last loaded id
		/// </summary>
		private int _currentId = -1;

		/// <summary>
		/// Lock used to syncronize events that control the background worker
		/// </summary>
		private readonly object _workerLock = new object();

        /// <summary>
        /// Stores the requests that were called for loading
        /// </summary>
        private Stack<int> _requestStack = new Stack<int>();

        /// <summary>
        /// Background thread responsible for loading a request
        /// </summary>
        private BackgroundWorker _loadWorker = new BackgroundWorker();


		/// <summary>
		/// Occurs when a new request was added to the data source
		/// </summary>
		private event TVDataAccessorDataEvent RequestAddedEventHandler;

		/// <summary>
		/// Occurs when a new response was added to the data source
		/// </summary>
		private event TVDataAccessorDataEvent ResponseAddedEventHandler;

		/// <summary>
		/// Occurs when the data source was cleared
		/// </summary>
		private event TVDataAccessorDataEvent DataClearedEventHandler;
		private bool _workerOn = true;		
        

		#endregion

		public RequestViewerLoader(RequestViewer control, ITrafficDataAccessor dataSource)
		{
			_control = control;
			_dataSource = dataSource;

			//attach to the data source events
			RequestAddedEventHandler += new TVDataAccessorDataEvent(DataSourceRequestAdded);
			ResponseAddedEventHandler += new TVDataAccessorDataEvent(DataSourceResponseAdded);
			DataClearedEventHandler += new TVDataAccessorDataEvent(DataCleared);

			_dataSource.RequestChanged += RequestAddedEventHandler;
			_dataSource.ResponseChanged += ResponseAddedEventHandler;
			_dataSource.DataCleared += DataClearedEventHandler;
			_dataSource.RequestBatchRemoved += new TVDataAccessorDataBatchEvent(RequestBatchRemoved);

			//attach to the control selected index changed
			_control.SelectedIndexChanged += new EventHandler(ControlSelectedIndexChanged);

			//attach an action to the save events coming from the viewer
			_control.TrafficView.SaveRequested += new RequestTrafficViewSaveEvent(TrafficViewSaveRequested);

            _loadWorker.DoWork += new DoWorkEventHandler(LoadWorkerExecute);
			_loadWorker.RunWorkerAsync();
		}


        void LoadWorkerExecute(object sender, DoWorkEventArgs e)
        {
			while (_workerOn)
			{
				int requestId = -1;

				lock (_workerLock)
				{
					if (_requestStack.Count > 0)
					{
						requestId = _requestStack.Pop();
						_requestStack.Clear();
					}
				}


				if (requestId != -1)
				{
					LoadRequest(requestId);
				}

				Thread.Sleep(10);
			}
		
        }

		/// <summary>
		/// Tells the loader that it should start loading the specified request
		/// </summary>
		/// <param name="requestId"></param>
		public void CallLoadRequest(int requestId)
		{
			lock (_workerLock)
			{
				_requestStack.Push(requestId);
			}
		}

		void RequestBatchRemoved(TVDataAccessorBatchEventArgs e)
		{
			if (_dataSource.RequestCount == 0)
			{
				ClearData();
			}
		}

		~RequestViewerLoader()
		{
			//detach events from the data source
			_workerOn = false;
			_dataSource.RequestChanged -= RequestAddedEventHandler;
			_dataSource.ResponseChanged -= ResponseAddedEventHandler;
			_dataSource.DataCleared -= DataClearedEventHandler;
		}


		private void TrafficViewSaveRequested(RequestTrafficViewSaveArgs e)
		{
			if (_currentId > -1)
			{
				//update the request info
				//parse the data
				HttpRequestInfo reqInfo = new HttpRequestInfo(e.Request);
				_responseBytes = Constants.DefaultEncoding.GetBytes(e.Response);
				HttpResponseInfo respInfo = new HttpResponseInfo();
				respInfo.ProcessResponse(_responseBytes);

				TVRequestInfo currentTVInfo = _dataSource.GetRequestInfo(_currentId);

				//do not set the dom uniqueness id, needs to be explicitly calculated
				currentTVInfo.DomUniquenessId = String.Empty;
				currentTVInfo.RequestLine = reqInfo.RequestLine;
				currentTVInfo.RequestTime = DateTime.Now;
				currentTVInfo.ResponseStatus = respInfo.Status.ToString();
				currentTVInfo.ResponseTime = DateTime.Now;
				currentTVInfo.Description = "Traffic Viewer Request";
				currentTVInfo.ThreadId = "[0000]";

				//convert the strings to bytes
				RequestResponseBytes reqData = new RequestResponseBytes();
				reqData.AddToRequest(Constants.DefaultEncoding.GetBytes(e.Request));
				reqData.AddToResponse(_responseBytes);

				//save the requests to the current data source
				_dataSource.SaveRequest(_currentId, reqData);
				_dataSource.SaveResponse(_currentId, reqData);

				_requestText = e.Request;
				_responseText = e.Response;

			}
		}

		private void DataCleared(TVDataAccessorDataArgs e)
		{
			ClearData();
		}

		private void ClearData()
		{
			_currentId = -1;
			//clear all cached information
			_responseBytes = new byte[0];
			_requestText = String.Empty;
			_responseText = String.Empty;
			_control.CrossThreadPopulateTrafficView(String.Empty, String.Empty);
		}

		/// <summary>
		/// Finishes incomplete requests
		/// </summary>
		/// <param name="e"></param>
		private void DataSourceRequestAdded(TVDataAccessorDataArgs e)
		{
            lock (_workerLock)
            {
                if (e.RequestId != _currentId)
                {
                    return;
                }
            }
            LoadRequest(e.RequestId);
		}

		/// <summary>
		/// Finishes incomplete responses
		/// </summary>
		/// <param name="e"></param>
		private void DataSourceResponseAdded(TVDataAccessorDataArgs e)
		{
            lock (_workerLock)
            {
                if (e.RequestId != _currentId)
                {
                    return;
                }
            }
            LoadRequest(e.RequestId);
		}

		/// <summary>
		/// Loads the request and response data to the GUI
		/// </summary>
		/// <param name="requestId"></param>
		/// <param name="dataSource">Traffic viewer file or other data source</param>
		private void LoadRequest(int requestId)
		{
			byte[] requestBytes = _dataSource.LoadRequestData(requestId);

			TVRequestInfo reqInfo = _dataSource.GetRequestInfo(requestId);
			Encoding enc = Constants.DefaultEncoding;
            /*
			if (reqInfo != null && reqInfo.Description.Contains("Binary"))
			{
				enc = new TrafficViewerEncoding();
			}*/

			if (requestBytes != null)
			{
				_requestText = enc.GetString(requestBytes);
			}
			else
			{
				_requestText = String.Empty;
			}

			_responseBytes = _dataSource.LoadResponseData(requestId);


			if (_responseBytes != null)
			{
				_responseText = enc.GetString(_responseBytes);
			}
			else
			{
				_responseBytes = new byte[0];
				_responseText = String.Empty;
			}

			_currentId = requestId;

			LoadSelectedView();

		}



		/// <summary>
		/// Handles a replace event
		/// </summary>
		/// <param name="replaceArgs"></param>
		public void HandleReplace(ReplaceEventArgs replaceArgs)
		{
			foreach (LineMatch match in replaceArgs.Matches)
			{
				if (match.RequestId == _currentId)
				{
					LoadRequest(_currentId);//reload the request data
					break;
				}
			}

		}


		/// <summary>
		/// Loads data into selected view
		/// </summary>
		private void LoadSelectedView()
		{
			if (_control.SelectedTab == RequestViewerTabs.HttpTraffic)
			{
				_control.CrossThreadPopulateTrafficView(_requestText, _responseText);
			}
            if (_control.SelectedTab == RequestViewerTabs.Entities)
            {
                _control.LoadEntitiesView(_currentId,_dataSource,_requestText);
            }
			else if (_control.SelectedTab == RequestViewerTabs.Browser ||
					_control.SelectedTab == RequestViewerTabs.DOM)
			{
				if (_responseBytes!=null && _responseBytes.Length > 0)
				{
					_control.NavigateBrowser(_dataSource.GetRequestInfo(_currentId), _responseBytes);
				}
			}
			else if (_control.SelectedTab == RequestViewerTabs.LogSync)
			{
				TVRequestInfo reqHeader = _dataSource.GetRequestInfo(_currentId);
				if (reqHeader != null)
				{
					_control.LogSyncViewEvent = reqHeader.RequestTime;
				}
			}
		}

		/// <summary>
		/// Loads the corresponding sub control when the used changes the selected tab
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ControlSelectedIndexChanged(object sender, EventArgs e)
		{
			LoadSelectedView();
		}

	}
}
