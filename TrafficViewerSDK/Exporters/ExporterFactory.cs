using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Options;
using System.Reflection;
using System.IO;

namespace TrafficViewerSDK.Exporters
{
	/// <summary>
	/// Constructs exporters
	/// </summary>
	public class ExporterFactory : BaseExtensionFactory<ITrafficExporter>
	{

		/// <summary>
		/// Returns the extension function
		/// </summary>
		protected override TrafficViewerExtensionFunction ExtensionFunction
		{
			get { return TrafficViewerExtensionFunction.TrafficExporter; }
		}

		/// <summary>
		/// Returns all available exporters
		/// </summary>
		/// <returns></returns>
		public override IList<ITrafficExporter> GetExtensions()
		{
			IList<ITrafficExporter> exporters = base.GetExtensions();
			exporters.Add(new ManualExploreExporter());
			exporters.Add(new LoginExporter());
			exporters.Add(new SequenceExporter());
			exporters.Add(new ASEExdExporter());
			return exporters;
		}
	}
}
