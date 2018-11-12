using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Properties;

namespace TrafficViewerSDK.Exporters
{
	/// <summary>
	/// AppScan Enterprise EXD Exporter
	/// </summary>
	public class ASEExdExporter : ManualExploreExporter
	{
		/// <summary>
		/// Parameter name to exclude from export
		/// </summary>
		protected override string ParameterExclusion
		{
			get
			{
				return "patternParameter";
			}
		}

		/// <summary>
		/// The Caption of the exporter
		/// </summary>
		public override string Caption
		{
			get
			{
				return Resources.ASEEXDCaption;
			}
		}
	}
}
