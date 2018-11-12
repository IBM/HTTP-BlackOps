using System;
using TrafficViewerSDK;

namespace TrafficViewerSDK.Http
{
	/// <summary>
	/// Interface that allows the Http server to send messages to the UI or a log file
	/// </summary>
	public interface ITrafficServerConsoleOutput
	{
		/// <summary>
		/// Start was requested
		/// </summary>
		event EventHandler StartRequested;
		/// <summary>
		/// The staus of the console
		/// </summary>
		TVConsoleStatus Status { get; set; }
		/// <summary>
		/// Stop was requested
		/// </summary>
		event EventHandler StopRequested;
		/// <summary>
		/// Write a line
		/// </summary>
		/// <param name="type"></param>
		/// <param name="message"></param>
		void WriteLine(LogMessageType type,string message);

	}

	/// <summary>
	/// Status of the console
	/// </summary>
	public enum TVConsoleStatus
	{
		/// <summary>
		/// If the http server was started
		/// </summary>
		ServerStarted,
		/// <summary>
		/// If the http server was stopped
		/// </summary>
		ServerStopped
	}
}
