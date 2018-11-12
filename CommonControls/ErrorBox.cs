using CommonControls.Properties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using TrafficViewerSDK;

namespace CommonControls
{
	public class ErrorBox : IExceptionMessageHandler
	{
		public static void ShowDialog(string errorMessage)
		{
			MessageBox.Show(errorMessage, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		#region IExceptionMessageHandler Members

		public void Show(string message)
		{
			ShowDialog(message);
		}

		#endregion
	}
}
