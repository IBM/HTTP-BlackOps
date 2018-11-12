using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Options;
using System.Reflection;
using System.IO;

namespace TrafficViewerSDK.Senders
{
	/// <summary>
	/// Constructs senders for the list right click menu
	/// </summary>
	public class SenderFactory : BaseExtensionFactory<ISender>
	{
		/// <summary>
		/// The type of this extension
		/// </summary>
		protected override TrafficViewerExtensionFunction ExtensionFunction
		{
			get { return TrafficViewerExtensionFunction.Sender; }
		}

	}
}
