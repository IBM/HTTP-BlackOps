using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK.Options;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace TrafficViewerSDK
{
	/// <summary>
	/// Base class for Traffic Viewer Extension factories
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class BaseExtensionFactory<T>
	{
		/// <summary>
		/// Property specifying what will the extension do
		/// </summary>
		protected abstract TrafficViewerExtensionFunction ExtensionFunction
		{
			get;
		}

		
		/// <summary>
		/// Gets the list of extensions
		/// </summary>
		/// <returns>List of extensions</returns>
		public virtual IList<T> GetExtensions()
		{
			List<T> extensionList = new List<T>();
			Dictionary<string, TrafficViewerExtensionFunction> extInfoList = SdkSettings.Instance.ExtensionInfoList;
			foreach (string path in extInfoList.Keys)
			{
				if (extInfoList[path] == ExtensionFunction)
				{
					//try to load the module
					T extension = default(T);
					try
					{
						Assembly dll = Assembly.LoadFile(path);

						string fullClassName = Path.GetFileNameWithoutExtension(path);
						fullClassName += "." + fullClassName;

						extension = (T)dll.CreateInstance(fullClassName);


					}
					catch (Exception ex)
					{
                        SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Cannot load extension of type '{2}' at path '{0}'. Reason: '{1}'", path, ex.Message, typeof(T));
					}

					if (extension != null)
					{
						extensionList.Add(extension);
					}
				}
			}

			return extensionList;
		}

		/// <summary>
		/// Finds extension of the specified ExtensionFunction
		/// </summary>
		/// <returns></returns>
        private IList<T> FindExtension()
		{
			List<T> extensionList = new List<T>();
			Dictionary<string, TrafficViewerExtensionFunction> extInfoList = SdkSettings.Instance.ExtensionInfoList;
			foreach (string path in extInfoList.Keys)
			{
				if (extInfoList[path] == ExtensionFunction)
				{
					//try to load the module
					T extension = default(T);
					try
					{
						Assembly dll = Assembly.LoadFile(path);

						string fullClassName = Path.GetFileNameWithoutExtension(path);
						fullClassName += "." + fullClassName;

						extension = (T)dll.CreateInstance(fullClassName);


					}
					catch (Exception ex)
					{
                        SdkSettings.Instance.Logger.Log(TraceLevel.Error, "Cannot load extension of type '{2}' at path '{0}'. Reason: '{1}'", path, ex.Message, typeof(T));
					}

					if (extension != null)
					{
						extensionList.Add(extension);
					}
				}
			}

			return extensionList;
		}
	}
}
