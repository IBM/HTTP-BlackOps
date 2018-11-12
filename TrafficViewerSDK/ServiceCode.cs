namespace TrafficViewerSDK
{
	/// <summary>
	/// Each code must have a corresponding string resource. Unlike the codes
	/// used for other components, these do not have to be in any particular order.
	/// </summary>
	public enum ServiceCode
	{
		/// <summary>
		/// Encountered when the request cannot be completed because the target server has an invalid certificate.
		/// </summary>
		OnSecureInvalidCertificate = 1404,
		/// <summary>
		/// Encountered when platform authentication cannot be completed because the job configuration doesn't have credentials configured
		/// </summary>
		InvalidPlatformAuthenticationSettings = 1405,
		/// <summary>
		/// Encountered when there is a problem connecting to the site through the proxy
		/// </summary>
		ProxyErrorSendingRequestToTheSite = 1406,
		/// <summary>
		/// Encountered when there is an error in the proxy
		/// </summary>
		ProxyInternalError = 1407,
		/// <summary>
		/// Encountered when the server violates a protocol
		/// </summary>
		ServerProtocolViolation = 1408,
		/// <summary>
		/// Encountered when the command proxy fails to start a proxy because the port is in use
		/// </summary>
		CommandProxyPortInUse = 1409,
		/// <summary>
		/// Encountered when the maxiumum number of recording proxies was reached
		/// </summary>
		CommandProxyMaxiumumProxiesReached = 1410,
		/// <summary>
		/// Encountered when an non-existing port is passed
		/// </summary>
		CommandProxyStopCannotFindPort = 1411,
		/// <summary>
		/// Encountered when an non-existing port is passed
		/// </summary>
		CommandProxyStopInvalidFileName = 1412,
		/// <summary>
		/// Encountered when an invalid port was specified
		/// </summary>
		CommandProxyStartInvalidPort = 1413,
		/// <summary>
		/// Encountered when a unsupported request is sent
		/// </summary>
		CommandProxyNotImplemented = 1414,
		/// <summary>
		/// Encountered when the proxy fails to get created because the specified host is invalid
		/// </summary>
		InvalidProxyHost = 1415,
		/// <summary>
		/// Encountered when the specified file name already exists
		/// </summary>
		CommandProxyStopFileExists = 1416
	}
}