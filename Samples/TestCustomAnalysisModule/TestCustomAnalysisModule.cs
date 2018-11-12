using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.AnalysisModules;

namespace TestCustomAnalysisModule
{
	/// <summary>
	/// Test custom analysis module used only to show how to implement the interface
	/// </summary>
	public class TestCustomAnalysisModule : IAnalysisModule
	{
		/// <summary>
		/// Logs a message to the GUI
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		protected void LogMessage(string format, params object[] args)
		{
			if (LogEvent != null)
			{
				LogEvent.Invoke(new AnalysisModuleLogMessage(format, args));
			}
		}

		#region IAnalysisModule Members

		/// <summary>
		/// The text displayed in the analysis modules menu
		/// </summary>
		public string Caption
		{
			get { return "Test Module Caption"; }
		}

		/// <summary>
		/// The module description
		/// </summary>
		public string Description
		{
			get { return "Test Module Description"; }
		}

		/// <summary>
		/// This function is called when the user chooses the start button for this module in the
		/// Traffic Viewer Analysis Module wizard
		/// </summary>
		/// <param name="source">Traffic viewer passes the cusrrent traffic file in this argument</param>
		/// <returns>A analysis module result to be used by traffic viewer</returns>
		public IAnalysisResults PerformAnalysis(TrafficViewerSDK.ITrafficDataAccessor source)
		{
			return new TestCustomAnalysisModuleResults();
		}

		/// <summary>
		/// The Traffic Viewer GUI hooks a function to this event to display log messages to the user
		/// </summary>
		public event AnalysisModuleLogEvent LogEvent;

		#endregion
	}
}
