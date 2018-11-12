namespace TrafficViewerSDK
{
	/// <summary>
	/// Enum used for importers
	/// </summary>
	public enum ImportMode
	{ 
		/// <summary>
		/// Imports files
		/// </summary>
		Files = 1,
		/// <summary>
		/// Imports from objects such as AppScan
		/// </summary>
		Objects = Files << 1,
		/// <summary>
		/// Supports tailing
		/// </summary>
		Tail = Objects << 1,
		/// <summary>
		/// Supports all types of imports
		/// </summary>
		All = Files | Objects | Tail
	}

	/// <summary>
	/// Enums the types of traffic viewer extension functionality
	/// </summary>
	public enum TrafficViewerExtensionFunction
	{ 
		/// <summary>
		/// Not specified
		/// </summary>
		Disabled,
		/// <summary>
		/// Parser
		/// </summary>
		TrafficParser,
		/// <summary>
		/// Exporter such as manual explore exd data
		/// </summary>
		TrafficExporter,
		/// <summary>
		/// Analysis module
		/// </summary>
		AnalysisModule,
		/// <summary>
		/// Factory of http clients
		/// </summary>
		HttpClientFactory,
		/// <summary>
		/// Sender
		/// </summary>
		Sender,
		/// <summary>
		/// Exploiter module
		/// </summary>
		Exploiter,
        /// <summary>
		/// Proxy Factory module
		/// </summary>
		HttpProxyFactory
	}

	/// <summary>
	/// Types of log messages
	/// </summary>
    public enum LogMessageType
    {
		/// <summary>
		/// Unknown
		/// </summary>
		None,
		/// <summary>
		/// Error
		/// </summary>
        Error,
		/// <summary>
		/// Warning
		/// </summary>
        Warning,
		/// <summary>
		/// Information
		/// </summary>
        Information,
		/// <summary>
		/// Debug
		/// </summary>
        Debug,
        /// <summary>
        /// Notification
        /// </summary>
        Notification
    }

	/// <summary>
	/// Exposes the state of the accessor for the traffic data
	/// </summary>
    public enum AccessorState
    { 
		/// <summary>
		/// The accessor is not doing anything
		/// </summary>
        Idle,
		/// <summary>
		/// The data file is being unpacked
		/// </summary>
		Unpacking,
		/// <summary>
		/// The data was unpacked and is being loaded
		/// </summary>
        Loading,
		/// <summary>
		/// The current state is tailing to a traffic log
		/// </summary>
		Tailing,
		/// <summary>
		/// Saving the file
		/// </summary>
        Saving,
		/// <summary>
		/// Clearing the existing data and the traffic file
		/// </summary>
		Clearing,
		/// <summary>
		/// Removing specific entries from the file
		/// </summary>
		RemovingEntries
    }

	/// <summary>
	/// Status for traffic parse
	/// </summary>
    public enum TrafficParserStatus
    { 
		/// <summary>
		/// Stopped
		/// </summary>
        Stopped,
		/// <summary>
		/// Running
		/// </summary>
        Running
    }

	/// <summary>
	/// The location in the request of a variable
	/// </summary>
	public enum RequestLocation
	{ 
		/// <summary>
		/// Path
		/// </summary>
		Path,
		/// <summary>
		/// Query
		/// </summary>
		Query,
		/// <summary>
		/// Cookies
		/// </summary>
		Cookies,
		/// <summary>
		/// Body
		/// </summary>
		Body,
        /// <summary>
		/// Headers
		/// </summary>
		Headers
	}

	/// <summary>
	/// The mode of the traffic server
	/// </summary>
	public enum TrafficServerMode
	{
		/// <summary>
		/// Used mostly for regressions and product scans, when you want to insure the product 
		/// behavior is consistent across multiple scans.
		/// Good for in-session detection problems where session id values matter
		/// </summary>
		Strict,
		/// <summary>
		/// Removes cookies but still requires parameter values to match
		/// </summary>
		IgnoreCookies,
		/// <summary>
		/// Used mostly for viewing the site in a browser or to reproduce javascript issues when
		/// you need
		/// </summary>
		BrowserFriendly
	}

	/// <summary>
	/// Used with IO operations
	/// </summary>
	public enum LineEnding
	{ 
		/// <summary>
		/// Strictly use the DOS \r\n line ending
		/// </summary>
		DosOnly,
		/// <summary>
		/// Also support *nix line ending (\n only)
		/// </summary>
		Any
	}
}