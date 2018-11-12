using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK.AnalysisModules
{
	/// <summary>
	/// Defines an analysis module
	/// </summary>
	public interface IAnalysisModule : ITrafficViewerExtension
	{
		/// <summary>
		/// The string displayed unde the Analysis menu
		/// </summary>
		string Caption
		{
			get;
		}

		/// <summary>
		/// Detailed description regarding the functionality of the module
		/// </summary>
		string Description
		{
			get;
		}

		/// <summary>
		/// Executes the analysis algorithms on the specified data source
		/// </summary>
		/// <param name="source"></param>
		/// <returns>Analysis results</returns>
		IAnalysisResults PerformAnalysis(ITrafficDataAccessor source);

		/// <summary>
		/// Occurs when the analysis module has to log a message
		/// </summary>
		event AnalysisModuleLogEvent LogEvent;

	}
}
