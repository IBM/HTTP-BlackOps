using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Net;

namespace TrafficViewerSDK.Http
{
	internal static class WebRequestCommonSettingsInitializer
	{
		private static bool _initialized = false;

		private const int CanonicalizeAsFilePath = 0x1000000;
		private const int UnEscapeDotsAndSlashes = 0x2000000;
		private const int ConvertPathSlashes = 0x400000;
		private const int SimpleUserSyntax = 0x20000;

		public static void Initialize()
		{
			if (!_initialized)
			{
				//set the default connection limit to higher than normal, which is 2
				ServicePointManager.DefaultConnectionLimit = 100;

				//workaround for bug in .Net URI that removes trailing dots
				MethodInfo getSyntax = typeof(UriParser).GetMethod("GetSyntax", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
				FieldInfo flagsField = typeof(UriParser).GetField("m_Flags", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				if (getSyntax != null && flagsField != null)
				{
					foreach (string scheme in new[] { "http", "https" })
					{
						UriParser parser = (UriParser)getSyntax.Invoke(null, new object[] { scheme });
						if (parser != null)
						{
							int flagsValue = (int)flagsField.GetValue(parser);
							flagsValue = flagsValue & ~CanonicalizeAsFilePath; //this flag causes the Uri to remove trailing dots in path
							flagsValue = flagsValue & ~UnEscapeDotsAndSlashes; //this flag causes the Uri to url decode slashes
							flagsValue = flagsValue & ~ConvertPathSlashes; //this flag causes the Uri to convert backslash to forward slash
							flagsValue = flagsValue & ~SimpleUserSyntax;

							flagsField.SetValue(parser, flagsValue & ~0x400000);

						}
					}
				}
			}
		}
	}

}
