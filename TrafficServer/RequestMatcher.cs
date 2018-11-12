using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficViewerSDK.Http;
using TrafficViewerSDK;

namespace TrafficServer
{
	public class RequestMatcher
	{
		/// <summary>
		/// Compares two requests infos
		/// </summary>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public static bool IsMatch(HttpRequestInfo first, HttpRequestInfo second, TrafficServerMode mode)
		{
			bool result;

			if (mode == TrafficServerMode.IgnoreCookies)
			{
				result = first.QueryVariables.GetHashCode() == second.QueryVariables.GetHashCode()
						&& first.BodyVariables.GetHashCode() == second.BodyVariables.GetHashCode();
			}
			else if (mode == TrafficServerMode.Strict)
			{
				result = first.GetHashCode() == second.GetHashCode();
			}
			else
			{
				result = first.QueryVariables.GetHashCode(false) == second.QueryVariables.GetHashCode(false)
						&& first.BodyVariables.GetHashCode(false) == second.BodyVariables.GetHashCode(false);
			}

			return result;

		}
	}
}
