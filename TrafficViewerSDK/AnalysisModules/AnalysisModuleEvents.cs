using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK.AnalysisModules
{
	/// <summary>
	/// Used to send a log message
	/// </summary>
	/// <param name="e"></param>
	public delegate void AnalysisModuleLogEvent(AnalysisModuleLogMessage e);

	/// <summary>
	/// Contains the log message
	/// </summary>
	public class AnalysisModuleLogMessage : EventArgs
	{
		private string _message = String.Empty;
		/// <summary>
		/// Log message
		/// </summary>
		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public AnalysisModuleLogMessage(string format, params object[] args)
		{
			_message = String.Format(format,args);
		}
	}


	/// <summary>
	/// Triggered when an analysis module was clicked in the GUI
	/// </summary>
	/// <param name="e"></param>
	public delegate void AnalysisModuleClickEvent(AnalysisModuleClickArgs e);

	/// <summary>
	/// Contains information abput the analysis module that was activated
	/// </summary>
	public class AnalysisModuleClickArgs : EventArgs
	{
		private IAnalysisModule _module;
		/// <summary>
		/// Gets/sets a reference to the analysis module that was requested
		/// </summary>
		public IAnalysisModule Module
		{
		  get { return _module; }
		  set { _module = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="module"></param>
		public AnalysisModuleClickArgs(IAnalysisModule module)
		{
			_module = module;
		}
	}
}
