using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.AnalysisModules;

namespace TestCustomAnalysisModule
{
	/// <summary>
	/// Implements the results of this module
	/// </summary>
	public class TestCustomAnalysisModuleResults : IAnalysisResults
	{
		#region IAnalysisResults Members

		public string ResultText
		{
			get { return "This is how you display analysis results"; }
		}

		public object ResultObject
		{
			get { return null; }
		}

		#endregion


        public string ResultBrowserContent
        {
            get { return null; }
        }

        public string ResultBrowserContentExtension
        {
            get { return null; }
        }
    }
}
