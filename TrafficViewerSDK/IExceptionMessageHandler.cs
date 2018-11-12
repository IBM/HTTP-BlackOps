using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK
{
	/// <summary>
	/// Interface used to handle exceptions, could be a UI element or a log
	/// </summary>
	public interface IExceptionMessageHandler
	{
		/// <summary>
		/// Shows the message
		/// </summary>
		/// <param name="message"></param>
		void Show(string message);
	}
}
