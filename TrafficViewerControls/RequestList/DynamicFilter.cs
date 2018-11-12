using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using TrafficViewerSDK;
using System.Collections;

namespace TrafficViewerControls.RequestList
{
	/// <summary>
	/// Contains the logic that allows dynamic filtering of the request Queue
	/// </summary>
	internal class DynamicFilter
	{
		/// <summary>
		/// The grid to be filtered
		/// </summary>
		private TVRequestsList _list;
		/// <summary>
		/// The amount of painting events to process at a time
		/// </summary>
		private const int FILTER_EVENTS_QUEUE_SIZE = 250;
		/// <summary>
		/// The amount of time to wait between keystrokes
		/// </summary>
		private const int USER_WAIT = 500;
		/// <summary>
		/// The amount of time between displays
		/// </summary>
		private const int DISPLAY_DELAY = 100;
		/// <summary>
		/// Flag used to interrupt gracefully an ongoing filter operation
		/// </summary>
		private bool _stopRequested = false;
		/// <summary>
		/// The current filter applied to the Queue
		/// </summary>
		private string _filter = String.Empty;
		/// <summary>
		/// The current filter applied to the Queue
		/// </summary>
		public string Filter
		{
			get { return _filter; }
		}
		/// <summary>
		/// The new filter applied to the Queue
		/// </summary>
		private string _newFilter = String.Empty;
		/// <summary>
		/// If the filter should be reversed or not
		/// </summary>
		private bool _reverseFilter = false;
		/// <summary>
		/// If the filter should be reversed or not
		/// </summary>
		private bool _newReverseFilter = false;
		/// <summary>
		/// Flag used to indicate to the init thread that the user changed the filter
		/// </summary>
		private bool _filterChanged = false;
		/// <summary>
		/// Wether the init thread has started
		/// </summary>
		private bool _initStarted = false;
		/// <summary>
		/// Used to make new filter requests wait until the ongoing operation has finished
		/// </summary>
		private ManualResetEvent _filterWorkerEvent = new ManualResetEvent(true);
		/// <summary>
		/// Controls changing the filter value
		/// </summary>
		private object _setFilterLock = new object();


		#region Threads

		/// <summary>
		/// Sets a filter, interrupting any other ongoing filter operation 
		/// </summary>
		/// <param name="regex"></param>
		/// <param name="reverse"></param>
		public void SetFilter(string regex, bool reverse)
		{
			//make set filter an atomic operation
			lock (_setFilterLock)
			{
				if (regex == _filter && reverse == _reverseFilter)
				{
					return; //discard any set filter requests that are the same with the current filter
				}

				//signal any ongoing filter operation to stop 
				_stopRequested = true;
				//inform InitThread that the filter was changed
				_filterChanged = true;
				//update the new values
				_newFilter = regex;
				_newReverseFilter = reverse;

				//if the init thread was started drop the current thread
				if (!_initStarted)
				{
					_initStarted = true;

					//start the init thread which will wait a little bit to give the user enough time
					Thread initThread = new Thread(new ThreadStart(InitThreadMethod));
					initThread.Start();
				}
			}
		}

		/// <summary>
		/// Contains the logic for initializing the filter
		/// </summary>
		private void InitThreadMethod()
		{
			//as long as the user keeps changing the filter don't start anything
			while (_filterChanged)
			{
				_filterChanged = false;
				Thread.Sleep(USER_WAIT);
			}

			//the user had enough chances to change the filter, lock SetFilter until the new
			//filter operation is started
			lock (_setFilterLock)
			{

				//wait for any existing operation to complete
				_filterWorkerEvent.WaitOne();

				//now that no filter threads are running start a new one

				//set the new filter values
				_filter = _newFilter;
				_reverseFilter = _newReverseFilter;

				Thread _filterThread = new Thread(new ThreadStart(FilterThreadMethod));

				//start the filter thread
				_filterThread.Start();

				//allow other init threads to start
				_initStarted = false;
			}
		}

		/// <summary>
		/// The method responsible for the filter operation
		/// </summary>
		private void FilterThreadMethod()
		{
			//block anew new threads from starting
			_filterWorkerEvent.Reset();

			//reset the stopRequested flag
			_stopRequested = false;


			FilterTaskQueue taskQueue = new FilterTaskQueue();

			//clear all the rows and re-add them, unfortunately due to performance issues
			//in DataGridView we can't just hide rows
			_list.Invoke((MethodInvoker)delegate
			{
				_list.ClearAllRows();
			});

			TVRequestInfo header;

			int currIndex = -1;

			//calculate visibility and re-add to the list
			while ((header = _list.DataSource.GetNext(ref currIndex)) != null)
			{
				if (_stopRequested)
				{
					break;
				}

				taskQueue.Enqueue(header);

				if (taskQueue.Count >= FILTER_EVENTS_QUEUE_SIZE)
				{
					Display(taskQueue);	
				}
			}

			//drain remaining requests
			if (taskQueue.Count > 0 && !_stopRequested)
			{
				Display(taskQueue);
			}

			//allow new threads to start
			_filterWorkerEvent.Set();
		}

		/// <summary>
		/// Starts a display operation
		/// </summary>
		/// <param name="taskQueue">Queue with request headers that should be added to the list</param>
		private void Display(FilterTaskQueue taskQueue)
		{
			TVRequestInfo currentTask;

			_list.Invoke((MethodInvoker)delegate
			{
                _list.SuspendLayout();
				while (taskQueue.Count > 0 && !_stopRequested)
				{
					currentTask = taskQueue.Dequeue();
					_list.AddRow(currentTask, false);
				}
                _list.ResumeLayout();
				if (!_stopRequested)
				{
					Thread.Sleep(DISPLAY_DELAY); //sleep a bit to allow user interaction with the list
				}

			});

		}

		#endregion

		/// <summary>
		/// Calculates the visibility of a row according to the value of the filter
		/// </summary>
		/// <returns>True if visible, false if not</returns>
		public bool GetRowVisibility(string s)
		{
			bool visible;

            //process the matches in the priority = empty is always visible, substring, regex
            if (String.IsNullOrWhiteSpace(_filter) || s.IndexOf(_filter, StringComparison.CurrentCultureIgnoreCase) > -1 || Utils.IsMatch(s, _filter))
			{
				visible = true;
			}
			else
			{
				visible = false;
			}

			//and then visibility is affected by the reverse flag; XOR
			return visible ^ _reverseFilter;
		}

		/// <summary>
		/// Cancels all filter operations and shows all the requests in the list
		/// </summary>
		public void ClearFilter()
		{
			this.SetFilter(String.Empty, false);
		}

		/// <summary>
		/// Constructs a DynamicFilter class
		/// </summary>
		/// <param name="grid"></param>
		public DynamicFilter(TVRequestsList grid)
		{
			_list = grid;
		}

	}

	internal class FilterTaskQueue : Queue<TVRequestInfo> { }
}
