/******************************************************************
* IBM Confidential
* OCO Source Materials
* IBM Rational Traffic Viewer
* (c) Copyright IBM Corp. 2010 All Rights Reserved.
* 
* The source code for this program is not published or otherwise
* divested of its trade secrets, irrespective of what has been
* deposited with the U.S. Copyright Office.
******************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;

namespace TrafficViewerControls
{
	public class ImportDialogResult
	{
		private DialogResult _dialogResult = DialogResult.OK;
		/// <summary>
		/// Returns OK or Cancel depending on the user's choice
		/// </summary>
		public DialogResult DialogResult
		{
			get { return _dialogResult; }
			set { _dialogResult = value; }
		}

		private string _targetFile = String.Empty;
		/// <summary>
		/// Full path to the file to import
		/// </summary>
		public string TargetFile
		{
			get { return _targetFile; }
			set { _targetFile = value; }
		}

		private string _parserPath = String.Empty;
		/// <summary>
		/// The path to the parser dll
		/// </summary>
		public string ParserPath
		{
			get { return _parserPath; }
			set { _parserPath = value; }
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
