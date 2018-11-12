using System;
using System.Collections.Generic;
using System.Text;
using TrafficViewerSDK;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// This class is used by the traffic proxy to log activity to a 
	/// ITrafficServerConsoleOutput which can be a console form or a log file
	/// </summary>
	public class HttpServerConsole
	{
		ITrafficServerConsoleOutput _output;
		/// <summary>
		/// Gets or sets the output of the traffic server messages
		/// </summary>
		public ITrafficServerConsoleOutput Output
		{
			get { return _output; }
			set 
			{ 
				_output = value; 
			}
		}

		/// <summary>
		/// Writes a message to the console output if an output is available
		/// </summary>
		/// <param name="type"></param>
		/// <param name="message"></param>
		public void WriteLine(LogMessageType type,string message)
		{
			if (_output != null)
			{
                if (type != LogMessageType.Notification)
                {
                    _output.WriteLine(type, String.Format("{0} {1}", DateTime.Now.ToString(), message));
                }
                else
                {
                    _output.WriteLine(type, message);
                }
			}
		}

		/// <summary>
		/// Writes a message to the console output if an output is available
		/// </summary>
		/// <param name="message"></param>
		public void WriteLine(string message)
		{
			this.WriteLine(LogMessageType.Information, message);
		}


		/// <summary>
		/// Writes a line to the current HTTP console
		/// </summary>
		/// <param name="ex"></param>
		public void WriteLine(Exception ex)
		{
			this.WriteLine(LogMessageType.Error,ex.Message);
		}

		/// <summary>
		/// Writes a message to the console output if an output is available
		/// </summary>
		/// <param name="type">Error log type</param>
		/// <param name="message">Formatted string, e.g. Starting proxy on port {0}</param>
		/// <param name="args">List of arguments</param>
		public void WriteLine(LogMessageType type,string message,params object[] args)
		{
			this.WriteLine(type,String.Format(message,args));
		}

		/// <summary>
		/// Writes a message to the console output if an output is available
		/// </summary>
		/// <param name="message">Formatted string, e.g. Starting proxy on port {0}</param>
		/// <param name="args">List of arguments</param>
		public void WriteLine(string message, params object[] args)
		{
			this.WriteLine(String.Format(message, args));
		}


		private static HttpServerConsole _instance;

		private static object _instanceLock = new object();
		/// <summary>
		/// The current server console instance
		/// </summary>
		public static HttpServerConsole Instance
		{
			get
			{
				lock (_instanceLock)
				{
					if (_instance == null)
					{
						_instance = new HttpServerConsole();
					}
				}
				return _instance;
			}
		}


	}
}
