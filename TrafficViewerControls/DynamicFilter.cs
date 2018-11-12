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
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using TrafficViewerSDK;

namespace TrafficViewerControls
{
	/// <summary>
	/// Contains the logic that allows dynamic filtering of the request Queue
	/// </summary>
	public class DynamicFilter
	{
		/// <summary>
		/// The gird to be filtered
		/// </summary>
		private DataGridView _grid;

		/// <summary>
		/// The amount of painting events to process at a time
		/// </summary>
		private const int FILTER_EVENTS_QUEUE_SIZE = 100;

		/// <summary>
		/// The amount of time to wait between keystrokes
		/// </summary>
		private const int USER_WAIT = 500;

		/// <summary>
		/// The amount of time between displays
		/// </summary>
		private const int DISPLAY_DELAY = 20;

		/// <summary>
		/// The estimated length of a row. Used to allocate memory for the string builder.
		/// </summary>
		private const int ROW_ESTIMATED_LENGTH = 255;

		/// <summary>
		/// Lock used to syncronize threads in the dynamic filter
		/// </summary>
		private object _filterLock = new object();

		/// <summary>
		/// Used to give the painter event the go ahead
		/// </summary>
		AutoResetEvent _paintSignal;

		/// <summary>
		/// Used to coordinate between filter operations
		/// </summary>
		ManualResetEvent _newFilterSignal;

		/// <summary>
		/// Used to coordinate init filter start
		/// </summary>
		ManualResetEvent _initFilterSignal;

		/// <summary>
		/// Flag used to interrupt gracefully an ongoing filter operation
		/// </summary>
		private bool _stopRequested = false;

		/// <summary>
		/// The current filter applied to the Queue
		/// </summary>
		private string _filter = String.Empty;

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
		/// True if the _initThread was started
		/// </summary>
		private bool _initStarted = false;

		/// <summary>
		/// Form operation friendly thread
		/// </summary>
		private BackgroundWorker _displayWorker = new BackgroundWorker();

		/// <summary>
		/// Contains the logic for initializing the filter
		/// </summary>
		private void InitThreadMethod()
		{
			_initStarted = true;
			//as long as the user keeps changing the filter don't start anything
			while (_filterChanged)
			{
				_filterChanged = false;
				Thread.Sleep(USER_WAIT);
			}

			//signal the SetFilter method that at this point it should wait before starting a new init thread
			_initFilterSignal.Reset();
			//wait for the old filter thread to finish
			_newFilterSignal.WaitOne();

			Thread _filterThread;

			if (!_filterChanged) //if the filter was not changed while we were waiting from the filterThread to stop start a new thread
			{
				_filterThread = new Thread(new ThreadStart(FilterThreadMethod));
				//start the filter thread
				_filterThread.Start();
			}
		}

		/// <summary>
		/// The method responsible for the filter operation
		/// </summary>
		private void FilterThreadMethod()
		{
			//allow SetFilter to start new init threads
			_initFilterSignal.Set();

			_initStarted = false;

			//now that this filter has control reset the signal
			_newFilterSignal.Reset();

			//reset the stopRequested flag
			_stopRequested = false;

			lock (_filterLock)
			{
				_filter = _newFilter;
				_reverseFilter = _newReverseFilter;
			}

			bool visible;
			Queue<KeyValuePair<DataGridViewRow, bool>> eventQueue = new Queue<KeyValuePair<DataGridViewRow, bool>>();

			foreach (DataGridViewRow row in _grid.Rows)
			{
				if (_stopRequested)
				{
					break;
				}
				visible = GetRowVisibility(row);
				eventQueue.Enqueue(new KeyValuePair<DataGridViewRow, bool>(row, visible));

				if (eventQueue.Count >= FILTER_EVENTS_QUEUE_SIZE)
				{
					//wait for the previous async operation to complete before starting a new one
					_paintSignal.WaitOne();
					//start the worker
					_paintSignal.Reset();
					_displayWorker.RunWorkerAsync(eventQueue);
					//start a  new queue
					eventQueue = new Queue<KeyValuePair<DataGridViewRow, bool>>();
				}
			}

			//drain remaining requests
			if (eventQueue.Count > 0 && !_stopRequested)
			{
				_paintSignal.WaitOne();
				_displayWorker.RunWorkerAsync(eventQueue);
			}

			_newFilterSignal.Set();
		}

		/// <summary>
		/// Processes display tasks
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DisplayWork(object sender, DoWorkEventArgs e)
		{
			Queue<KeyValuePair<DataGridViewRow, bool>> taskQueue =
				(Queue<KeyValuePair<DataGridViewRow, bool>>)e.Argument;

			KeyValuePair<DataGridViewRow, bool> currentTask;

			_grid.Invoke((MethodInvoker)delegate
			{
				while (taskQueue.Count > 0 && !_stopRequested)
				{
					currentTask = taskQueue.Dequeue();
					currentTask.Key.Visible = currentTask.Value;
				}
			});

			Thread.Sleep(DISPLAY_DELAY); //sleep a bit to allow user interaction with the list
		}

		void DisplayWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			_paintSignal.Set();
		}

		/// <summary>
		/// Calculates the visibility of a row according to the value of the filter
		/// </summary>
		/// <returns>True if visible, false if not</returns>
		public bool GetRowVisibility(DataGridViewRow row)
		{
			//make a string from the row
			StringBuilder sb = new StringBuilder(ROW_ESTIMATED_LENGTH);

			foreach (DataGridViewCell cell in row.Cells)
			{
				sb.Append(cell.Value);
				sb.Append(" ");
			}

			string s = sb.ToString();

			//calculate visibility
			bool visible;

			if (Utils.IsMatch(s, _filter) || s.IndexOf(_filter, StringComparison.OrdinalIgnoreCase) > -1)
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
		/// Sets a filter, interrupting any other ongoing filter operation 
		/// </summary>
		/// <param name="regex"></param>
		/// <param name="reverse"></param>
		public void SetFilter(string regex, bool reverse)
		{
			//inform _initThread that the filter was changed
			_filterChanged = true;

			//signal any other filter operation to stop 
			_stopRequested = true;

			lock (_filterLock)
			{
				_newFilter = regex;
				_newReverseFilter = reverse;
			}

			Thread _initThread;

			//wait if necessary before eventually starting the init thread
			_initFilterSignal.WaitOne();


			//start the init thread which will wait a little bit to give the user enough time
			//to finish typing
			if (!_initStarted)
			{
				_initThread = new Thread(new ThreadStart(InitThreadMethod));
				_initThread.Start();
			}

		}

		/// <summary>
		/// Constructs a DynamicFilter class
		/// </summary>
		/// <param name="grid"></param>
		public DynamicFilter(DataGridView grid)
		{
			_grid = grid;
			_newFilterSignal = new ManualResetEvent(true);
			_initFilterSignal = new ManualResetEvent(true);
			_paintSignal = new AutoResetEvent(true);
			_displayWorker.DoWork += new DoWorkEventHandler(DisplayWork);
			_displayWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DisplayWorkerCompleted);
		}

	}
}
