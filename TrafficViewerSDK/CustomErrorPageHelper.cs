using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficViewerSDK
{
	/// <summary>
	/// Helps identify if a page is a custom error page
	/// </summary>
	public static class CustomErrorPageHelper
	{
		private static List<string> _regexList;

		static CustomErrorPageHelper()
		{
			_regexList = new List<string>();
			_regexList.Add("404.{1,15}Error");
			_regexList.Add("Error.{1,15}404");
			_regexList.Add("404.{1,15}Found");
			_regexList.Add("Page.{1,15}not.{1,15}found");
			_regexList.Add("Resource.{1,15}not.{1,15}found");
			_regexList.Add("not.{1,15}exist");
			_regexList.Add("error.{1,15}request");
			_regexList.Add("request.{1,15}error");
			_regexList.Add("Unable to open");
			_regexList.Add("Unable to find");
			_regexList.Add("No such file");
			_regexList.Add("Invalid Action");
			_regexList.Add("Generic (Application )?Error");
		}

		/// <summary>
		/// Returns true if the response matches the custom error pages regexes
		/// </summary>
		/// <param name="responseBody"></param>
		/// <returns></returns>
		public static bool IsCustomErrorPage(string responseBody)
		{
			foreach (string regex in _regexList)
			{
				if (Utils.IsMatch(responseBody, regex))
				{
					return true;
				}
			}

			return false;
		}
	}
}
