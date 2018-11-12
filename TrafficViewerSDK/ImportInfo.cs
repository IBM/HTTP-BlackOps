using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Options;
using TrafficViewerSDK.Importers;

namespace TrafficViewerSDK
{
	/// <summary>
	/// Contains information about an import
	/// </summary>
	public class ImportInfo
	{
		private object _sender = null;
		/// <summary>
		/// Sender containing the traffic data, for example AppScan
		/// </summary>
		public object Sender
		{
			get { return _sender; }
			set { _sender = value; }
		}

		private List<string> _targetFiles = new List<string>();
		/// <summary>
		/// Full path to the file to import
		/// </summary>
		public List<string> TargetFiles
		{
			get { return _targetFiles; }
			set { _targetFiles = value; }
		}

		private ITrafficParser _parser;
		/// <summary>
		/// The path to the parser dll
		/// </summary>
		public ITrafficParser Parser
		{
			get { return _parser; }
			set { _parser = value; }
		}

		private ParsingOptions _profile = new ParsingOptions();
		/// <summary>
		/// The the current parsing options
		/// </summary>
		public ParsingOptions Profile
		{
			get { return _profile; }
			set { _profile = value; }
		}

		private bool _append = false;
		/// <summary>
		/// Wether the previous requests will be kept or not
		/// </summary>
		public bool Append
		{
			get { return _append; }
			set { _append = value; }
		}
	}
}
