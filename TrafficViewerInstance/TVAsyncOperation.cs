using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TrafficViewerSDK;
using System.Diagnostics;

namespace TrafficViewerInstance
{
	public class TVAsyncOperation
	{
		private Thread _thread;
		private ParameterizedThreadStart _start;
		private IExceptionMessageHandler _handler;
		private object _opLock = new object();
		private AsyncCallback _successCallback;
		private AsyncCallback _errorCallback;

		public void ExecuteAsyncOperation(object param)
		{
			lock (_opLock)
			{
				try
				{
					_start.Invoke(param);
					if (_successCallback != null)
					{
						_successCallback.Invoke(null);
					}
				}
				catch (Exception ex)
				{
                    SdkSettings.Instance.Logger.Log(TraceLevel.Error, ex.Message);
                    if (_handler != null)
                    {
                        _handler.Show(ex.Message);
                    }
					if (_errorCallback != null)
					{
						_errorCallback.Invoke(null);
					}
				}
				finally
				{
					;
				}
			}
		}

		/// <summary>
		/// Executes an operation asyncroneusly
		/// </summary>
		/// <param name="start"></param>
		/// <param name="exceptionHandler">Object doing something with the exception</param>
		/// <param name="successCallback">On Success</param>
		/// <param name="errorCallback">On Error</param>
		public TVAsyncOperation(ParameterizedThreadStart start, IExceptionMessageHandler exceptionHandler, AsyncCallback successCallback, AsyncCallback errorCallback)
		{
			_thread = new Thread(ExecuteAsyncOperation);
			_start = start;
			_handler = exceptionHandler;
			_successCallback = successCallback;
			_errorCallback = errorCallback;
		}

		/// <summary>
		/// Starts an async operation
		/// </summary>
		/// <param name="param"></param>
		public void Start(object param)
		{
			_thread.Start(param);
		}
	}
}
