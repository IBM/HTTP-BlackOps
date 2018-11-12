using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using TrafficViewerSDK.Options;

namespace TrafficViewerSDK.AnalysisModules
{
	/// <summary>
	/// Loads a list of custom analysis modules
	/// </summary>
	public class AnalysisModulesFactory : BaseExtensionFactory<IAnalysisModule>
	{
		/// <summary>
		/// Property specifying what will the extension do
		/// </summary>
		protected override TrafficViewerExtensionFunction ExtensionFunction
		{
			get { return TrafficViewerExtensionFunction.AnalysisModule; }
		}

	}
}
