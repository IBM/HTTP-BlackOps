using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViewerInstance;
using TrafficViewerSDK;

namespace TrafficServer
{
	/// <summary>
	/// Removes dynamic elements from a response
	/// </summary>
	public class DynamicElementsRemover
	{
		private static List<string> _dynElems = TrafficViewerOptions.Instance.GetDynamicElements();

		/// <summary>
		/// Removes user defined dynamic elements from the raw request text
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static string Remove(string request)
		{
			foreach (string elem in _dynElems)
			{
				request = Utils.ReplaceGroups(request, elem, TrafficViewerSDK.Constants.DYNAMIC_ELEM_STRING);
			}
			return request;
		}
	}
}
