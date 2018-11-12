using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;
using TrafficViewerSDK.Options;

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

		private ImportInfo _importInfo = new ImportInfo();
		/// <summary>
		/// Contains information about the import
		/// </summary>
		public ImportInfo ImportInfo
		{
			get { return _importInfo; }
			set { _importInfo = value; }
		}

		
	}
}
