using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK.Senders
{
	/// <summary>
	/// Handles send actions from the list of requests
	/// </summary>
	public interface ISender : ITrafficViewerExtension
	{
		/// <summary>
		/// The sender name displayed in the UI
		/// </summary>
		string Caption
		{
			get;
		}

		/// <summary>
		/// Sends one or more requests
		/// </summary>
		/// <param name="requests"></param>
		void Send(IEnumerable<TVRequestInfo> requests);
	}
}
