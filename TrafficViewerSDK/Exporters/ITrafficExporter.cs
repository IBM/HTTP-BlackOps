using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TrafficViewerSDK.Exporters
{
	/// <summary>
	/// Exports the current traffic file to a different format
	/// </summary>
	public interface ITrafficExporter : ITrafficViewerExtension
	{
		/// <summary>
		/// Gets the display name of the exporter
		/// </summary>
		string Caption
		{
			get;
		}

		/// <summary>
		/// Gets the extension for the specified export file
		/// </summary>
		string Extension
		{
			get;
		}

		/// <summary>
		/// Executes the export operation and overrites the scheme(SSL), host and port of each request
		/// </summary>
		/// <param name="source">The traffic viewer data accessor to use</param>
		/// <param name="stream">Stream to export to (e.g. File)</param>
		/// <param name="newHost">Replacement host</param>
		/// <param name="newPort">Replacement port</param>
		void Export(ITrafficDataAccessor source, Stream stream, string newHost, int newPort);


		/// <summary>
		/// Executes the export operation using the information in the scan
		/// </summary>
		/// <param name="source">The traffic viewer data accessor to use</param>
		/// <param name="stream">Stream to export to (e.g. File)</param>
		void Export(ITrafficDataAccessor source, Stream stream);
	}
}
