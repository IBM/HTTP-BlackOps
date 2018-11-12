using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using TrafficViewerSDK.Options;
using System.IO;
using System.Diagnostics;

namespace TrafficViewerSDK.Importers
{

	/// <summary>
	/// Constructs traffic parsers or importers
	/// </summary>
    public class TrafficParserFactory : BaseExtensionFactory<ITrafficParser>
    {
        /// <summary>
        /// Instantiates a Traffic Log parser from external user code
        /// </summary>
        /// <param name="dllPath">Location of the user dll</param>
        /// <returns>Traffic Parser instance</returns>
        public static ITrafficParser Create(string dllPath)
        {
			ITrafficParser result=null;
			try
			{
				Assembly dll = Assembly.LoadFile(dllPath);
				string parserClass = Path.GetFileName(dllPath);
				//the extension should have the same class name and name space as the file name
				parserClass = parserClass.Replace(".dll", "");
				string fullClassName = String.Format("{0}.{0}",parserClass);
				result = (ITrafficParser)dll.CreateInstance(fullClassName);
			}
			catch(Exception ex)
			{
                SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Error opening parsing DLL: {0}", ex.Message);
			}
			return result;
        }

		/// <summary>
		/// The functionality of this extension
		/// </summary>
		protected override TrafficViewerExtensionFunction ExtensionFunction
		{
			get { return TrafficViewerExtensionFunction.TrafficParser; }
		}

		/// <summary>
		/// Gets all available parsers
		/// </summary>
		/// <returns></returns>
		public override IList<ITrafficParser> GetExtensions()
		{
			IList<ITrafficParser> trafficParsers = base.GetExtensions();
			//add the default parsers
			trafficParsers.Add(new DefaultTrafficParser());
			trafficParsers.Add(new ConfigurationParser());
			trafficParsers.Add(new UriParser());

			return trafficParsers;
		}
    }
}
