using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using TrafficViewerSDK;
using TrafficViewerSDK.Options;
using System.Diagnostics;

namespace TrafficViewerControls
{
	public static class TVColorConverter
	{
		/// <summary>
		/// Creates a color from a string
		/// </summary>
		/// <param name="colorString">
		/// Known color or string using the following format A,R,G,B,</param>
		/// <returns>Color</returns>
		public static Color GetColorFromString(string colorString)
		{
			ColorConverter c = new ColorConverter();
			Color col = new Color();

			try
			{
				col = (Color)c.ConvertFrom(colorString);
			}
			catch 
			{
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Invalid color definition {0}", colorString);
			}

			return col;
		}

		/// <summary>
		/// Retrieves the highlighting color based on request description
		/// </summary>
		/// <param name="profile">Profile options for this file</param>
		/// <param name="description">The request description</param>
		/// <returns>Color</returns>
		public static Color GetColorForRequestDescription(ParsingOptions profile, string description)
		{
			Dictionary<string, string> hDefs = profile.GetHighlightingDefinitions();

			//check first if there is an exact match
			if (hDefs.ContainsKey(description))
			{
				return TVColorConverter.GetColorFromString(hDefs[description]);
			}
			else
			{
				//check for all definitions if there is a partial match
				foreach (KeyValuePair<string, string> hDef in hDefs)
				{
                    if (Utils.IsMatch(description, hDef.Key))
					{
						return  TVColorConverter.GetColorFromString(hDef.Value);
					}
				}
			}
			return Color.Black;
		}

		/// <summary>
		/// Retrieves the highlighting color based on request id
		/// </summary>
		/// <param name="profile">Profile options for this file</param>
		/// <param name="description">The request description</param>
		/// <returns>Color</returns>
		public static Color GetColorForRequestId(ParsingOptions profile, string requestId)
		{
			Dictionary<string, string> hDefs = profile.GetHighlightingDefinitions();

			//check first if there is an exact match
			if (hDefs.ContainsKey(requestId))
			{
				return TVColorConverter.GetColorFromString(hDefs[requestId]);
			}
			else
			{
				return Color.Black;
			}
		}

		/// <summary>
		/// Creates an ARGB string using the format 'A,R,G,B'
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public static string GetARGBString(Color c)
		{
			return String.Format("{0},{1},{2},{3}",c.A,c.R,c.G,c.B);
		}

	}
}
